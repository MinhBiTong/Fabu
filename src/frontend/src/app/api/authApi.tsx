import ApiClient, { globalApiClient } from './ApiClient';

const authClient = new ApiClient('/Auth');     //http://localhost:5000/api/v1/Auth/login

export const LoginApi = {
  login: async (email: string, password: string) => {
    return globalApiClient.post('v1/auth/login', {
      Email: email,
      Password: password
    });
  },

  register: async (email: string, password: string) => {
    return authClient.post<any>('v1/register', {
      Email: email,
      Password: password
    });
  },

  refreshToken: async () => {
    return authClient.post<any>('/refresh-token', {});
  }
};