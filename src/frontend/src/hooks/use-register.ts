"use client";

import { useCallback } from "react";
import { useRouter } from "next/navigation";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { toastError, toastSuccess } from "@/services/toast-service";
import { authService } from "@/services/auth-service";
import { signupSchema, type SignupFormData } from "@/core/validations/signup.schema";

type UseRegisterOptions = {
  onSuccess?: () => void;
};

export const useRegister = (options?: UseRegisterOptions) => {
  const router = useRouter();

  const form = useForm<SignupFormData>({
    resolver: zodResolver(signupSchema),
    defaultValues: {
      username: "",
      fullName: "",
      email: "",
      phoneNumber: "",
      password: "",
      confirmPassword: "",
    },
  });

  const onRegisterSubmit = useCallback(
    async (data: SignupFormData) => {
      try {
        const response = await authService.register({
          Username: data.username,
          FullName: data.fullName,
          PhoneNumber: data.phoneNumber,
          Email: data.email,
          Password: data.password,
        });

        if (response.code === 200 || response.code === 201) {
          toastSuccess(response.data?.message || "Register successfully");
          options?.onSuccess?.();
          router.push("/login");
          return { success: true };
        }

        toastError(response.message || "Register failed.");
        return { success: false };
      } catch (error) {
        const message = error instanceof Error ? error.message : "Register failed.";
        toastError(message);
        return { success: false };
      }
    },
    [options, router]
  );

  return {
    ...form,
    onRegisterSubmit,
  };
};
