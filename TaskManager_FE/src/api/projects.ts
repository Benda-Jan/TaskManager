import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import client from './client';
import type { Project, ProjectDetail } from './types';

const PROJECTS_KEY = ['projects'];

export function useMyProjects() {
  return useQuery<Project[]>({
    queryKey: PROJECTS_KEY,
    queryFn: async () => {
      const { data } = await client.get<Project[]>('/projects');
      return data;
    },
  });
}

export function useProject(id: string) {
  return useQuery<ProjectDetail>({
    queryKey: ['projects', id],
    queryFn: async () => {
      const { data } = await client.get<ProjectDetail>(`/projects/${id}`);
      return data;
    },
    enabled: !!id,
  });
}

export interface CreateProjectInput {
  name: string;
  description?: string;
  budget: number;
}

export function useCreateProject() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (input: CreateProjectInput) => {
      const { data } = await client.post<{ id: string }>('/projects', input);
      return data.id;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: PROJECTS_KEY });
    },
  });
}
