"use client";

import { useState } from "react";
import Image from "next/image";

import Loginform from "../features/auth/LoginForm";

import logo from "../../styles/images/FABUlogo.png";
import Icon from "../../styles/images/search.png";
import Menu from "../../styles/images/menu.png";
import User from "../../styles/images/user.png";

function Header() {

    const [showLogin, setShowLogin] = useState(false);
    
  return (
    <>
    <div className="navibar">

      <div className="menu">
      <Image src={Menu} alt="Menu"  />
      </div>
      
       <div className="logo">
        <Image src={logo} alt="Logo"  />
      </div>

      <div className="navlinks">
        <button>Homasdasde</button>
        <button>Aboasdasdut</button>
        <button>Serasdasdvices</button>
        <button>Contasdasdact</button>
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

    </>
  );
}

export default Header;