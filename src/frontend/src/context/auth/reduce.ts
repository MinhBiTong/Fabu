import type { AuthAction, AuthState } from "./types";

export const initialAuthState: AuthState = {
    AccessToken: null,
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
                AccessToken: action.payload
            }

        case "SET_LOADING":
            return {
                ...state,
                isLoading:action.payload
            }
        case "LOGOUT":
            return {
                AccessToken: null,
                isLoading: false
            }

        default:
            return state;   
    }
}