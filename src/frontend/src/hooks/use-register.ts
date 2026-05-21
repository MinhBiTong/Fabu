import { toastError, toastSuccess } from "../services/toast-service";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter } from "next/navigation";
import { signupSchema, type SignupFormData } from "../core/validations/signup.schema";
import { globalApiClient } from "@/app/api/api-client";
import { LoginApi } from "@/app/api/auth-api";

export const useRegister = (onClose?: () => void) => {
  const router = useRouter();

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<SignupFormData>({
    resolver: zodResolver(signupSchema),
    defaultValues: {
      username: "",
      email: "",
      password: "",
      confirmPassword: "",
    },
  });

  const onRegisterSubmit = async (data: SignupFormData) => {
    console.log(data);
    
    try {
      // Gọi API đăng ký (Thay đổi endpoint tùy theo backend của bạn)
      const response = (await LoginApi.register(
        data.email,
        data.password,
        data.username,
        data.confirmPassword
      ));

      if (response.code === 200 || response.code === 201) {
        toastSuccess("Đăng ký tài khoản thành công!");
        
        // Nếu có truyền vào hàm đóng Modal thì thực thi
        if (onClose) onClose();
        
        // Điều hướng người dùng (ví dụ: sang trang login hoặc bắt xác thực email)
        router.push("/login"); 
      }
    } catch (error: any) {
      const errorMessage = error.response?.data?.message || error.message || "Đăng ký thất bại.";
      toastError(errorMessage);
    }
  };

  return {
    register,
    handleSubmit,
    errors,
    isSubmitting,
    onRegisterSubmit,
  };
};