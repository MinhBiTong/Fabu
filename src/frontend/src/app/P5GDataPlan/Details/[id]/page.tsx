"use client"

import Image from "next/image"
import Wallet from "../../../../styles/images/wallet.png";
import Clock from "../../../../styles/images/clock.png";
import Bonus from "../../../../styles/images/revenue.png";
import Gb from "../../../../styles/images/gb.png";
import PName from "../../../../styles/images/subscription.png";

import { useEffect, useState } from "react";
import { useParams } from "next/navigation";
import { globalApiClient } from "@/app/api/ApiClient";

export default function DataPlanDetails() {

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
    maxActivationsPerMonth: number;
  };

 
  const [formData, setFormData] = useState<Package | null>(null);

  const params = useParams();
  const id = Array.isArray(params.id) ? params.id[0] : params.id;

  const [pkg, setPkg] = useState<Package | null>(null);

  useEffect(() => {
    const fetchPackage = async () => {
      try {
        const token = localStorage.getItem("accessToken");
        globalApiClient.setToken(token);

        const res = await globalApiClient.get<Package>(`Service/${id}`);
        setPkg(res.data);
      } catch (err) {
        console.error("DETAIL ERROR:", err);
      }
    };

    if (id) fetchPackage();
  }, [id]);

   useEffect(() => {
     if (pkg) setFormData(pkg);
   }, [pkg]);

  if (!pkg) return <div>Loading...</div>;



  return ( 
    <div className="Totality">
        <h1>Data Plan</h1>
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
            <Image src={Gb} alt="" />
            <div className="Infotexts">
              <p>Service Code</p>
              <p>{pkg.serviceCode}</p>
            </div>
          </div>

          <div className="Infobox">
            <Image src={Wallet} alt="" />
            <div className="Infotexts">
              <p>Price</p>
              <p>{pkg.price.toLocaleString()} VND</p>
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
              <p>{pkg.dataAmountMB} MB</p>
            </div>
          </div>

          <div className="Infobox">
            <Image src={Bonus} alt="" />
            <div className="Infotexts">
              <p>Category</p>
              <p>{pkg.category}</p>
            </div>
          </div>

        </div>

      
        <div className="Descriptions">
          <div className="Descriptioncontent">
            <p>{pkg.description}</p>
          </div>
        </div>

      </div>

<div className="ChoiceButtons">
      <button className="Sub"> Subscribe</button>
     </div>
  



   </div>
  )
}