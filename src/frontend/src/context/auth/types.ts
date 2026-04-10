export type AuthState = {
    AccessToken: string | null;
    isLoading: boolean;
}

export type AuthAction = 
| { type: "SET_ACCESS_TOKEN", payload: string | null }
| { type: "SET_LOADING"; payload: boolean }
| { type: "LOGOUT" };