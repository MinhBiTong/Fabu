import { useEffect, useReducer } from "react";
import { AuthContext } from "./AuthContext";
import { globalApiClient } from "../../app/api/ApiClient";
import { authReducer, initialAuthState } from "../auth/reduce";
import { LoginApi } from "@/app/api/authApi";

//logic refresh token + apiClient
type AuthProviderProps = {
    children: React.ReactNode;
}
export const AuthProvider = ({ children }: AuthProviderProps) => {
    const [state, dispatch] = useReducer(
        authReducer,
        initialAuthState
    );

    //moi khi accessToken thay doi, cap nhat lai globalApiClient (apiclient)
    useEffect(() => {
        globalApiClient.setToken(state.AccessToken);
    }, [state.AccessToken]);

    useEffect(() => {
        //1, khi load trang, goi api /refresh-token len .NET
        //2, neu success -> cookie hop le, setAccessToken (token moi)
        //3, neu fail -> cookie khong hop le, setAccessToken(null)
        //4, set isloading(false)

        //logic refresh token, silent refresh khi load app
        const silentRefresh = async () => {
            try {
                //api goi refresh token
                const response = await LoginApi.refreshToken();
                const newToken = response.Data.AccessToken;
                // const response = await fetch('/api/v1/Auth/refresh-token', {
                //     method: 'POST',
                //     credentials: 'include', //gui cookie len server
                // });

                // const data = await response.json();
                const data = newToken ? { accessToken: newToken } : null;

                dispatch({ type: 'SET_ACCESS_TOKEN', payload: data?.accessToken || null });
            } catch {
                dispatch({ type: 'SET_ACCESS_TOKEN', payload: null });
            } finally {
                dispatch({ type: "SET_LOADING", payload: false });
            }
        };
        silentRefresh();
    }, []);

    return (
        <AuthContext.Provider value={{ state, dispatch }}>
            {children}
        </AuthContext.Provider>
    )
};