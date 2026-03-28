import { useEffect, useReducer } from "react";
import { AuthContext } from "./AuthContext";
import { globalApiClient } from "../../api/ApiClient";
import { authReducer, initialAuthState } from "./reducer";

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
        globalApiClient.setToken(state.accessToken);
    }, [state.accessToken]);

    useEffect(() => {
        //1, khi load trang, goi api /refresh-token len .NET
        //2, neu success -> cookie hop le, setAccessToken (token moi)
        //3, neu fail -> cookie khong hop le, setAccessToken(null)
        //4, set isloading(false)

        //logic refresh token, silent refresh khi load app
        const silentRefresh = async () => {
            try {
                //api goi refresh token
                const response = await fetch('/auth/refresh-token', {
                    method: 'POST',
                    credentials: 'include', //gui cookie len server
                });

                if (!response.ok) throw new Error('Failed to refresh token');

                const data = await response.json();
                dispatch({ type: 'SET_ACCESS_TOKEN', payload: data.accessToken });
            } catch {
                dispatch({ type: 'SET_ACCESS_TOKEN', payload: null }    );
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