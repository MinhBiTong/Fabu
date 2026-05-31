import type { AuthAction, AuthState } from "./types";

export const initialAuthState: AuthState = {
  accessToken: null,
  expiresAt: null,
  profile: {
    roles: [],
    permissions: [],
  },
  isAuthenticated: false,
  isLoading: true,
  isBootstrapped: false,
};

export const authReducer = (state: AuthState, action: AuthAction): AuthState => {
  switch (action.type) {
    case "SET_ACCESS_TOKEN":
      return {
        ...state,
        accessToken: action.payload,
        isAuthenticated: Boolean(action.payload),
      };
    case "SET_LOADING":
      return {
        ...state,
        isLoading: action.payload,
      };
    case "LOGOUT":
      return initialAuthState;
    default:
      return state;
  }
};
