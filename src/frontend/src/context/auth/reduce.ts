import type { AuthAction, AuthState } from "./types";

export const initialAuthState: AuthState = {
    accessToken: null,
    isLoading: false
}

export const authReducer = (
    state: AuthState,
    action: AuthAction
): AuthState => {
    switch (action.type) {
        case "SET_ACCESS_TOKEN":
            return {
                ...state,
                accessToken: action.payload
            }

        case "SET_LOADING":
            return {
                ...state,
                isLoading:action.payload
            }
        case "LOGOUT":
            return {
                accessToken: null,
                isLoading: false
            }

        default:
            return state;
    }
}