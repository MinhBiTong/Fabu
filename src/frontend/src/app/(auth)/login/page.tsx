"use client";

import Logo from "../../../styles/images/FABUlogo.png";
import Image from 'next/image';
import Icon from "../../../styles/images/gmail.png";
import Menu from "../../../styles/images/menu.png";
import User from "../../../styles/images/user.png";
import { useLogin } from "@/hooks/use-login";

interface LoginFormProps {
  onClose: () => void;
  onSwitchToSignup: () => void;
}

const LoginForm: React.FC<LoginFormProps> = ({ onClose, onSwitchToSignup }) => {
  // const {
  //   register,
  //   handleSubmit,
  //   errors,
  //   onLoginSubmit,
  //   isSubmitting, // Lấy thêm trạng thái này từ hook
  //   handleGoogleLogin, // Tên mới thay cho handleClick để rõ nghĩa
  // } = useLogin();
  const form = useLogin();

  return (
   <div className="FullScreen">
  {/* Overlay */}
  <div className="loginOverlay" onClick={onClose}>
    console.log(param);
    

    {/* Form Card */}
    <div className="loginform" onClick={(e) => e.stopPropagation()}>

      <button className="closeBtn" onClick={onClose}>✕</button>

      <h1>Sign In</h1>

      <form onSubmit={form.handleSubmit(form.onLoginSubmit)} className="Mainform">
        
        {/* Email */}
        <input
          type="email"
          placeholder="Email"
          {...form.register("email")}
        />
        {form.formState.errors.email && <p className="error">{form.formState.errors.email.message}</p>}

        {/* Password */}
        <input
          type="password"
          placeholder="Password"
          {...form.register("password")}
        />
        {form.formState.errors.password && <p className="error">{form.formState.errors.password.message}</p>}

        <p>Forgot Password</p>

        {/* Submit */}
        <button className="loginSubmit" type="submit" disabled={form.formState.isSubmitting}>
          {form.formState.isSubmitting ? "Logging in..." : "Login"}
        </button>

        {/* Divider */}
        <div className="divider">
          <span>or</span>
        </div>

        {/* Facebook */}
        <div className="facebookLoginContainer">
          <button className="facebookLoginBtn" type="button">
            <Image src={User} alt="Facebook Icon" width={20} height={20} />
            Sign in with Facebook
          </button>
        </div>

        {/* Google */}
        <div className="googleLoginContainer">
          <button className="googleLoginBtn" type="button" onClick={form.handleGoogleLogin}>
            <Image src={Icon} alt="Google Icon" width={20} height={20} />
            Sign in with Google
          </button>
        </div>

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

