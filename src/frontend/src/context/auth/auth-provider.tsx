"use client";

import { useCallback, useEffect, useMemo, type ReactNode } from "react";
import { AuthContext } from "./auth-context";
import { authService } from "@/services/auth-service";
import { globalApiClient } from "@/lib/api/http-client";
import { useAuthStore } from "@/store/auth.store";

type AuthProviderProps = {
  children: ReactNode;
};

export function AuthProvider({ children }: AuthProviderProps) {
  const {
    accessToken,
    expiresAt,
    profile,
    isAuthenticated,
    isLoading,
    isBootstrapped,
    setSession,
    setAccessToken,
    setLoading,
    markBootstrapped,
    reset,
    hasRole,
    hasPermission,
  } = useAuthStore();

  useEffect(() => {
    globalApiClient.setToken(accessToken);
  }, [accessToken]);

  useEffect(() => {
    let cancelled = false;

    async function bootstrap() {
      setLoading(true);
      try {
        const response = await authService.refreshToken();
        if (!cancelled && response.data?.accessToken) {
          setSession(response.data);
          return;
        }
      } catch {
        if (!cancelled) reset();
        return;
      }

      if (!cancelled) markBootstrapped();
    }

    bootstrap();
    return () => {
      cancelled = true;
    };
  }, [markBootstrapped, reset, setLoading, setSession]);

  const logout = useCallback(async () => {
    try {
      await authService.logout();
    } catch {
      // The client must clear local auth state even if the server session is already gone.
    } finally {
      reset();
    }
  }, [reset]);

  const value = useMemo(
    () => ({
      accessToken,
      expiresAt,
      profile,
      isAuthenticated,
      isLoading,
      isBootstrapped,
      setToken: setAccessToken,
      logout,
      hasRole,
      hasPermission,
    }),
    [
      accessToken,
      expiresAt,
      hasPermission,
      hasRole,
      isAuthenticated,
      isBootstrapped,
      isLoading,
      logout,
      profile,
      setAccessToken,
    ]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
