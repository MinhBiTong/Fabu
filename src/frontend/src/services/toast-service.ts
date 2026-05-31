import { toast } from "react-toastify";

export const toastSuccess = (message: string) => {
  toast.success(message, {
    className: "border-l-4 border-emerald-500",
  });
};

export const toastError = (message: string) => {
  toast.error(message, {
    position: "bottom-left",
    autoClose: 4000,
  });
};

export const toastWarning = (message: string) => {
  toast.warn(message, {
    position: "top-center",
    draggable: true,
    pauseOnHover: true,
  });
};
