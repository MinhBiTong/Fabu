"use client";

import { useRouter } from "next/navigation";
import { useState } from "react";
import { loginSchema } from "../../../core/validations/LoginSchema";

import { LoginApi } from "../../../app/api/authApi";
import { globalApiClient } from "../../../app/api/ApiClient";
import Image from "next/image";

import logo from "../../styles/images/FABUlogo.png";
import Icon from "../../styles/images/search.png";
import Menu from "../../styles/images/menu.png";
import User from "../../styles/images/user.png";


type Props = {
  onClose: () => void;
  onSwitchToSignup: () => void;
};

function LoginForm({ onClose, onSwitchToSignup}: Props) {
const [email, setEmail] = useState("");
const [password, setPassword] = useState("");
const router = useRouter();
const [errors, setErrors] = useState<{ email?: string; password?: string }>({});


const handleLogin = async () => {
  const result = loginSchema.safeParse({ email, password });

  if (!result.success) {
    const fieldErrors: { email?: string; password?: string } = {};

    result.error.issues.forEach((err) => {
      if (err.path[0] === "email") fieldErrors.email = err.message;
      if (err.path[0] === "password") fieldErrors.password = err.message;
    });

    setErrors(fieldErrors);
    return;
  }

  setErrors({});

  try {
    const res = await LoginApi.login(email, password);

    const token = res.Result.AccessToken;

    localStorage.setItem("accessToken", token);
    globalApiClient.setToken(token);

    onClose();
    router.refresh();
  } catch (err) {
    console.error("Login failed:", err);
  }
};
  return (
    <div className= "FullScreen" >
    <div className="loginOverlay"onClick={onClose} >

      <div className="loginform"    onClick={(e) => e.stopPropagation()}>

        <button className="closeBtn"onClick={onClose}>✕</button>

        <h1>Sign In</h1>

       <div className="Mainform">
        <input type="email" placeholder="Email"   value={email}
  onChange={(e) => setEmail(e.target.value)}/>
  {errors.email && <p className="error">{errors.email}</p>}
        <input type="password" placeholder="Password" value={password}
  onChange={(e) => setPassword(e.target.value)} />
  {errors.password && <p className="error">{errors.password}</p>}
                   <p>Forgot Password </p>
       <button className="loginSubmit" onClick={handleLogin}>
                    Login
           </button>
           <div className="Signuplink">
        <span> No accounts ?</span> <p onClick={onSwitchToSignup}>Sign up here</p>
          </div>
           </div>

           
      </div>

    </div>
    </div>
  );
}
export default LoginForm;