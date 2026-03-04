import client from './client';

export function parseJwtSub(token: string): string | null {
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    return payload.sub ?? null;
  } catch {
    return null;
  }
}

interface AuthResponse {
  accessToken: string;
  refreshToken: string;
}

export async function register(email: string, name: string, password: string, invitationToken?: string): Promise<AuthResponse> {
  const { data } = await client.post<AuthResponse>('/auth/register', { email, name, password, invitationToken });
  return data;
}

export async function login(email: string, password: string, invitationToken?: string): Promise<AuthResponse> {
  const { data } = await client.post<AuthResponse>('/auth/login', { email, password, invitationToken });
  return data;
}

export async function refresh(token: string): Promise<AuthResponse> {
  const { data } = await client.post<AuthResponse>('/auth/refresh', { token });
  return data;
}
