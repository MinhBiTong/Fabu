import { toastError, toastSuccess } from "../services/ToastService";
import { LoginApi } from "../app/api/authApi";
import { useAuth } from "../hooks/use-auth";
import { useForm } from "react-hook-form";
import { loginSchema, type LoginFormData } from "../core/validations/LoginSchema";
import { zodResolver } from "@hookform/resolvers/zod";
// import { useRouter } from "next/router";
import { useRouter } from "next/navigation";

export const useLogin = () => {
  const { setToken } = useAuth();
  const router = useRouter();
  const {
    register, //ham register cua react hook form tra ve 1 object chua name, onBlur, onChange, ref
    handleSubmit, //ham handleSubmit de wrap ham onSubmit, tu dong preventDefault va lay data
    formState: { errors, isSubmitting}, //lay errors va isSubmitting tu formState
  } = useForm<LoginFormData>({
    resolver: zodResolver(loginSchema), //ket noi voi zod
    defaultValues: {
      email: "",
      password: "",
    }
  });

  const onLoginSubmit = async (data: LoginFormData) => { //nhan data tu hook form, ko nhan event
    console.log(data);
   
    try {
      //gui xuong .net de xac thuc
      const result = (await LoginApi.login(
        data.email,
        data.password
      ));
      console.log(result);
      
      if (result?.code === 200&& result?.data?.accessToken) {
        toastSuccess(result.message);
        setToken(result.data.accessToken); //luu token vao context
        
        //redirect
        router.push("/");
        return { success: true };
        //router.refresh();
      } else {
        toastError(result.message  || "Login failed. Please try again.");
      }
    } catch (error: any) {
        const errorMessage = error.response?.data?.message || error.message || "An unexpected error occurred from the server.";
        toastError("Login failed. Please try again." + errorMessage);
    } 
  };

  const handleGoogleLogin = () => {
    const backendUrl = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";
    // Điều hướng trình duyệt trực tiếp đến Endpoint của Backend
    window.location.href = `${backendUrl}/api/v1/auth/external-login?provider=Google`;
  }

  const handleClick = () => {
    alert("This is a demo button click handler.");
  };

  return {
    register,
    handleSubmit,
    errors,
    isSubmitting,
    onLoginSubmit, 
    handleClick, 
    handleGoogleLogin
  };
};
