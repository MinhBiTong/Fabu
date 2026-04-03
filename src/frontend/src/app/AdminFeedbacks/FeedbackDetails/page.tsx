"use client"

import Image from "next/image";

import Star from "../../../styles/images/StarratingYes.png"


export default function FeedbacksDetails() {

  return ( 
  <>
  <div className="AdminFeedDetailsContainer"> 
    <h1>Nguyen Binh Huong's Feedback</h1>
    <div className="StarsRated">
           <Image src={Star} alt=""></Image>
            <Image src={Star} alt=""></Image>
            <Image src={Star} alt=""></Image>

    </div>
     <div className="Nameplace">
        <span>UserName : Karrykaro</span>
       <span>Email : Ntduon24566@gmail.com</span>
     </div>
     <div className="DisplayContent">
      <h2> Subject : Why the fuck this exist ?  </h2>
      <span>dsaas rgrgeer gergerger gergerg ergerg ergerg ergerg erger gerg ererg egerger gerg rgger grger geg eg er e r er erer e rererere rereree rererere rerere erre</span>
     </div>
    
  


  </div>

  
  
  </>
  )
}