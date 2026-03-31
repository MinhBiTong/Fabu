import { useContext } from "react";
import { AuthContext } from "../context/auth/AuthContext";
export const useAuth = () => {
    const context = useContext(AuthContext);

    if (!context) {
        throw new Error("useAuth must be used within AuthProvider");
    }

    const {state, dispatch} = context;

    return {
        accessToken: state.accessToken,
        isLoading: state.isLoading,
        setToken: (token: string | null) =>
            dispatch({ type: "SET_ACCESS_TOKEN", payload: token }),
        logout: () => dispatch({ type: "LOGOUT" }),
    };
};
