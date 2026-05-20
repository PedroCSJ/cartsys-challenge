"use client";

import { useEffect, useState, useCallback } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { toast } from "sonner";
import { Plus, Pencil, Trash2, Search } from "lucide-react";
import api from "@/lib/api";
import { getErrorMessage, formatDate } from "@/lib/utils";
import type { State, PagedResult } from "@/types";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
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

const stateSchema = z.object({
  name: z.string().min(2, "Nome deve ter ao menos 2 caracteres"),
  uf: z
    .string()
    .length(2, "UF deve ter exatamente 2 letras")
    .regex(/^[A-Za-z]+$/, "UF deve conter apenas letras"),
});

type StateForm = z.infer<typeof stateSchema>;

export default function StatesPage() {
  const [states, setStates] = useState<State[]>([]);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [search, setSearch] = useState("");
  const [isLoading, setIsLoading] = useState(true);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [deleteDialog, setDeleteDialog] = useState<{
    open: boolean;
    id: string;
  }>({ open: false, id: "" });
  const [editing, setEditing] = useState<State | null>(null);
  const [deleting, setDeleting] = useState(false);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<StateForm>({ resolver: zodResolver(stateSchema) });

  const fetchStates = useCallback(async () => {
    setIsLoading(true);
    try {
      const res = await api.get<PagedResult<State>>("/states", {
        params: { name: search || undefined, page, pageSize: 10 },
      });
      setStates(res.data.items);
      setTotal(res.data.totalCount);
      setTotalPages(res.data.totalPages);
    } catch (err) {
      toast.error(getErrorMessage(err));
    } finally {
      setIsLoading(false);
    }
  }, [search, page]);

  useEffect(() => {
    fetchStates();
  }, [fetchStates]);

  function openCreate() {
    setEditing(null);
    reset({ name: "", uf: "" });
    setDialogOpen(true);
  }

  function openEdit(state: State) {
    setEditing(state);
    reset({ name: state.name, uf: state.uf });
    setDialogOpen(true);
  }

  async function onSubmit(data: StateForm) {
    try {
      if (editing) {
        await api.put(`/states/${editing.id}`, data);
        toast.success("Estado atualizado com sucesso!");
      } else {
        await api.post("/states", data);
        toast.success("Estado cadastrado com sucesso!");
      }
      setDialogOpen(false);
      fetchStates();
    } catch (err) {
      toast.error(getErrorMessage(err));
    }
  }

  async function handleDelete() {
    setDeleting(true);
    try {
      await api.delete(`/states/${deleteDialog.id}`);
      toast.success("Estado excluído com sucesso!");
      setDeleteDialog({ open: false, id: "" });
      fetchStates();
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
          <h1 className="text-2xl font-bold">Estados</h1>
          <p className="text-sm text-muted-foreground">{total} registros encontrados</p>
        </div>
        <Button onClick={openCreate}>
          <Plus size={16} className="mr-2" />
          Novo Estado
        </Button>
      </div>

      <Card>
        <CardHeader className="pb-3">
          <div className="flex items-center gap-2">
            <div className="relative flex-1 max-w-sm">
              <Search size={14} className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" />
              <Input
                placeholder="Buscar por nome..."
                className="pl-9"
                value={search}
                onChange={(e) => {
                  setSearch(e.target.value);
                  setPage(1);
                }}
              />
            </div>
          </div>
        </CardHeader>
        <CardContent>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Nome</TableHead>
                <TableHead>UF</TableHead>
                <TableHead>Cadastrado em</TableHead>
                <TableHead className="w-24">Ações</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {isLoading ? (
                <TableRow>
                  <TableCell colSpan={4} className="text-center py-8 text-muted-foreground">
                    Carregando...
                  </TableCell>
                </TableRow>
              ) : states.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={4} className="text-center py-8 text-muted-foreground">
                    Nenhum estado encontrado.
                  </TableCell>
                </TableRow>
              ) : (
                states.map((s) => (
                  <TableRow key={s.id}>
                    <TableCell className="font-medium">{s.name}</TableCell>
                    <TableCell>
                      <span className="font-mono text-sm bg-muted px-2 py-0.5 rounded">{s.uf}</span>
                    </TableCell>
                    <TableCell className="text-muted-foreground">{formatDate(s.createdAt)}</TableCell>
                    <TableCell>
                      <div className="flex items-center gap-1">
                        <Button variant="ghost" size="icon" onClick={() => openEdit(s)}>
                          <Pencil size={14} />
                        </Button>
                        <Button
                          variant="ghost"
                          size="icon"
                          className="text-destructive hover:text-destructive"
                          onClick={() => setDeleteDialog({ open: true, id: s.id })}
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
            <DialogTitle>{editing ? "Editar Estado" : "Novo Estado"}</DialogTitle>
          </DialogHeader>
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4 pt-2">
            <div className="space-y-1.5">
              <Label htmlFor="name">Nome</Label>
              <Input id="name" placeholder="Ex: Minas Gerais" {...register("name")} />
              {errors.name && <p className="text-xs text-destructive">{errors.name.message}</p>}
            </div>
            <div className="space-y-1.5">
              <Label htmlFor="uf">UF</Label>
              <Input id="uf" placeholder="Ex: MG" maxLength={2} className="uppercase" {...register("uf")} />
              {errors.uf && <p className="text-xs text-destructive">{errors.uf.message}</p>}
            </div>
            <div className="flex justify-end gap-2 pt-2">
              <Button type="button" variant="outline" onClick={() => setDialogOpen(false)}>
                Cancelar
              </Button>
              <Button type="submit" disabled={isSubmitting}>
                {isSubmitting ? "Salvando..." : "Salvar"}
              </Button>
            </div>
          </form>
        </DialogContent>
      </Dialog>

      <ConfirmDialog
        open={deleteDialog.open}
        onOpenChange={(open) => setDeleteDialog({ open, id: deleteDialog.id })}
        title="Excluir Estado"
        description="Tem certeza que deseja excluir este estado? Esta ação não pode ser desfeita."
        onConfirm={handleDelete}
        loading={deleting}
      />
    </div>
  );
}
