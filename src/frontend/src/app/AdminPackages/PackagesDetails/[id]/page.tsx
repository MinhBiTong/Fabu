"use client";

import Image from "next/image";
import Wallet from "../../../../styles/images/wallet.png";
import Clock from "../../../../styles/images/clock.png";
import Bonus from "../../../../styles/images/revenue.png";
import Gb from "../../../../styles/images/gb.png";
import PName from "../../../../styles/images/subscription.png";

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
    maxActivationsPerMonth: number;
  };

  const [isEditing, setIsEditing] = useState(false);
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




   const handleChange = (field: keyof Package, value: any) => {
  setFormData((prev) => prev ? { ...prev, [field]: value } : prev);
};


const handleDelete = async () => {
  if (!confirm("Are you sure you want to delete this package?")) return;

  try {
    const token = localStorage.getItem("accessToken");
    globalApiClient.setToken(token);

    await globalApiClient.delete(`Service/${id}`);

    alert("Deleted successfully");
      window.location.href = "/AdminPackages"; 
    } catch (err) {
    console.error("DELETE ERROR:", err);
  }
};

const handleUpdate = async () => {
  try {
    const token = localStorage.getItem("accessToken");
    globalApiClient.setToken(token);

    await globalApiClient.put(`Service/${id}`, formData);

    setPkg(formData);
    setIsEditing(false);

    alert("Updated successfully");
  } catch (err) {
    console.error("UPDATE ERROR:", err);
  }
};


  return ( 
    <div className="AdminTotality">
        <h1>Data Plan</h1>
  <div className="DpDetailsContainer">

        <div className="MainInfos">

  <div className="Infobox">
    <Image src={PName} alt="" />
    <div className="Infotexts">
      <p>Service Name</p>
      {isEditing ? (
  <input
    value={formData?.serviceName || ""}
    onChange={(e) => handleChange("serviceName", e.target.value)}
  />
) : (
  <p>{pkg.serviceName}</p>
)}
    </div>
  </div>


 <div className="Infobox">
    <Image src={PName} alt="" />
    <div className="Infotexts">
      <p>Service Code</p>
       {isEditing ? (
  <input
    value={formData?.serviceCode || ""}
    onChange={(e) => handleChange("serviceCode", e.target.value)}
  />
) : (
  <p>{pkg.serviceCode}</p>
)}
    </div>
  </div>

  <div className="Infobox">
    <Image src={Wallet} alt="" />
    <div className="Infotexts">
      <p>Price</p>
      {isEditing ? (
  <input
    type="number"
    value={formData?.price || 0}
    onChange={(e) => handleChange("price", Number(e.target.value))}
  />
) : (
  <p>{pkg.price}$</p>
)}
    </div>
  </div>

  <div className="Infobox">
    <Image src={Clock} alt="" />
    <div className="Infotexts">
      <p>Duration</p>
       {isEditing ? (
  <input
    value={formData?.validityDays || ""}
    onChange={(e) => handleChange("validityDays", e.target.value)}
  />
) : (
  <p>{pkg.validityDays} Days</p>
)}
    </div>
  </div>

  <div className="Infobox">
    <Image src={Gb} alt="" />
    <div className="Infotexts">
      <p>Amount</p>
    {isEditing ? (
  <input
    type="number"
    value={formData?.dataAmountMB || 0}
    onChange={(e) => handleChange("dataAmountMB", Number(e.target.value))}
  />
) : (
  <p>{pkg.dataAmountMB} GB</p>
)}
    </div>
  </div>

  <div className="Infobox">
    <Image src={Bonus} alt="" />
    <div className="Infotexts">
      <p>Category</p>
         {isEditing ? (
  <input
    value={formData?.category || ""}
    onChange={(e) => handleChange("category", e.target.value)}
  />
) : (
  <p>{pkg.category}</p>
)}
    </div>
  </div>

<div className="Infobox">
    <Image src={Bonus} alt="" />
    <div className="Infotexts">
      <p>Activation per/month</p>
        {isEditing ? (
  <input
    value={formData?.maxActivationsPerMonth || ""}
    onChange={(e) => handleChange("maxActivationsPerMonth", e.target.value)}
  />
) : (
  <p>{pkg.maxActivationsPerMonth}</p>
)}
    </div>
  </div>

<div className="Infobox">
    <Image src={Bonus} alt="" />
    <div className="Infotexts">
      <p>isActive</p>
      {isEditing ? (
  <select
    value={formData?.isActive ? "true" : "false"}
    onChange={(e) => handleChange("isActive", e.target.value === "true")}
  >
    <option value="true">Yes</option>
    <option value="false">No</option>
  </select>
) : (
  <p>{pkg.isActive ? "Yes" : "No"}</p>
)}
    </div>
  </div>

</div>

<div className="Descriptions">
  <div className="Descriptioncontent">
  {isEditing ? (
  <textarea
    value={formData?.description || ""}
    onChange={(e) => handleChange("description", e.target.value)}
  />
) : (
  <p>{pkg.description}</p>
)}
  </div>
      </div>
        </div>




<div className="ChoiceButtons">
  <button className="Sub" onClick={handleDelete}>
    Delete
  </button>

  {isEditing ? (
    <button className="Sub" onClick={handleUpdate}>
      Save
    </button>
  ) : (
    <button className="Sub" onClick={() => setIsEditing(true)}>
      Edit
    </button>
  )}
</div>



   </div>
  )
}