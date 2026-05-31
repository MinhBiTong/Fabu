import type { LoginResponse, RegisterRequest, RegisterResponse } from "@/core/types/api.types";
import { endpoints } from "@/lib/api/endpoints";
import { globalApiClient } from "@/lib/api/http-client";

export const authService = {
  login(email: string, password: string) {
    return globalApiClient.post<LoginResponse>(endpoints.auth.login, {
      Email: email,
      Password: password,
    });
  },

  register(payload: RegisterRequest) {
    return globalApiClient.post<RegisterResponse>(endpoints.auth.register, payload);
  },

  refreshToken() {
    return globalApiClient.post<LoginResponse>(endpoints.auth.refresh);
  },

  async logout() {
    try {
      return await globalApiClient.post<null>(endpoints.auth.logout);
    } finally {
      globalApiClient.setToken(null);
    }
  },

  getExternalLoginUrl(provider: "google" | "github") {
    const base =
      process.env.NEXT_PUBLIC_API_BASE_URL ||
      process.env.NEXT_PUBLIC_API_URL ||
      "http://localhost:5000/api";
    const root = /\/api(\/v\d+)?$/i.test(base.replace(/\/+$/, ""))
      ? base.replace(/\/+$/, "")
      : `${base.replace(/\/+$/, "")}/api`;

    return `${root}/${provider === "google" ? endpoints.auth.google : endpoints.auth.github}`;
  },
};
