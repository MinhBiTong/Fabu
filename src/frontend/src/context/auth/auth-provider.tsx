import { useEffect, useReducer } from "react";
import { AuthContext } from "./auth-context";
import { globalApiClient } from "../../app/api/api-client";
import { authReducer, initialAuthState } from "../auth/reduce";
import { LoginApi } from "@/app/api/auth-api";
import { useRouter } from "next/navigation";
//logic refresh token + apiClient
type AuthProviderProps = {
    children: React.ReactNode;
}
export const AuthProvider = ({ children }: AuthProviderProps) => {
    const router = useRouter();
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
                //api goi refresh token, truyen refresh token tu cookie len server, neu hop le se tra ve access token moi
                const response = await LoginApi.refreshToken();
                //xu ly ket qua tra ve, neu co access token moi thi cap nhat vao state, neu khong thi set null
                if (response?.code === 200 && response?.data?.accessToken) {
                    dispatch({ type: 'SET_ACCESS_TOKEN', payload: response.data.accessToken });
                } else {
                    dispatch({ type: 'SET_ACCESS_TOKEN', payload: null });
                }
            } catch {
                console.log("Can't auto create new token");
                dispatch({ type: 'SET_ACCESS_TOKEN', payload: null });
            } finally {
                dispatch({ type: "SET_LOADING", payload: false });
            }
        };

        const silentLogout = async () => {
            try {
                await LoginApi.logout();
                dispatch({ type: 'LOGOUT' });
                //redirect ve trang login
                router.push("/login");
            } catch (error) {
                console.error('Logout failed:', error);
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