"use client";

import { useEffect, useState, useCallback } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { toast } from "sonner";
import { Plus, Pencil, Trash2, Search, FileDown } from "lucide-react";
import api from "@/lib/api";
import { getErrorMessage } from "@/lib/utils";
import type { Developer, City, State, Language, PagedResult } from "@/types";
import { SeniorityLabels } from "@/types";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { Badge } from "@/components/ui/badge";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Card, CardContent, CardHeader } from "@/components/ui/card";
import { ConfirmDialog } from "@/components/shared/ConfirmDialog";
import { Pagination } from "@/components/shared/Pagination";

const devSchema = z.object({
  name: z.string().min(2, "Nome deve ter ao menos 2 caracteres"),
  email: z.string().email("E-mail inválido"),
  seniority: z.coerce.number().min(1).max(3),
  cityId: z.string().uuid("Selecione uma cidade"),
  languageIds: z.array(z.string()).min(1, "Selecione ao menos uma linguagem"),
  notes: z.string().optional(),
});

type DevForm = z.infer<typeof devSchema>;

const seniorityColors: Record<number, string> = {
  1: "bg-green-100 text-green-700 dark:bg-green-900 dark:text-green-300",
  2: "bg-blue-100 text-blue-700 dark:bg-blue-900 dark:text-blue-300",
  3: "bg-purple-100 text-purple-700 dark:bg-purple-900 dark:text-purple-300",
};

