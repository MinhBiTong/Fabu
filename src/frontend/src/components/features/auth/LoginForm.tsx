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
// type Props = {
//   onClose: () => void;
//   onSwitchToSignup: () => void;
// };


// function LoginForm({ onClose, onSwitchToSignup}: Props) {
// const [email, setEmail] = useState("");
// const [password, setPassword] = useState("");
// const router = useRouter();
// const [errors, setErrors] = useState<{ email?: string; password?: string }>({});


// // const handleLogin = async () => {
// //   const result = loginSchema.safeParse({ email, password });

// //   if (!result.success) {
// //     const fieldErrors: { email?: string; password?: string } = {};

// //     result.error.issues.forEach((err) => {
// //       if (err.path[0] === "email") fieldErrors.email = err.message;
// //       if (err.path[0] === "password") fieldErrors.password = err.message;
// //     });

// //     setErrors(fieldErrors);
// //     return;
// //   }

// //   setErrors({});

// //   try {
// //     const res = await LoginApi.login(email, password);

// //     // const token = res.Data.AccessToken;

// //     // localStorage.setItem("accessToken", token);
// //     // globalApiClient.setToken(token);

// //     const { setAccessToken } = res.Data.AccessToken;
// //     const token = res.Data.AccessToken;
// //     setAccessToken(token)

// //     onClose();
// //     router.push("/dashboard")
// //   } catch (err) {
// //     console.error("Login failed:", err);
// //   }
// // }
// const handleLogin = async () => {
//   const {
//     register,
//     handleSubmit,
//     errors,
//     onLoginSubmit,
//     handleClick
//   } = useLogin();
// };
//   return (
//     <div className= "FullScreen" >
//     <div className="loginOverlay"onClick={onClose} >

//       <div className="loginform"    onClick={(e) => e.stopPropagation()}>

//         <button className="closeBtn"onClick={onClose}>✕</button>

//         <h1>Sign In</h1>

//        <div className="Mainform">
//         <input type="email" placeholder="Email"   value={email}
//   onChange={(e) => setEmail(e.target.value)}/>
//   {errors.email && <p className="error">{errors.email}</p>}
//         <input type="password" placeholder="Password" value={password}
//   onChange={(e) => setPassword(e.target.value)} />
//   {errors.password && <p className="error">{errors.password}</p>}
//                    <p>Forgot Password </p>
//        <button className="loginSubmit" onClick={handleLogin}>
//                     Login
//            </button>
//            <div className="Signuplink">
//         <span> No accounts ?</span> <p onClick={onSwitchToSignup}>Sign up here</p>
//           </div>
//            </div>

           
//       </div>

//     </div>
//     </div>
//   );
// }
// export default LoginForm;
