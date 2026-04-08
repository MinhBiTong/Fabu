"use client"

import Image from "next/image"
import Wallet from "../../../styles/images/wallet.png";
import Clock from "../../../styles/images/clock.png";
import Bonus from "../../../styles/images/revenue.png";
import Gb from "../../../styles/images/gb.png";
import PName from "../../../styles/images/subscription.png";

import { useEffect, useState } from "react";
import { useParams } from "next/navigation";
import { globalApiClient } from "@/app/api/ApiClient";

export default function PackagesDetails() {

 type Package = {
   id: number;
  serviceName: string;
  serviceCode: string;
  category: string;
  dataAmountMB: number;
  price: number;
  validityDays: number;
  description: string;
  isActive: boolean;
};

const { id } = useParams(); // 🔥 GET ID FROM URL
  const [pkg, setPkg] = useState<Package | null>(null);

  useEffect(() => {
    const fetchPackage = async () => {
      try {
        const token = localStorage.getItem("accessToken");
        globalApiClient.setToken(token);

        const res = await globalApiClient.get<Package>(`/Service/${id}`);

        console.log("DETAIL DATA:", res.Data);

        setPkg(res.Data); 

      } catch (err) {
        console.error("DETAIL ERROR:", err);
      }
    };

    if (id) fetchPackage();
  }, [id]);





if (!pkg) return <div>Loading...</div>;






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
  
 {/* API Verison  */}

 <div className="DpDetailsContainer">

        <div className="MainInfos">

  <div className="Infobox">
    <Image src={PName} alt="" />
    <div className="Infotexts">
      <p>Plan Name</p>
      <p>{pkg.serviceName}</p>
    </div>
  </div>

  <div className="Infobox">
    <Image src={Wallet} alt="" />
    <div className="Infotexts">
      <p>Price</p>
      <p>{pkg.price}$</p>
    </div>
  </div>

  <div className="Infobox">
    <Image src={Clock} alt="" />
    <div className="Infotexts">
      <p>Duration</p>
      <p>{pkg.validityDays} Days</p>
    </div>
  </div>

  <div className="Infobox">
    <Image src={Gb} alt="" />
    <div className="Infotexts">
      <p>Amount</p>
      <p>{pkg.dataAmountMB} GB</p>
    </div>
  </div>

  <div className="Infobox">
    <Image src={Bonus} alt="" />
    <div className="Infotexts">
      <p>Category</p>
      <p>{pkg.category} GB</p>
    </div>
  </div>

</div>

<div className="Descriptions">
  <div className="Descriptioncontent">
    <p>{pkg.description}</p>
  </div>
      </div>
        </div>















   </div>
  )
}