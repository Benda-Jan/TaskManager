import axios from 'axios';
import keycloak from '../auth/keycloak';

const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

apiClient.interceptors.request.use(async (config) => {
  if (keycloak.isTokenExpired(30)) {
    await keycloak.updateToken(30);
  }
  if (keycloak.token) {
    config.headers.Authorization = `Bearer ${keycloak.token}`;
  }
  return config;
});

export default apiClient;
