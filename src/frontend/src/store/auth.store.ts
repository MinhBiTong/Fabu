import { create } from "zustand";
import type { ClaimDto, LoginResponse } from "@/core/types/api.types";

export type AuthProfile = {
  id?: string;
  email?: string;
  username?: string;
  roles: string[];
  permissions: string[];
};

type AuthState = {
  accessToken: string | null;
  expiresAt: string | null;
  claims: ClaimDto[];
  profile: AuthProfile;
  isAuthenticated: boolean;
  isLoading: boolean;
  isBootstrapped: boolean;
};

type AuthActions = {
  setSession: (session: LoginResponse | null) => void;
  setAccessToken: (token: string | null) => void;
  setLoading: (isLoading: boolean) => void;
  markBootstrapped: () => void;
  reset: () => void;
  hasRole: (role: string) => boolean;
  hasPermission: (permission: string) => boolean;
};

const emptyProfile: AuthProfile = {
  roles: [],
  permissions: [],
};

function claimValue(claims: ClaimDto[], ...aliases: string[]) {
  const lowerAliases = aliases.map((alias) => alias.toLowerCase());
  return claims.find((claim) => {
    const type = claim.type.toLowerCase();
    return lowerAliases.some((alias) => type === alias || type.endsWith(`/${alias}`));
  })?.value;
}

function claimValues(claims: ClaimDto[], alias: string) {
  const lowerAlias = alias.toLowerCase();
  return claims
    .filter((claim) => {
      const type = claim.type.toLowerCase();
      return type === lowerAlias || type.endsWith(`/${lowerAlias}`);
    })
    .map((claim) => claim.value);
}

function toProfile(claims: ClaimDto[]): AuthProfile {
  return {
    id: claimValue(claims, "nameidentifier", "sub"),
    email: claimValue(claims, "emailaddress", "email"),
    username: claimValue(claims, "name", "unique_name"),
    roles: claimValues(claims, "role"),
    permissions: claimValues(claims, "permission"),
  };
}

const initialState: AuthState = {
  accessToken: null,
  expiresAt: null,
  claims: [],
  profile: emptyProfile,
  isAuthenticated: false,
  isLoading: true,
  isBootstrapped: false,
};

export const useAuthStore = create<AuthState & AuthActions>((set, get) => ({
  ...initialState,

  setSession: (session) =>
    set({
      accessToken: session?.accessToken ?? null,
      expiresAt: session?.expiresAt ?? null,
      claims: session?.claims ?? [],
      profile: session?.claims ? toProfile(session.claims) : emptyProfile,
      isAuthenticated: Boolean(session?.accessToken),
      isLoading: false,
      isBootstrapped: true,
    }),

  setAccessToken: (token) =>
    set((state) => ({
      accessToken: token,
      isAuthenticated: Boolean(token),
      profile: token ? state.profile : emptyProfile,
    })),

  setLoading: (isLoading) => set({ isLoading }),
  markBootstrapped: () => set({ isBootstrapped: true, isLoading: false }),
  reset: () => set({ ...initialState, isLoading: false, isBootstrapped: true }),
  hasRole: (role) =>
    get().profile.roles.some((value) => value.toLowerCase() === role.toLowerCase()),
  hasPermission: (permission) =>
    get().profile.permissions.some(
      (value) => value.toLowerCase() === permission.toLowerCase()
    ),
}));
