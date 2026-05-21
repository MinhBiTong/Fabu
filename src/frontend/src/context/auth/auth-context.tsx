import React, { createContext} from "react";
import type { AuthAction, AuthState } from "./types";

//expose state + dispatch + helper
//day la noi duy nhat set token
type AuthContextType = {
    state: AuthState;
    dispatch: React.Dispatch<AuthAction>;
};

export const AuthContext = createContext<AuthContextType | undefined>(undefined);


 

