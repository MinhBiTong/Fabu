import { authService } from "@/services/auth-service";

export const LoginApi = {
  login: authService.login,
  register: (
    email: string,
    password: string,
    username: string,
    _confirmPassword: string,
    fullName = username,
    phoneNumber = ""
  ) =>
    authService.register({
      Email: email,
      Password: password,
      Username: username,
      FullName: fullName,
      PhoneNumber: phoneNumber,
    }),
  refreshToken: authService.refreshToken,
  logout: authService.logout,
};
