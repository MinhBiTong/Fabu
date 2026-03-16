"use client";

import { useRouter } from "next/navigation";
import { useState } from "react";
import Image from "next/image";

import Loginform from "../features/auth/LoginForm";

import logo from "../../styles/images/FABUlogo.png";
import Icon from "../../styles/images/search.png";
import Menu from "../../styles/images/menu.png";
import User from "../../styles/images/user.png";

function Header() {
    const router = useRouter()
    const [showLogin, setShowLogin] = useState(false);
    
    const [showOptionbar, setOptionBar] = useState(false);
  return (
    <>
    <div className="navibar">

      <div className="menu">
      <Image src={Menu} alt="Menu" onClick={() => setOptionBar(prev => !prev)} />
      </div>
      
       <div className="logo">
        <Image src={logo} alt="Logo" onClick={() => router.push("/")} />
      </div>

      <div className="navlinks">
         <button onClick={() => router.push("/")}>Home</button>
          <button onClick={() => router.push("/P5GDataPlan")}>P5GDataPlan</button>
         <button onClick={() => router.push("/about")}>About</button>
         <button onClick={() => router.push("/services")}>Services</button>
         <button onClick={() => router.push("/contact")}>Contact</button>
      </div>
      <div className="signin">
      <button className="SearchButton">
        <Image src={Icon} alt="Search" width={20} height={20} />
      </button >
     
        <button className="SigninButton"   onClick={() => setShowLogin(true)}>Sign in</button>
         <button className="ProfileButton">
           <Image src={User} alt="Search"/>
         </button>

      </div>

    </div>
      {showLogin && (
        <Loginform onClose={() => setShowLogin(false)} />
      )}

      {showOptionbar &&(
        <div className="MenuList">
           <button onClick={() => { router.push("/"); setOptionBar(false); }}>Home</button>
          <button onClick={() => {router.push("/P5GDataPlan") ; setOptionBar(false); }}>P5GDataPlan</button>
         <button onClick={() => {router.push("/about"); setOptionBar(false);}}>About</button>
         <button onClick={() => {router.push("/services") ; setOptionBar(false);}}>Services</button>
         <button onClick={() => {router.push("/contact"); setOptionBar(false);}}>Contact</button>
        </div>
      )}


    </>
  );
}

export default Header;