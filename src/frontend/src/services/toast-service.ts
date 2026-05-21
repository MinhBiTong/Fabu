import { toast } from "react-toastify";

export const toastSuccess = (message: string) => {
  toast.success(message, {
    icon: "✅",
    className: "border-l-4 border-emerald-500",
    progressClassName: "fancy-progress-bar",
  });
};

export const toastError = (message: string) => {
  toast.error(message, {
    position: "bottom-left",
    autoClose: 4000,
    icon: "❌",
  });
};

export const toastWarning = (message: string) => {
  toast.warn(message, {
    position: "top-center",
    draggable: true,
    pauseOnHover: true,
  });
};


