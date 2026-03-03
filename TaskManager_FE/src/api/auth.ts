import client from './client';

interface AuthResponse {
  accessToken: string;
  refreshToken: string;
}

export async function register(email: string, name: string, password: string): Promise<AuthResponse> {
  const { data } = await client.post<AuthResponse>('/auth/register', { email, name, password });
  return data;
}

export async function login(email: string, password: string): Promise<AuthResponse> {
  const { data } = await client.post<AuthResponse>('/auth/login', { email, password });
  return data;
}

export async function refresh(token: string): Promise<AuthResponse> {
  const { data } = await client.post<AuthResponse>('/auth/refresh', { token });
  return data;
}
