import type { AuthProfile } from "@/store/auth.store";

export type AuthState = {
  accessToken: string | null;
  expiresAt: string | null;
  profile: AuthProfile;
  isAuthenticated: boolean;
  isLoading: boolean;
  isBootstrapped: boolean;
};

export type AuthContextValue = AuthState & {
  setToken: (token: string | null) => void;
  logout: () => Promise<void>;
  hasRole: (role: string) => boolean;
  hasPermission: (permission: string) => boolean;
};

export type AuthAction =
  | { type: "SET_ACCESS_TOKEN"; payload: string | null }
  | { type: "SET_LOADING"; payload: boolean }
  | { type: "LOGOUT" };
