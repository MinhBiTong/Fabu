"use client";
import { useRouter } from "next/navigation";
import { useState } from "react";
import Image from "next/image";

import logo from "../../styles/images/FABUlogo.png";
import Icon from "../../styles/images/search.png";
import Menu from "../../styles/images/menu.png";
import User from "../../styles/images/user.png";
import { useLogin } from "@/hooks/use-login";

interface LoginFormProps {
  onClose: () => void;
  onSwitchToSignup: () => void;
}

const LoginForm: React.FC<LoginFormProps> = ({ onClose, onSwitchToSignup }) => {
  const {
    register,
    handleSubmit,
    errors,
    onLoginSubmit,
    isSubmitting, // Lấy thêm trạng thái này từ hook
    handleGoogleLogin, // Tên mới thay cho handleClick để rõ nghĩa
  } = useLogin();

  return (
   <div className="FullScreen">
  {/* Overlay */}
  <div className="loginOverlay" onClick={onClose}>

    {/* Form Card */}
    <div className="loginform" onClick={(e) => e.stopPropagation()}>

      <button className="closeBtn" onClick={onClose}>✕</button>

      <h1>Sign In</h1>

      <form onSubmit={handleSubmit(onLoginSubmit)} className="Mainform">
        
        {/* Email */}
        <input
          type="email"
          placeholder="Email"
          {...register("email")}
        />
        {errors.email && <p className="error">{errors.email.message}</p>}

        {/* Password */}
        <input
          type="password"
          placeholder="Password"
          {...register("password")}
        />
        {errors.password && <p className="error">{errors.password.message}</p>}

        <p>Forgot Password</p>

        {/* Submit */}
        <button className="loginSubmit" type="submit" disabled={isSubmitting}>
          {isSubmitting ? "Logging in..." : "Login"}
        </button>

        {/* Signup */}
        <div className="Signuplink">
          <span>No accounts?</span>
          <p onClick={onSwitchToSignup}>Sign up here</p>
        </div>

      </form>
    </div>
  </div>
</div>
  );
};

export default LoginForm;
