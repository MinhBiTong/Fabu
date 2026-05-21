"use client";
import Image from "next/image";

import test from "../../../styles/images/blueytitlebackground.png";

import { useState } from "react";
import { signupSchema } from "../../../core/validations/signup.schema";
import { globalApiClient } from "@/app/api/api-client";
import { useRegister } from "@/hooks/use-register";


interface RegisterFormProps {
  onClose: () => void;
  onSwitchToLogin: () => void;
}

export const RegisterForm: React.FC<RegisterFormProps> = ({ onClose, onSwitchToLogin }) => {
  const { register, handleSubmit, errors, isSubmitting, onRegisterSubmit } = useRegister(onClose);

  return (
    <form onSubmit={handleSubmit(onRegisterSubmit)} className="space-y-4">
      <h2 className="text-2xl font-bold">Đăng ký</h2>

      <div>
        <input 
           {...register("username")} 
           placeholder="Họ và tên"
           className="w-full p-2 border rounded"
        />
        {errors.username && <p className="text-red-500 text-sm">{errors.username.message}</p>}
      </div>

      <div>
        <input 
           {...register("email")} 
           placeholder="Email"
           className="w-full p-2 border rounded"
        />
        {errors.email && <p className="text-red-500 text-sm">{errors.email.message}</p>}
      </div>

      <div>
        <input 
           {...register("password")} 
           type="password" 
           placeholder="Mật khẩu"
           className="w-full p-2 border rounded"
        />
        {errors.password && <p className="text-red-500 text-sm">{errors.password.message}</p>}
      </div>

      <div>
        <input 
           {...register("confirmPassword")} 
           type="password" 
           placeholder="Xác nhận mật hàng"
           className="w-full p-2 border rounded"
        />
        {errors.confirmPassword && <p className="text-red-500 text-sm">{errors.confirmPassword.message}</p>}
      </div>

      <button 
        type="submit" 
        disabled={isSubmitting}
        className="w-full bg-blue-600 text-white p-2 rounded disabled:bg-gray-400"
      >
        {isSubmitting ? "Đang xử lý..." : "Đăng ký ngay"}
      </button>

      <div className="relative">
        <div className="absolute inset-0 flex items-center">
          <div className="w-full border-t border-gray-300"></div>
        </div>
        <div className="relative flex justify-center text-sm">
          <span className="bg-white px-2 text-gray-500">or</span>
        </div>
      </div>

      <button 
        type="button" 
        className="w-full border border-gray-300 p-2 rounded flex items-center justify-center"
      >
        <Image src="/google.png" alt="Google" width={20} height={20} />
        <span className="ml-2">Đăng ký với Google</span>
      </button>
      <p className="text-center">
        Đã có tài khoản?{" "}
        <span onClick={onSwitchToLogin} className="text-blue-500 cursor-pointer">
          Đăng nhập
        </span>
      </p>
    </form>
  );
};
  // return (
  //   <div className="FullScreenup" >
  //     <div className="SignupOverlay" onClick={onClose}>
  //       <div className="SignupContain" onClick={(e) => e.stopPropagation()}>
  //         <div className="image">

  //         </div>
  //         <div className="Signupform">
  //           <h1>Sign up</h1>
  //           <div className="CorrectLine">
  //             <p>Email*</p>
  //             <input name="Email" type="email" placeholder="Enter Email" onChange={handleChange}></input>
  //             {errors.Email && <span className="error">{errors.Email}</span>}
  //           </div>
  //           <div className="CorrectLine">
  //             <p>Username*</p>
  //             <input name="Username" type="text" placeholder="Enter Username" onChange={handleChange}></input>
  //             {errors.Username && <span className="error">{errors.Username}</span>}
  //           </div>
  //           <div className="CorrectLine">
  //             <p>Full Name</p>
  //             <input
  //               name="FullName"
  //               type="text"
  //               placeholder="Enter Full Name"
  //               onChange={handleChange}
  //             />
  //             {errors.FullName && <span className="error">{errors.FullName}</span>}

  //           </div>
  //           <div className="CorrectLine">
  //             <p>Phone Number</p>
  //             <input name="PhoneNumber" type="text" placeholder="Enter your number" onChange={handleChange}></input>
  //             {errors.PhoneNumber && <span className="error">{errors.PhoneNumber}</span>}
  //           </div>

  //           <div className="CorrectLine">
  //             <p>Password*</p>
  //             <input name="Password" type={showPassword ? "text" : "password"} placeholder="Enter Password" onChange={handleChange}></input>
  //             {errors.Password && <span className="error">{errors.Password}</span>}
  //           </div>

  //           <div className="CorrectLine">
  //             <p>Confirm Password*</p>
  //             <input name="confirmPassword" type={showPassword ? "text" : "password"} placeholder="Confirm Password" onChange={handleChange}></input>
  //             {errors.confirmPassword && (
  //               <span className="error">{errors.confirmPassword}</span>
  //             )}
  //           </div>

  //           <div className="ToS">
  //             <input type="checkbox"></input>
  //             <p>Agreed with Term of Services</p>
  //           </div>

  //           <button onClick={handleSubmit}> Sign Up </button>
  //         </div>
  //       </div>
  //     </div>
  //   </div>
  // );

export default RegisterForm;