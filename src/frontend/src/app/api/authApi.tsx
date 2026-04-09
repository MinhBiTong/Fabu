import ApiClient, { globalApiClient } from './ApiClient';

const authClient = new ApiClient('v1/Auth');     //http://localhost:5000/api/v1/Auth/login

export const LoginApi = {
  login: async (email: string, password: string) => {
    return globalApiClient.post('v1/Auth/login', {
      Email: email,
      Password: password
    });
  },

  register: async (email: string, password: string) => {     //http://localhost:5000/api/v1/Auth/register
    return authClient.post<any>('/register', {
      Email: email,
      Password: password
    });
  },

  refreshToken: async () => {
    // return authClient.post<any>('/refresh-token', {});
    //check httpOnly cookie tu server gui len, neu co thi goi api refresh token
    return globalApiClient.post<any>('v1/Auth/refresh-token', {});
  }
};