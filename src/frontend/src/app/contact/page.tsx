"use client"

import { ContactForm } from "@/components/ui/Form/contact-form";
import Image from "next/image";
import { useState } from "react";

import Phone from "../../styles/images/phonecall.png";
import Starrate from "../../styles/images/Starating.png"
import Starrateyes from "../../styles/images/StarratingYes.png"

import Location2 from "../../styles/images/location2.png";

import Gmail from "../../styles/images/gmail.png"


export default function ContactPage() {
  const [hovered, setHovered] = useState(0);
  const [rating, setRating] = useState(0);

  return (
    <div className="ContactContainer">
          <h1>Contact Us</h1>
       
      <div className="ContactContent">
      
             <h3>How would you rate your experience?</h3>
           <div className="Starsrating"> 
             {[1, 2, 3, 4, 5].map((star) => (
        <Image
          key={star}
          src={star <= (hovered || rating) ? Starrateyes :Starrate}
          alt=""
          onClick={() => setRating(star)}
          onMouseEnter={() => setHovered(star)}
          onMouseLeave={() => setHovered(0)}
        />
      ))}
           </div>
           <h3>Any suggestions for improvement? Send us a message!</h3>
 
             <div className="ContactForm">
              
              <p>Email</p>
               <input type="text" placeholder="Enter your email" />
              <p>Subject</p>
               <input type="text" placeholder="Enter subject" />
              <p>Message</p>
               <textarea placeholder="Enter your message" />

                <button type="submit">Submit</button>
                </div>
                </div>
     <h1>You want to contact us directly ?</h1>
       <div className="ContactInfos">
        <div className="ContactLine">
         <div className="ContactBox">
         <Image src={Phone} alt="" ></Image>
          <h3>Phone Numbers</h3>
          <div className="Lines">
            <p>0924010294</p>
           <p>0694206767</p>
                 <p>0694206767</p>
                 
           </div>
          
         </div>
        <div className="ContactBox">
          <Image src={Location2} alt="" ></Image>
          <h3>Phone Numbers</h3>
          <div className="Lines">
            <p> fjsdkahksdj ashjkdg d ashkjdh kjasd d jkasdjk dh s hdsadjhd sajd sakd dsausad dhjekdwi sdkw </p>
         
                 
           </div>

         </div>
        </div>

       <div className="ContactLine">
             <div className="ContactBox">
         <Image src={Gmail} alt="" ></Image>
          <h3>Phone Numbers</h3>
          <div className="Lines">
            <p> fjsdkahksdj ashjkdg d ashkjdh kjasd d jkasdjk dh s hdsadjhd sajd sakd dsausad dhjekdwi sdkw </p>
         
                 
           </div>


          
         </div>
      
       </div>



       </div>
      </div>
  );
}