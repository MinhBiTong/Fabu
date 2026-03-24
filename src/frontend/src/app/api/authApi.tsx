import ApiClient, { globalApiClient } from './ApiClient';

const authClient = new ApiClient('/auth');

export const LoginApi = {
  login: async (email: string, password: string) => {
    //endpoint: baseUrl + /auth + /login
    //globalApiClient se tu dong them sessionId va Authorization header neu co token
    return globalApiClient.post('/auth/login', {email, password});
  },
  register: async (email: string, password: string) => {
    return authClient.post<any>('/register', {email, password});
  },
  refreshToken: async () => {
    return authClient.post<any>('/refresh-token', {});
  }
};