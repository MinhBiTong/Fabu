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
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50 backdrop-blur-sm p-4">
      {/* Overlay để click ra ngoài thì đóng */}
      <div className="absolute inset-0" onClick={onClose}></div>

      {/* Card Form */}
      <div 
        className="relative w-full max-w-md transform overflow-hidden rounded-2xl bg-white p-8 shadow-2xl transition-all"
        onClick={(e) => e.stopPropagation()}
      >
        {/* Nút đóng */}
        <button 
          onClick={onClose}
          className="absolute right-4 top-4 text-gray-400 hover:text-gray-600 transition-colors"
        >
          <span className="text-2xl">✕</span>
        </button>

        <div className="mb-8 text-center">
          <h1 className="text-3xl font-bold text-gray-800">Welcome Back</h1>
          <p className="mt-2 text-sm text-gray-500">Please enter your details to sign in</p>
        </div>

        <form onSubmit={handleSubmit(onLoginSubmit)} className="space-y-5">
          {/* Email Field */}
          <div>
            <label className="block text-sm font-medium text-gray-700">Email Address</label>
            <input
              type="email"
              {...register("email")}
              className={`mt-1 block w-full rounded-lg border px-4 py-3 text-gray-900 focus:outline-none focus:ring-2 transition-all ${
                errors.email 
                  ? "border-red-500 focus:ring-red-200" 
                  : "border-gray-300 focus:border-blue-500 focus:ring-blue-100"
              }`}
              placeholder="name@company.com"
            />
            {errors.email && (
              <p className="mt-1 text-xs text-red-500">{errors.email.message}</p>
            )}
          </div>

          {/* Password Field */}
          <div>
            <div className="flex items-center justify-between">
              <label className="block text-sm font-medium text-gray-700">Password</label>
              <a href="#" className="text-xs text-blue-600 hover:underline">Forgot password?</a>
            </div>
            <input
              type="password"
              {...register("password")}
              className={`mt-1 block w-full rounded-lg border px-4 py-3 text-gray-900 focus:outline-none focus:ring-2 transition-all ${
                errors.password 
                  ? "border-red-500 focus:ring-red-200" 
                  : "border-gray-300 focus:border-blue-500 focus:ring-blue-100"
              }`}
              placeholder="••••••••"
            />
            {errors.password && (
              <p className="mt-1 text-xs text-red-500">{errors.password.message}</p>
            )}
          </div>

          {/* Login Button */}
          <button
            type="submit"
            disabled={isSubmitting}
            className="w-full rounded-lg bg-blue-600 py-3 font-semibold text-white shadow-lg hover:bg-blue-700 focus:outline-none focus:ring-4 focus:ring-blue-300 disabled:opacity-70 transition-all active:scale-[0.98]"
          >
            {isSubmitting ? (
              <span className="flex items-center justify-center gap-2">
                <svg className="h-5 w-5 animate-spin text-white" viewBox="0 0 24 24">
                  <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" fill="none" />
                  <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z" />
                </svg>
                Logging in...
              </span>
            ) : "Sign In"}
          </button>
        </form>

        {/* Divider */}
        <div className="my-6 flex items-center before:flex-1 before:border-t before:border-gray-300 after:flex-1 after:border-t after:border-gray-300">
          <p className="mx-4 text-center text-sm font-semibold text-gray-500 uppercase tracking-wider">Or</p>
        </div>

        {/* Social & Signup Actions */}
        <div className="space-y-3">
          <button
            onClick={handleGoogleLogin}
            className="flex w-full items-center justify-center gap-3 rounded-lg border border-gray-300 bg-white py-2.5 text-gray-700 hover:bg-gray-50 transition-colors shadow-sm"
          >
            <img src="https://www.svgrepo.com/show/355037/google.svg" className="h-5 w-5" alt="Google" />
            <span>Continue with Google</span>
          </button>

          <div className="text-center text-sm text-gray-600">
            Dont have an account?{" "}
            <button 
              onClick={onSwitchToSignup}
              className="font-bold text-blue-600 hover:text-blue-700 transition-colors"
            >
              Sign up here
            </button>
          </div>
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