export default function DevelopersPage() {
  const [developers, setDevelopers] = useState<Developer[]>([]);
  const [cities, setCities] = useState<City[]>([]);
  const [states, setStates] = useState<State[]>([]);
  const [languages, setLanguages] = useState<Language[]>([]);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [search, setSearch] = useState("");
  const [filterSeniority, setFilterSeniority] = useState("");
  const [filterCity, setFilterCity] = useState("");
  const [filterState, setFilterState] = useState("");
  const [isLoading, setIsLoading] = useState(true);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [deleteDialog, setDeleteDialog] = useState({ open: false, id: "" });
  const [editing, setEditing] = useState<Developer | null>(null);
  const [deleting, setDeleting] = useState(false);
  const [selectedLanguages, setSelectedLanguages] = useState<string[]>([]);
  const [generatingPdf, setGeneratingPdf] = useState(false);
  const [filteredCities, setFilteredCities] = useState<City[]>([]);

  const {
    register,
    handleSubmit,
    reset,
    setValue,
    watch,
    formState: { errors, isSubmitting },
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  } = useForm<DevForm>({ resolver: zodResolver(devSchema) as any });

  const selectedSeniority = watch("seniority");
  const selectedCityId = watch("cityId");

  const fetchDevelopers = useCallback(async () => {
    setIsLoading(true);
    try {
      const res = await api.get<PagedResult<Developer>>("/developers", {
        params: {
          name: search || undefined,
          seniority: filterSeniority || undefined,
          cityId: filterCity || undefined,
          page,
          pageSize: 10,
        },
      });
      setDevelopers(res.data.items);
      setTotal(res.data.totalCount);
      setTotalPages(res.data.totalPages);
    } catch (err) {
      toast.error(getErrorMessage(err));
    } finally {
      setIsLoading(false);
    }
  }, [search, filterSeniority, filterCity, page]);

  useEffect(() => {
    fetchDevelopers();
  }, [fetchDevelopers]);

  useEffect(() => {
    Promise.all([
      api.get<State[]>("/states/all"),
      api.get<Language[]>("/languages/all"),
    ]).then(([statesRes, langsRes]) => {
      setStates(statesRes.data);
      setLanguages(langsRes.data);
    });
  }, []);

  useEffect(() => {
    if (filterState) {
      api.get<City[]>(`/cities/by-state/${filterState}`).then((r) => {
        setFilteredCities(r.data);
      });
    } else {
      setFilteredCities([]);
    }
  }, [filterState]);

  useEffect(() => {
    if (filterState) {
      api.get<City[]>(`/cities/by-state/${filterState}`).then((r) => {
        setCities(r.data);
      });
    }
  }, [filterState]);

  function handleStateChange(stateId: string) {
    setFilterState(stateId === "all" ? "" : stateId);
    setFilterCity("");
    setPage(1);
    if (stateId && stateId !== "all") {
      api.get<City[]>(`/cities/by-state/${stateId}`).then((r) => setFilteredCities(r.data));
    } else {
      setFilteredCities([]);
    }
  }

  function toggleLanguage(id: string) {
    const updated = selectedLanguages.includes(id)
      ? selectedLanguages.filter((l) => l !== id)
      : [...selectedLanguages, id];
    setSelectedLanguages(updated);
    setValue("languageIds", updated, { shouldValidate: true });
  }

  function openCreate() {
    setEditing(null);
    setSelectedLanguages([]);
    reset({ name: "", email: "", seniority: 1, cityId: "", languageIds: [], notes: "" });
    setDialogOpen(true);
  }

  function openEdit(dev: Developer) {
    setEditing(dev);
    const langIds = dev.languages.map((l) => l.id);
    setSelectedLanguages(langIds);
    reset({
      name: dev.name,
      email: dev.email,
      seniority: dev.seniority,
      cityId: dev.cityId,
      languageIds: langIds,
      notes: dev.notes ?? "",
    });

    api.get<City[]>(`/cities/by-state/${dev.cityId}`).catch(() => {});
    setCities([{ id: dev.cityId, name: dev.cityName, stateId: "", stateName: dev.stateName, stateUF: dev.stateUF, createdAt: "" }]);
    setDialogOpen(true);
  }

  async function handleStateDialogChange(stateId: string) {
    setValue("cityId", "");
    if (stateId && stateId !== "none") {
      const res = await api.get<City[]>(`/cities/by-state/${stateId}`);
      setCities(res.data);
    } else {
      setCities([]);
    }
  }

  async function onSubmit(data: DevForm) {
    try {
      const payload = { ...data, languageIds: selectedLanguages };
      if (editing) {
        await api.put(`/developers/${editing.id}`, payload);
        toast.success("Desenvolvedor atualizado com sucesso!");
      } else {
        await api.post("/developers", payload);
        toast.success("Desenvolvedor cadastrado com sucesso!");
      }
      setDialogOpen(false);
      fetchDevelopers();
    } catch (err) {
      toast.error(getErrorMessage(err));
    }
  }

  async function handleDelete() {
    setDeleting(true);
    try {
      await api.delete(`/developers/${deleteDialog.id}`);
      toast.success("Desenvolvedor excluído com sucesso!");
      setDeleteDialog({ open: false, id: "" });
      fetchDevelopers();
    } catch (err) {
      toast.error(getErrorMessage(err));
    } finally {
      setDeleting(false);
    }
  }

  async function handleGeneratePdf() {
    setGeneratingPdf(true);
    try {
      const res = await api.get("/developers/report/pdf", { responseType: "blob" });
      const url = URL.createObjectURL(new Blob([res.data], { type: "application/pdf" }));
      const a = document.createElement("a");
      a.href = url;
      a.download = `relatorio-desenvolvedores-${new Date().toISOString().slice(0, 10)}.pdf`;
      a.click();
      URL.revokeObjectURL(url);
      toast.success("Relatório gerado com sucesso!");
    } catch (err) {
      toast.error(getErrorMessage(err));
    } finally {
      setGeneratingPdf(false);
    }
  }

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold">Desenvolvedores</h1>
          <p className="text-sm text-muted-foreground">{total} registros encontrados</p>
        </div>
        <div className="flex items-center gap-2">
          <Button variant="outline" onClick={handleGeneratePdf} disabled={generatingPdf}>
            <FileDown size={16} className="mr-2" />
            {generatingPdf ? "Gerando..." : "Exportar PDF"}
          </Button>
          <Button onClick={openCreate}>
            <Plus size={16} className="mr-2" />
            Novo Desenvolvedor
          </Button>
        </div>
      </div>

      <Card>
        <CardHeader className="pb-3">
          <div className="flex items-center gap-2 flex-wrap">
            <div className="relative flex-1 max-w-xs">
              <Search size={14} className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" />
              <Input
                placeholder="Buscar por nome..."
                className="pl-9"
                value={search}
                onChange={(e) => { setSearch(e.target.value); setPage(1); }}
              />
            </div>
            <Select value={filterSeniority} onValueChange={(v) => { setFilterSeniority(v === "all" ? "" : v); setPage(1); }}>
              <SelectTrigger className="w-36">
                <SelectValue placeholder="Senioridade" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">Todas</SelectItem>
                {Object.entries(SeniorityLabels).map(([k, v]) => (
                  <SelectItem key={k} value={k}>{v}</SelectItem>
                ))}
              </SelectContent>
            </Select>
            <Select value={filterState} onValueChange={handleStateChange}>
              <SelectTrigger className="w-44">
                <SelectValue placeholder="Estado" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">Todos os estados</SelectItem>
                {states.map((s) => (
                  <SelectItem key={s.id} value={s.id}>{s.name}</SelectItem>
                ))}
              </SelectContent>
            </Select>
            {filteredCities.length > 0 && (
              <Select value={filterCity} onValueChange={(v) => { setFilterCity(v === "all" ? "" : v); setPage(1); }}>
                <SelectTrigger className="w-44">
                  <SelectValue placeholder="Cidade" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">Todas as cidades</SelectItem>
                  {filteredCities.map((c) => (
                    <SelectItem key={c.id} value={c.id}>{c.name}</SelectItem>
                  ))}
                </SelectContent>
              </Select>
            )}
          </div>
        </CardHeader>
        <CardContent>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Nome</TableHead>
                <TableHead>Senioridade</TableHead>
                <TableHead>Cidade / Estado</TableHead>
                <TableHead>Linguagens</TableHead>
                <TableHead className="w-24">Ações</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {isLoading ? (
                <TableRow>
                  <TableCell colSpan={5} className="text-center py-8 text-muted-foreground">Carregando...</TableCell>
                </TableRow>
              ) : developers.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={5} className="text-center py-8 text-muted-foreground">Nenhum desenvolvedor encontrado.</TableCell>
                </TableRow>
              ) : (
                developers.map((d) => (
                  <TableRow key={d.id}>
                    <TableCell>
                      <div>
                        <p className="font-medium">{d.name}</p>
                        <p className="text-xs text-muted-foreground">{d.email}</p>
                      </div>
                    </TableCell>
                    <TableCell>
                      <span className={`text-xs font-medium px-2 py-0.5 rounded-full ${seniorityColors[d.seniority]}`}>
                        {d.seniorityLabel}
                      </span>
                    </TableCell>
                    <TableCell>
                      <p className="text-sm">{d.cityName}</p>
                      <p className="text-xs text-muted-foreground">{d.stateName} ({d.stateUF})</p>
                    </TableCell>
                    <TableCell>
                      <div className="flex flex-wrap gap-1">
                        {d.languages.slice(0, 3).map((l) => (
                          <Badge key={l.id} variant="secondary" className="text-xs">{l.name}</Badge>
                        ))}
                        {d.languages.length > 3 && (
                          <Badge variant="outline" className="text-xs">+{d.languages.length - 3}</Badge>
                        )}
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center gap-1">
                        <Button variant="ghost" size="icon" onClick={() => openEdit(d)}>
                          <Pencil size={14} />
                        </Button>
                        <Button
                          variant="ghost"
                          size="icon"
                          className="text-destructive hover:text-destructive"
                          onClick={() => setDeleteDialog({ open: true, id: d.id })}
                        >
                          <Trash2 size={14} />
                        </Button>
                      </div>
                    </TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
          <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
        </CardContent>
      </Card>

      <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
        <DialogContent className="sm:max-w-lg max-h-[90vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle>{editing ? "Editar Desenvolvedor" : "Novo Desenvolvedor"}</DialogTitle>
          </DialogHeader>
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4 pt-2">
            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-1.5 col-span-2">
                <Label>Nome</Label>
                <Input placeholder="Nome completo" {...register("name")} />
                {errors.name && <p className="text-xs text-destructive">{errors.name.message}</p>}
              </div>
              <div className="space-y-1.5 col-span-2">
                <Label>E-mail</Label>
                <Input type="email" placeholder="dev@email.com" {...register("email")} />
                {errors.email && <p className="text-xs text-destructive">{errors.email.message}</p>}
              </div>
              <div className="space-y-1.5">
                <Label>Senioridade</Label>
                <Select
                  value={String(selectedSeniority || 1)}
                  onValueChange={(v) => setValue("seniority", Number(v) as 1 | 2 | 3)}
                >
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    {Object.entries(SeniorityLabels).map(([k, v]) => (
                      <SelectItem key={k} value={k}>{v}</SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              <div className="space-y-1.5">
                <Label>Estado</Label>
                <Select onValueChange={handleStateDialogChange}>
                  <SelectTrigger>
                    <SelectValue placeholder="Selecione" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="none">Selecione um estado</SelectItem>
                    {states.map((s) => (
                      <SelectItem key={s.id} value={s.id}>{s.name} ({s.uf})</SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              <div className="space-y-1.5 col-span-2">
                <Label>Cidade</Label>
                <Select value={selectedCityId} onValueChange={(v) => setValue("cityId", v)}>
                  <SelectTrigger>
                    <SelectValue placeholder="Selecione uma cidade" />
                  </SelectTrigger>
                  <SelectContent>
                    {cities.map((c) => (
                      <SelectItem key={c.id} value={c.id}>{c.name}</SelectItem>
                    ))}
                  </SelectContent>
                </Select>
                {errors.cityId && <p className="text-xs text-destructive">{errors.cityId.message}</p>}
              </div>
            </div>

            <div className="space-y-2">
              <Label>Linguagens <span className="text-destructive">*</span></Label>
              <div className="flex flex-wrap gap-2 border rounded-md p-3 min-h-[60px]">
                {languages.map((l) => {
                  const selected = selectedLanguages.includes(l.id);
                  return (
                    <button
                      key={l.id}
                      type="button"
                      onClick={() => toggleLanguage(l.id)}
                      className={`text-xs px-2.5 py-1 rounded-full border transition-colors ${
                        selected
                          ? "bg-primary text-primary-foreground border-primary"
                          : "bg-muted text-muted-foreground hover:border-primary"
                      }`}
                    >
                      {l.name}
                    </button>
                  );
                })}
              </div>
              {errors.languageIds && (
                <p className="text-xs text-destructive">{errors.languageIds.message}</p>
              )}
            </div>

            <div className="space-y-1.5">
              <Label>Observações</Label>
              <Textarea placeholder="Informações adicionais..." rows={3} {...register("notes")} />
            </div>

            <div className="flex justify-end gap-2 pt-2">
              <Button type="button" variant="outline" onClick={() => setDialogOpen(false)}>Cancelar</Button>
              <Button type="submit" disabled={isSubmitting}>{isSubmitting ? "Salvando..." : "Salvar"}</Button>
            </div>
          </form>
        </DialogContent>
      </Dialog>

      <ConfirmDialog
        open={deleteDialog.open}
        onOpenChange={(open) => setDeleteDialog({ open, id: deleteDialog.id })}
        title="Excluir Desenvolvedor"
        description="Tem certeza que deseja excluir este desenvolvedor?"
        onConfirm={handleDelete}
        loading={deleting}
      />
    </div>
  );
}
