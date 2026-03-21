"use client";
import Image from "next/image";

import test from "../../../styles/images/blueytitlebackground.png";

type Props = {
  onClose: () => void;
};

function SignUpForm({ onClose }: Props) {
  return (
    <div className= "FullScreenup" >
    <div className="SignupOverlay" onClick={onClose}>
     <div className="SignupContain" onClick={(e) => e.stopPropagation()}>
    <div className="image">

    </div>    
    <div className="Signupform">
      <h1>Sign up</h1>
      <div className="CorrectLine">
       <p>Email</p>
      <input type="Email" placeholder="Enter Email"></input>
      </div>
     <div className="CorrectLine">
       <p>Username</p>
      <input type="text" placeholder="Enter Username"></input>
      </div>

    <div className="JoinLine">
     <div className="CorrectLine">
       <p>Phone Number</p>
      <input type="number" placeholder="Enter your number"></input>
      </div>
      <div className="CorrectLine">
       <p>Birth Date</p>
      <input type="date" placeholder="Choose a Date"></input>
      </div>
   </div>
       
        <div className="CorrectLine">
       <p>Password</p>
      <input type="password" placeholder="Enter Password"></input>
      </div>

      <div className="CorrectLine">
       <p>Confirm Password</p>
      <input type="password" placeholder="Confirm Password"></input>
      </div>
     
       <div className="ToS">
       <input type="checkbox"></input>
       <p>Agreed with Term of Services</p>
       </div>

       <button> Sign Up </button>


    </div>




   </div>

    </div>
   
    </div>
  );
}
export default SignUpForm;