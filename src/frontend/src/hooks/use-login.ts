import { toastError, toastSuccess } from "../services/ToastService";
import { LoginApi } from "../app/api/authApi";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../hooks/use-auth";
import { useForm } from "react-hook-form";
import { loginSchema, type LoginFormData } from "../validators/ValidateFormLogin";
import { zodResolver } from "@hookform/resolvers/zod";

export const useLogin = () => {
  const { setAccessToken } = useAuth();
  const navigate = useNavigate();
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
      
      if (result.code === 200) {
        toastSuccess(result.message);
        setAccessToken(result.data.accessToken);
        
        //redirect
        navigate("/");
      } else {
        toastError(result.message);
      }
    } catch (error: any) {
        const errorMessage = error.response?.data?.message || error.message || "An unexpected error occurred from the server.";
        toastError("Login failed. Please try again." + errorMessage);
    } 
  };

  const handleClick = () => {
    alert("This is a demo button click handler.");
  };

  return {
    register,
    handleSubmit,
    errors,
    isSubmitting,
    onLoginSubmit, 
    handleClick
  };
};
