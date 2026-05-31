"use client";

import { useCallback } from "react";
import { useRouter } from "next/navigation";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { toastError, toastSuccess } from "@/services/toast-service";
import { authService } from "@/services/auth-service";
import { useAuthStore } from "@/store/auth.store";
import { loginSchema, type LoginFormData } from "@/core/validations/login.schema";

type UseLoginOptions = {
  onSuccess?: () => void;
};

export const useLogin = (options?: UseLoginOptions) => {
  const router = useRouter();
  const setSession = useAuthStore((state) => state.setSession);

  const form = useForm<LoginFormData>({
    resolver: zodResolver(loginSchema),
    defaultValues: { email: "", password: "" },
  });

  const onLoginSubmit = useCallback(
    async (data: LoginFormData) => {
      try {
        const result = await authService.login(data.email, data.password);
        if (result.code === 200 && result.data?.accessToken) {
          setSession(result.data);
          toastSuccess(result.message || "Login successfully");
          options?.onSuccess?.();
          router.push("/");
          return { success: true };
        }

        toastError(result.message || "Login failed. Please try again.");
        return { success: false };
      } catch (error) {
        const message = error instanceof Error ? error.message : "Login failed.";
        toastError(message);
        return { success: false };
      }
    },
    [options, router, setSession]
  );

  const handleGoogleLogin = useCallback(() => {
    window.location.href = authService.getExternalLoginUrl("google");
  }, []);

  return {
    ...form,
    onLoginSubmit,
    handleGoogleLogin,
  };
};
