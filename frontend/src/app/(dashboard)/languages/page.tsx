"use client";

import { useEffect, useState, useCallback } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { toast } from "sonner";
import { Plus, Pencil, Trash2, Search } from "lucide-react";
import api from "@/lib/api";
import { getErrorMessage, formatDate } from "@/lib/utils";
import type { Language, PagedResult } from "@/types";
import { LanguageTypeLabels } from "@/types";
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

const languageSchema = z.object({
  name: z.string().min(1, "Nome é obrigatório"),
  type: z.coerce.number().min(1).max(5),
});

type LanguageForm = z.infer<typeof languageSchema>;

const typeColors: Record<number, string> = {
  1: "bg-blue-100 text-blue-700 dark:bg-blue-900 dark:text-blue-300",
  2: "bg-green-100 text-green-700 dark:bg-green-900 dark:text-green-300",
  3: "bg-purple-100 text-purple-700 dark:bg-purple-900 dark:text-purple-300",
  4: "bg-orange-100 text-orange-700 dark:bg-orange-900 dark:text-orange-300",
  5: "bg-red-100 text-red-700 dark:bg-red-900 dark:text-red-300",
};

export default function LanguagesPage() {
  const [languages, setLanguages] = useState<Language[]>([]);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [search, setSearch] = useState("");
  const [filterType, setFilterType] = useState<string>("");
  const [isLoading, setIsLoading] = useState(true);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [deleteDialog, setDeleteDialog] = useState({ open: false, id: "" });
  const [editing, setEditing] = useState<Language | null>(null);
  const [deleting, setDeleting] = useState(false);

  const {
    register,
    handleSubmit,
    reset,
    setValue,
    watch,
    formState: { errors, isSubmitting },
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  } = useForm<LanguageForm>({ resolver: zodResolver(languageSchema) as any });

  const selectedType = watch("type");

  const fetchLanguages = useCallback(async () => {
    setIsLoading(true);
    try {
      const res = await api.get<PagedResult<Language>>("/languages", {
        params: {
          name: search || undefined,
          type: filterType || undefined,
          page,
          pageSize: 10,
        },
      });
      setLanguages(res.data.items);
      setTotal(res.data.totalCount);
      setTotalPages(res.data.totalPages);
    } catch (err) {
      toast.error(getErrorMessage(err));
    } finally {
      setIsLoading(false);
    }
  }, [search, filterType, page]);

  useEffect(() => {
    fetchLanguages();
  }, [fetchLanguages]);

  function openCreate() {
    setEditing(null);
    reset({ name: "", type: 1 });
    setDialogOpen(true);
  }

  function openEdit(lang: Language) {
    setEditing(lang);
    reset({ name: lang.name, type: lang.type });
    setDialogOpen(true);
  }

  async function onSubmit(data: LanguageForm) {
    try {
      if (editing) {
        await api.put(`/languages/${editing.id}`, data);
        toast.success("Linguagem atualizada com sucesso!");
      } else {
        await api.post("/languages", data);
        toast.success("Linguagem cadastrada com sucesso!");
      }
      setDialogOpen(false);
      fetchLanguages();
    } catch (err) {
      toast.error(getErrorMessage(err));
    }
  }

  async function handleDelete() {
    setDeleting(true);
    try {
      await api.delete(`/languages/${deleteDialog.id}`);
      toast.success("Linguagem excluída com sucesso!");
      setDeleteDialog({ open: false, id: "" });
      fetchLanguages();
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
          <h1 className="text-2xl font-bold">Linguagens de Programação</h1>
          <p className="text-sm text-muted-foreground">{total} registros encontrados</p>
        </div>
        <Button onClick={openCreate}>
          <Plus size={16} className="mr-2" />
          Nova Linguagem
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
            <Select value={filterType} onValueChange={(v) => { setFilterType(v === "all" ? "" : v); setPage(1); }}>
              <SelectTrigger className="w-44">
                <SelectValue placeholder="Filtrar por tipo" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">Todos os tipos</SelectItem>
                {Object.entries(LanguageTypeLabels).map(([key, label]) => (
                  <SelectItem key={key} value={key}>{label}</SelectItem>
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
                <TableHead>Tipo</TableHead>
                <TableHead>Cadastrado em</TableHead>
                <TableHead className="w-24">Ações</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {isLoading ? (
                <TableRow>
                  <TableCell colSpan={4} className="text-center py-8 text-muted-foreground">Carregando...</TableCell>
                </TableRow>
              ) : languages.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={4} className="text-center py-8 text-muted-foreground">Nenhuma linguagem encontrada.</TableCell>
                </TableRow>
              ) : (
                languages.map((l) => (
                  <TableRow key={l.id}>
                    <TableCell className="font-medium">{l.name}</TableCell>
                    <TableCell>
                      <span className={`text-xs font-medium px-2 py-0.5 rounded-full ${typeColors[l.type]}`}>
                        {l.typeLabel}
                      </span>
                    </TableCell>
                    <TableCell className="text-muted-foreground">{formatDate(l.createdAt)}</TableCell>
                    <TableCell>
                      <div className="flex items-center gap-1">
                        <Button variant="ghost" size="icon" onClick={() => openEdit(l)}>
                          <Pencil size={14} />
                        </Button>
                        <Button
                          variant="ghost"
                          size="icon"
                          className="text-destructive hover:text-destructive"
                          onClick={() => setDeleteDialog({ open: true, id: l.id })}
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
            <DialogTitle>{editing ? "Editar Linguagem" : "Nova Linguagem"}</DialogTitle>
          </DialogHeader>
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4 pt-2">
            <div className="space-y-1.5">
              <Label htmlFor="lang-name">Nome</Label>
              <Input id="lang-name" placeholder="Ex: TypeScript" {...register("name")} />
              {errors.name && <p className="text-xs text-destructive">{errors.name.message}</p>}
            </div>
            <div className="space-y-1.5">
              <Label>Tipo</Label>
              <Select
                value={String(selectedType || 1)}
                onValueChange={(v) => setValue("type", Number(v) as 1 | 2 | 3 | 4 | 5)}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Selecione o tipo" />
                </SelectTrigger>
                <SelectContent>
                  {Object.entries(LanguageTypeLabels).map(([key, label]) => (
                    <SelectItem key={key} value={key}>{label}</SelectItem>
                  ))}
                </SelectContent>
              </Select>
              {errors.type && <p className="text-xs text-destructive">{errors.type.message}</p>}
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
        title="Excluir Linguagem"
        description="Tem certeza que deseja excluir esta linguagem?"
        onConfirm={handleDelete}
        loading={deleting}
      />
    </div>
  );
}
