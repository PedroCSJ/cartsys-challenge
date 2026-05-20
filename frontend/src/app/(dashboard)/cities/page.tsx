"use client";

import { useEffect, useState, useCallback } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { toast } from "sonner";
import { Plus, Pencil, Trash2, Search } from "lucide-react";
import api from "@/lib/api";
import { getErrorMessage, formatDate } from "@/lib/utils";
import type { City, State, PagedResult } from "@/types";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
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

const citySchema = z.object({
  name: z.string().min(2, "Nome deve ter ao menos 2 caracteres"),
  stateId: z.string().uuid("Selecione um estado"),
});

type CityForm = z.infer<typeof citySchema>;

export default function CitiesPage() {
  const [cities, setCities] = useState<City[]>([]);
  const [states, setStates] = useState<State[]>([]);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [search, setSearch] = useState("");
  const [filterState, setFilterState] = useState<string>("");
  const [isLoading, setIsLoading] = useState(true);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [deleteDialog, setDeleteDialog] = useState({ open: false, id: "" });
  const [editing, setEditing] = useState<City | null>(null);
  const [deleting, setDeleting] = useState(false);

  const {
    register,
    handleSubmit,
    reset,
    setValue,
    watch,
    formState: { errors, isSubmitting },
  } = useForm<CityForm>({ resolver: zodResolver(citySchema) });

  const selectedStateId = watch("stateId");

  const fetchCities = useCallback(async () => {
    setIsLoading(true);
    try {
      const res = await api.get<PagedResult<City>>("/cities", {
        params: {
          name: search || undefined,
          stateId: filterState || undefined,
          page,
          pageSize: 10,
        },
      });
      setCities(res.data.items);
      setTotal(res.data.totalCount);
      setTotalPages(res.data.totalPages);
    } catch (err) {
      toast.error(getErrorMessage(err));
    } finally {
      setIsLoading(false);
    }
  }, [search, filterState, page]);

  useEffect(() => {
    fetchCities();
  }, [fetchCities]);

  useEffect(() => {
    api.get<State[]>("/states/all").then((r) => setStates(r.data));
  }, []);

  function openCreate() {
    setEditing(null);
    reset({ name: "", stateId: "" });
    setDialogOpen(true);
  }

  function openEdit(city: City) {
    setEditing(city);
    reset({ name: city.name, stateId: city.stateId });
    setDialogOpen(true);
  }

  async function onSubmit(data: CityForm) {
    try {
      if (editing) {
        await api.put(`/cities/${editing.id}`, data);
        toast.success("Cidade atualizada com sucesso!");
      } else {
        await api.post("/cities", data);
        toast.success("Cidade cadastrada com sucesso!");
      }
      setDialogOpen(false);
      fetchCities();
    } catch (err) {
      toast.error(getErrorMessage(err));
    }
  }

  async function handleDelete() {
    setDeleting(true);
    try {
      await api.delete(`/cities/${deleteDialog.id}`);
      toast.success("Cidade excluída com sucesso!");
      setDeleteDialog({ open: false, id: "" });
      fetchCities();
    } catch (err) {
      toast.error(getErrorMessage(err));
    } finally {
      setDeleting(false);
    }
  }

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold">Cidades</h1>
          <p className="text-sm text-muted-foreground">{total} registros encontrados</p>
        </div>
        <Button onClick={openCreate}>
          <Plus size={16} className="mr-2" />
          Nova Cidade
        </Button>
      </div>

      <Card>
        <CardHeader className="pb-3">
          <div className="flex items-center gap-2 flex-wrap">
            <div className="relative flex-1 max-w-sm">
              <Search size={14} className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" />
              <Input
                placeholder="Buscar por nome..."
                className="pl-9"
                value={search}
                onChange={(e) => { setSearch(e.target.value); setPage(1); }}
              />
            </div>
            <Select value={filterState || "all"} onValueChange={(v) => { setFilterState(v === "all" ? "" : (v ?? "")); setPage(1); }}>
              <SelectTrigger className="w-48">
                <SelectValue placeholder="Filtrar por estado" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">Todos os estados</SelectItem>
                {states.map((s) => (
                  <SelectItem key={s.id} value={s.id}>{s.name} ({s.uf})</SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
        </CardHeader>
        <CardContent>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Nome</TableHead>
                <TableHead>Estado</TableHead>
                <TableHead>Cadastrado em</TableHead>
                <TableHead className="w-24">Ações</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {isLoading ? (
                <TableRow>
                  <TableCell colSpan={4} className="text-center py-8 text-muted-foreground">Carregando...</TableCell>
                </TableRow>
              ) : cities.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={4} className="text-center py-8 text-muted-foreground">Nenhuma cidade encontrada.</TableCell>
                </TableRow>
              ) : (
                cities.map((c) => (
                  <TableRow key={c.id}>
                    <TableCell className="font-medium">{c.name}</TableCell>
                    <TableCell>{c.stateName} <span className="text-muted-foreground">({c.stateUF})</span></TableCell>
                    <TableCell className="text-muted-foreground">{formatDate(c.createdAt)}</TableCell>
                    <TableCell>
                      <div className="flex items-center gap-1">
                        <Button variant="ghost" size="icon" onClick={() => openEdit(c)}>
                          <Pencil size={14} />
                        </Button>
                        <Button
                          variant="ghost"
                          size="icon"
                          className="text-destructive hover:text-destructive"
                          onClick={() => setDeleteDialog({ open: true, id: c.id })}
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
        <DialogContent className="sm:max-w-md">
          <DialogHeader>
            <DialogTitle>{editing ? "Editar Cidade" : "Nova Cidade"}</DialogTitle>
          </DialogHeader>
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4 pt-2">
            <div className="space-y-1.5">
              <Label htmlFor="city-name">Nome</Label>
              <Input id="city-name" placeholder="Ex: Belo Horizonte" {...register("name")} />
              {errors.name && <p className="text-xs text-destructive">{errors.name.message}</p>}
            </div>
            <div className="space-y-1.5">
              <Label>Estado</Label>
              <Select value={selectedStateId} onValueChange={(v) => setValue("stateId", v)}>
                <SelectTrigger>
                  <SelectValue placeholder="Selecione um estado" />
                </SelectTrigger>
                <SelectContent>
                  {states.map((s) => (
                    <SelectItem key={s.id} value={s.id}>{s.name} ({s.uf})</SelectItem>
                  ))}
                </SelectContent>
              </Select>
              {errors.stateId && <p className="text-xs text-destructive">{errors.stateId.message}</p>}
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
        title="Excluir Cidade"
        description="Tem certeza que deseja excluir esta cidade?"
        onConfirm={handleDelete}
        loading={deleting}
      />
    </div>
  );
}
