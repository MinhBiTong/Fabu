import Image from "next/image"
import Fabuabout from "../../styles/images/FabuAbout.png";
import Blender from "../../styles/images/newblenderlogo.png";

import Mobifone from "../../styles/images/mobifone.png";

import Viettel from "../../styles/images/viettel.png";

import MBBank from "../../styles/images/mbbanklogo.png";

export default function AboutUs(){
 return(
   <div className="AboutContainer">
    <h1>About Fabu</h1>
   <div className="WhatisFabu">
       <Image src={Fabuabout} alt=""></Image>
       <div className="Lineofsomething">
        <h2>What do we provide ?</h2>
        <p>We provide a modern platform that makes 5G recharge fast, simple, and reliable. Our service helps users quickly purchase data plans, manage usage, and stay connected anytime. With a focus on speed, security, and ease of use, we aim to deliver a smooth and seamless mobile connectivity experience. </p>
       </div>

   </div>
  <h1>Trustful Investors</h1>
  <div className="GridInvestor"> 
    <div className="Investorbox">
      <Image src={Blender} alt=""></Image>
    </div>
<div className="Investorbox"> 
<Image src={Mobifone} alt=""></Image>
   </div>
<div className="Investorbox">
<Image src={Viettel} alt=""></Image>
    </div>
 <div className="Investorbox">
  <Image src={MBBank} alt=""></Image>
    </div>
<div className="Investorbox"> 
  
   </div>
   <div className="Investorbox"> 
  
   </div>


  </div>









   </div>
 )
}