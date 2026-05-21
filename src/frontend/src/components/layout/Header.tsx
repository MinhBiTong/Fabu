"use client";
import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { useState } from "react";
import Image from "next/image";

import Loginform from "../../app/(auth)/login/page";
import { globalApiClient } from "../../app/api/api-client";

import logo from "../../styles/images/FABUlogo.png";
import Icon from "../../styles/images/search.png";
import Menu from "../../styles/images/menu.png";
import User from "../../styles/images/user.png";
import SignUpForm from "../../app/(auth)/register/page";

function Header() {
    const router = useRouter()
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const [showLogin, setShowLogin] = useState(false);
     const [showSignup , setSignup] = useState(false);
    const [showOptionbar, setOptionBar] = useState(false);
    const [showSettings , setSettings] = useState(false);
    const[showSettingOptions , setSettingOptions] = useState(false);

    useEffect(() => {
    const token = localStorage.getItem("accessToken");
    setIsLoggedIn(!!token);
    }, []);


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
       
         <button onClick={() => router.push("/contact")}>Contact</button>
      </div>
      <div className="signin">
      <button className="SearchButton">
        <Image src={Icon} alt="Search" width={20} height={20} />
      </button >
     
      {!isLoggedIn && (
        <button className="SigninButton" onClick={() => setShowLogin(true)}>
         Sign in
         </button>
      )}

    {isLoggedIn && (
       <button className="ProfileButton" onClick={() => setSettings(prev => !prev)}>
        <Image src={User} alt="Profile"/>
         </button>
     )}

      </div>

    </div>
      {showLogin && (
       <Loginform 
     onClose={() => setShowLogin(false)} 
     onSwitchToSignup={() => {
     setShowLogin(false);
      setSignup(true);
    }}
/>
      )}

      {showOptionbar &&(
        <div className="MenuList">
           <button onClick={() => { router.push("/"); setOptionBar(false); }}>Home</button>
          <button onClick={() => {router.push("/P5GDataPlan") ; setOptionBar(false); }}>P5GDataPlan</button>
         <button onClick={() => {router.push("/about"); setOptionBar(false);}}>About</button>
         <button onClick={() => {router.push("/contact"); setOptionBar(false);}}>Contact</button>
        </div>
      )}

     {showSignup && (
  <SignUpForm onClose={() => setSignup(false)} />
   )}
 
    {showSettings &&(
         <div className="Settings">
           <button onClick={() => setSettings(false)}>Profile</button>
          <button onClick={() => setSettings(false)}>Settings</button>
       <button onClick={() => setSettings(false)}>Contact</button>
       <button
  onClick={() => {
    // remove stored token
    localStorage.removeItem("accessToken");
    // clear token from ApiClient
    globalApiClient.setToken(null);
    // update UI state
    setIsLoggedIn(false);
    // close dropdown
    setSettings(false);

    router.push("/");
  }}
>
  Log out
</button>
        </div>
      )}

         {showSettingOptions &&(
         <div className="SettingsChoicesContainer">
            <div className="SettingsChoices">



            </div>
       </div>
      )}


    </>
  );
}

export default Header;