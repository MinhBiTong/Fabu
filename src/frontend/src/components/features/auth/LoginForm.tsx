"use client";

import { useRouter } from "next/navigation";
import { useState } from "react";
import Image from "next/image";

import logo from "../../styles/images/FABUlogo.png";
import Icon from "../../styles/images/search.png";
import Menu from "../../styles/images/menu.png";
import User from "../../styles/images/user.png";


type Props = {
  onClose: () => void;
  onSwitchToSignup: () => void;
};

function LoginForm({ onClose, onSwitchToSignup}: Props) {


  return (
    <div className= "FullScreen" >
    <div className="loginOverlay"onClick={onClose} >

      <div className="loginform"    onClick={(e) => e.stopPropagation()}>

        <button className="closeBtn"onClick={onClose}>✕</button>

        <h1>Sign In</h1>

       <div className="Mainform">
        <input type="email" placeholder="Email" />
        <input type="password" placeholder="Password" />
                   <p>Forgot Password </p>
        <button className="loginSubmit">
          Login
        </button>
           <div className="Signuplink">
        <span> No accounts ?</span> <p onClick={onSwitchToSignup}>Sign up here</p>
          </div>
           </div>

           
      </div>

    </div>
    </div>
  );
}
export default LoginForm;