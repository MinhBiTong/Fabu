import ApiClient, { globalApiClient } from './ApiClient';

const authClient = new ApiClient('/Auth');

export const LoginApi = {
  login: async (email: string, password: string) => {
    return globalApiClient.post('/auth/login', {
      Email: email,
      Password: password
    });
  },

  register: async (email: string, password: string) => {
    return authClient.post<any>('/register', {
      Email: email,
      Password: password
    });
  },

  refreshToken: async () => {
    return authClient.post<any>('/refresh-token', {});
  }
};