"use client"

import Image from "next/image"
import Wallet from "../../../styles/images/wallet.png";
import Clock from "../../../styles/images/clock.png";
import Bonus from "../../../styles/images/revenue.png";
import Gb from "../../../styles/images/gb.png";
import PName from "../../../styles/images/subscription.png";

export default function PackagesDetails() {
  return ( 
    <div className="AdminTotality">
        <h1>Data Plan</h1>
   <div className="DpDetailsContainer">
   <div className="MainInfos">
     <div className="Infobox">
         <Image src={PName} alt=""></Image>
        <div className="Infotexts"> 
      <p>Plan Name</p>
       <p>SDKD3345</p>
       </div>
     </div>

    <div className="Infobox">
         <Image src={Wallet} alt=""></Image>
        <div className="Infotexts">
       <p>Price</p>
       <p>900.000VND</p>
       </div>
     </div>

     <div className="Infobox">
         <Image src={Clock} alt=""></Image>
        <div className="Infotexts">
       <p>Duration</p>
       <p>30 Days</p>
       </div>
     </div>

     <div className="Infobox">
         <Image src={Gb} alt=""></Image>
        <div className="Infotexts">
       <p>Amount</p>
       <p>89GB/day</p>
       </div>
     </div>

     <div className="Infobox">
         <Image src={Bonus} alt=""></Image>
        <div className="Infotexts">
       <p>Bonus</p>
       <p>2GB</p>
       </div>
     </div>

   </div>

   <div className="Descriptions">
   <div className="Descriptioncontent">
    <p>dsakjhdkhdk ksahjdkjahdkjsadh akdhjkasdhkjasd kasasssss sssss sssssssssssssssssssss sssssssssssss ssssssssssssdhkjdhsdksj dasdhkdkhjkdkdk akdskhjkh</p>
    
    </div>
   </div>

   </div>

<div className="ChoiceButtons">
      <button className="Sub"> Delete</button>
      <button className="Sub"> Edit</button>
     </div>
  



   </div>
  )
}