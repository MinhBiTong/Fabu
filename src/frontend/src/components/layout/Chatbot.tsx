"use client";
import { useState } from "react";

import Image from "next/image";
import Xbut from "../../styles/images/close.png"
import Sending from "../../styles/images/Sending.png"
import Optins from "../../styles/images/listchoice.png"

function Chatbot(){
    const [isOpen, setIsOpen] = useState(false);

  return (
    <>
     {!isOpen && (
 <div className="BotCircle"   onClick={() => setIsOpen(true)} ></div>
 )}

   {isOpen && (
 <div className="ChatContainer">
   <div className="ChatTopbar">
     <div className="BotIcon"></div>
     <Image src={Xbut} alt=""   onClick={() => setIsOpen(false)}></Image>
   </div>
   <div className="Chattingbox">
    {/* Each time send paste them here cause its reversed  */}
    
  <div className="UserSpeechBubble">
    <p>Working i guess but idk how would this line extend because i don't know how it works or other waise</p>
  </div> 
  <div className="BotSpeechBubble">
    <p>Working i guess UwU</p>
  </div> 
  <div className="UserSpeechBubble">
    <p>Working i guess</p>
  </div> 




   </div>

   <div className="ChatInputbox">
     <Image src={Optins} alt=""></Image>
     <input placeholder="Type something"></input>
     <Image src={Sending} alt=""></Image>
   </div>

</div>
)}
</>
  )
}

export default Chatbot;