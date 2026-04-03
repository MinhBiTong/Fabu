"use client";
import Image from "next/image";

import test from "../../../styles/images/blueytitlebackground.png";

import { useState } from "react";
import { signupSchema } from "../../../core/validations/SignupSchema";
import { globalApiClient } from "@/app/api/ApiClient";

type Props = {
  onClose: () => void;
};

function SignUpForm({ onClose }: Props) {

   const [form, setForm] = useState({
    Email: "",
    Username: "",
     FullName: "",
    PhoneNumber: "",
    Password: "",
    confirmPassword: ""
  });

  const [errors, setErrors] = useState<any>({});

  const handleChange = (e: any) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async () => {
    
    const result = signupSchema.safeParse(form);

    if (!result.success) {
      const fieldErrors: any = {};
      result.error.issues.forEach((err) => {
        fieldErrors[err.path[0]] = err.message;
      });
      setErrors(fieldErrors);
      return;
    }

    try {
   
    const res = await globalApiClient.post<any>(
      "/Auth/register", 
      {
        Email: form.Email,
        Username: form.Username,
        FullName: form.FullName, 
        PhoneNumber: form.PhoneNumber,
        Password: form.Password
      }
    );

    if (res.code === 200) {
      alert("Account created!");
      onClose();
    }
  } catch (err) {
    console.error(err);
  }
};
  

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
      <input name="Email" type="email" placeholder="Enter Email" onChange={handleChange}></input>
      </div>
     <div className="CorrectLine">
       <p>Username</p>
      <input name="Username" type="text" placeholder="Enter Username" onChange={handleChange}></input>
      </div>   
      <div className="CorrectLine">
  <p>Full Name</p>
  <input
    name="FullName"
    type="text"
    placeholder="Enter Full Name"
    onChange={handleChange}
  />
</div>
     <div className="CorrectLine">
       <p>Phone Number</p>
      <input name="PhoneNumber" type="text" placeholder="Enter your number"onChange={handleChange}></input>
      </div>
     
        <div className="CorrectLine">
       <p>Password</p>
      <input name="Password" type="password" placeholder="Enter Password" onChange={handleChange}></input>
      </div>

      <div className="CorrectLine">
       <p>Confirm Password</p>
      <input name="confirmPassword" type="password" placeholder="Confirm Password" onChange={handleChange}></input>
      </div>
     
       <div className="ToS">
       <input type="checkbox"></input>
       <p>Agreed with Term of Services</p>
       </div>

       <button onClick={handleSubmit}> Sign Up </button>


    </div>




   </div>

    </div>
   
    </div>
  );
}
export default SignUpForm;