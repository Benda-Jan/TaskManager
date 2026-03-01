import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import client from './client';
import type { TaskItem, CreateTaskInput, MoveTaskInput } from './types';

export function useProjectTasks(projectId: string) {
  return useQuery<TaskItem[]>({
    queryKey: ['tasks', projectId],
    queryFn: async () => {
      const { data } = await client.get<TaskItem[]>(`/projects/${projectId}/tasks`);
      return data;
    },
    enabled: !!projectId,
  });
}

export function useCreateTask() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (input: CreateTaskInput) => {
      const { data } = await client.post<{ id: string }>('/tasks', input);
      return data.id;
    },
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['tasks', variables.projectId] });
    },
  });
}

export function useMoveTask() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async ({ taskId, statusId }: MoveTaskInput) => {
      await client.patch(`/tasks/${taskId}/status`, { statusId });
    },
    onMutate: async ({ taskId, statusId, projectId }) => {
      await queryClient.cancelQueries({ queryKey: ['tasks', projectId] });
      const previous = queryClient.getQueryData<TaskItem[]>(['tasks', projectId]);
      queryClient.setQueryData<TaskItem[]>(['tasks', projectId], (old) =>
        old?.map((t) => (t.id === taskId ? { ...t, statusId } : t)) ?? []
      );
      return { previous };
    },
    onError: (_, { projectId }, context) => {
      queryClient.setQueryData(['tasks', projectId], context?.previous);
    },
    onSettled: (_, __, { projectId }) => {
      queryClient.invalidateQueries({ queryKey: ['tasks', projectId] });
    },
  });
}
