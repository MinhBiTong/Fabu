"use client";

type Props = {
  onClose: () => void;
};

function LoginForm({ onClose }: Props) {
  return (
    <div className= "FullScreen">
    <div className="loginOverlay">

      <div className="loginform">

        <button className="closeBtn"onClick={onClose}>✕</button>

        <h1>Sign In</h1>

       <div className="Mainform">
        <input type="email" placeholder="Email" />
        <input type="password" placeholder="Password" />

        <button className="loginSubmit">
          Login
        </button>
           </div>

           
      </div>

    </div>
    </div>
  );
}
export default LoginForm;