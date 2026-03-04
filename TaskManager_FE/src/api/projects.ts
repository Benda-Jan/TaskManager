import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import client from './client';
import type { Member, Project, ProjectDetail } from './types';

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

export interface UpdateProjectInput {
  name: string;
  description?: string;
  budget: number;
}

export function useUpdateProject(id: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (input: UpdateProjectInput) => {
      await client.put(`/projects/${id}`, input);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['projects', id] });
      queryClient.invalidateQueries({ queryKey: PROJECTS_KEY });
    },
  });
}

export interface StatusInput {
  name: string;
  color: string;
}

export function useCreateStatus(projectId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (input: StatusInput) => {
      const { data } = await client.post<{ id: string }>(`/projects/${projectId}/statuses`, input);
      return data.id;
    },
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['projects', projectId] }),
  });
}

export function useUpdateStatus(projectId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async ({ statusId, ...input }: StatusInput & { statusId: string }) => {
      await client.put(`/projects/${projectId}/statuses/${statusId}`, input);
    },
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['projects', projectId] }),
  });
}

export function useDeleteStatus(projectId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (statusId: string) => {
      await client.delete(`/projects/${projectId}/statuses/${statusId}`);
    },
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['projects', projectId] }),
  });
}

export function useProjectMembers(projectId: string) {
  return useQuery<Member[]>({
    queryKey: ['projects', projectId, 'members'],
    queryFn: async () => {
      const { data } = await client.get<Member[]>(`/projects/${projectId}/members`);
      return data;
    },
    enabled: !!projectId,
  });
}

export function useInviteMember(projectId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (email: string) => {
      await client.post(`/projects/${projectId}/invitations`, { email });
    },
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['projects', projectId, 'members'] }),
  });
}

export function useRemoveMember(projectId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (userId: string) => {
      await client.delete(`/projects/${projectId}/members/${userId}`);
    },
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['projects', projectId, 'members'] }),
  });
}
