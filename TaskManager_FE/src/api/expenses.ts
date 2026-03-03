import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import client from './client';
import type { Expense, ExpenseCategory } from './types';

// ── Expenses ─────────────────────────────────────────────────────────────────

export function useProjectExpenses(projectId: string) {
  return useQuery<Expense[]>({
    queryKey: ['expenses', projectId],
    queryFn: async () => {
      const { data } = await client.get<Expense[]>(`/projects/${projectId}/expenses`);
      return data;
    },
    enabled: !!projectId,
  });
}

export interface CreateExpenseInput {
  projectId: string;
  categoryId?: string;
  amount: number;
  description?: string;
  date: string;
}

export function useCreateExpense(projectId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (input: CreateExpenseInput) => {
      const { data } = await client.post<{ id: string }>('/expenses', input);
      return data.id;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['expenses', projectId] });
    },
  });
}

export interface UpdateExpenseInput {
  categoryId?: string;
  amount: number;
  description?: string;
  date: string;
}

export function useUpdateExpense(projectId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async ({ expenseId, input }: { expenseId: string; input: UpdateExpenseInput }) => {
      await client.put(`/expenses/${expenseId}`, input);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['expenses', projectId] });
    },
  });
}

export function useDeleteExpense(projectId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (expenseId: string) => {
      await client.delete(`/expenses/${expenseId}`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['expenses', projectId] });
    },
  });
}

// ── Categories ────────────────────────────────────────────────────────────────

export function useProjectCategories(projectId: string) {
  return useQuery<ExpenseCategory[]>({
    queryKey: ['expense-categories', projectId],
    queryFn: async () => {
      const { data } = await client.get<ExpenseCategory[]>(`/projects/${projectId}/expense-categories`);
      return data;
    },
    enabled: !!projectId,
  });
}

export function useCreateCategory(projectId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (name: string) => {
      const { data } = await client.post<{ id: string }>('/expense-categories', { projectId, name });
      return data.id;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['expense-categories', projectId] });
    },
  });
}

export function useDeleteCategory(projectId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (categoryId: string) => {
      await client.delete(`/expense-categories/${categoryId}`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['expense-categories', projectId] });
      // expenses that had this category now show as uncategorized — refetch them too
      queryClient.invalidateQueries({ queryKey: ['expenses', projectId] });
    },
  });
}
