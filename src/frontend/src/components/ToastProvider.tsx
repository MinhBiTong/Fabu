import { ToastContainer } from "react-toastify";
import "../assets/styles/toast.css";

export const ToastProvider = () => {
  return (
    <ToastContainer
      position="top-right"
      autoClose={4000}
      hideProgressBar={false}
      newestOnTop
      closeOnClick
      pauseOnHover
      draggable
      theme="light"
      progressClassName="fancy-progress-bar"
    />
  );
};
