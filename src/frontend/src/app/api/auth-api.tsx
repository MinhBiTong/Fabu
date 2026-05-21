import ApiClient, { globalApiClient } from './api-client';

const authClient = new ApiClient('v1/Auth');     //http://localhost:5000/api/v1/Auth/login

export const LoginApi = {
  login: async (email: string, password: string) => {
    return globalApiClient.post('v1/Auth/login', {
      Email: email,
      Password: password
    });
  },

  register: async (email: string, password: string, username: string, confirmPassword: string) => {     //http://localhost:5000/api/v1/Auth/register
    return authClient.post<any>('/register', {
      Email: email,
      Password: password,
      ConfirmPassword: confirmPassword,
      Username: username
    });
  },

  refreshToken: async () =>
    await globalApiClient.post<any>('v1/Auth/refresh-token', {}, {
      credential: 'include'
    }
  ),

  logout: async () => {
    try {
      await globalApiClient.post<void>('v1/Auth/logout');
      //refresh(); // Clear cache and refresh the page
    } catch (error) {
      console.error('Logout failed:', error);
    }
  },
};