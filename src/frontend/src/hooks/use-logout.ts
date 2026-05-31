"use client";

import { useCallback } from "react";
import { useAuth } from "./use-auth";

export function useLogout() {
  const { logout } = useAuth();
  return useCallback(() => logout(), [logout]);
}
