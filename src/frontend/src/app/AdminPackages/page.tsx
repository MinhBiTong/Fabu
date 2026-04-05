"use client"

import Image from "next/image";
import Way from "../../styles/images/Way.png"

import { useEffect, useState } from "react";
import { globalApiClient } from "@/app/api/ApiClient";

import { useRouter } from "next/navigation";

export default function AdminPackages() {
const router = useRouter()

const [packages, setPackages] = useState<Package[]>([]);

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

useEffect(() => {
  const fetchPackages = async () => {
    try {
         const token = localStorage.getItem("accessToken");
        globalApiClient.setToken(token);

      const res = await globalApiClient.get("/Service");

      console.log("DATA:", res.data);

      setPackages(Array.isArray(res.data) ? res.data : []);

    } catch (err) {
      console.error(err);
    }
  };

  fetchPackages();
}, []);



  return ( 
  <>
  <div className="PackagesContainer">
      <h1>Packages</h1> 

   <div className="SearchTools">
      <input className="Search" placeholder="Search Package Name"></input>

  
    </div>
     <div className="SearchTools">
        <button className="AddPack">Add Package</button>
    </div>
 <div className="ChooseButtons">
   </div>
   <div className="TableList">
     <table>
        <thead>
          <tr>
              <th>Package Name</th>
              <th>Amount</th>
              <th>Category</th>
              <th>Price</th>
              <th>Length</th>
              <th>Options</th>
          </tr>
        </thead>
         <tbody>
          <tr>
              <td>APK123</td>
               <td>160GB</td>
               <td>asdwasd</td>
               <td>9000$</td>
               <td>30 Dáy</td>
               <td>  <span
    className="Clickablewords"
    onClick={() => router.push("/AdminPackages/PackagesDetails")}
  >
    Details
  </span></td>

            </tr>




              <tr>
              <td>APK123</td>
               <td>160GB</td>
               <td>asdwaqsda</td>
               <td>9000$</td>
               <td>30 Dáy</td>
               <td><span
    className="Clickablewords"
    onClick={() => router.push("/AdminPackages/PackagesDetails")}
  >
    Details
  </span></td>

            </tr>


 {packages.map((pkg) => (
                <tr key={pkg.id}>
                  <td>{pkg.serviceName}</td>
                  <td>{pkg.dataAmountMB} GB</td>
                  <td>{pkg.category} GB</td>
                  <td>{pkg.price}$</td> 
                  <td>{pkg.validityDays} Days</td>
                  <td>
                    <span
                      className="Clickablewords"
                      onClick={() =>
                        router.push(`/AdminPackages/PackagesDetails/${pkg.id}`)
                      }
                    >
                      Details
                    </span>
                  </td>
                </tr>
              ))}






         </tbody>
     </table>

   </div>

   <div className="Pagination">
              <div className="Left">
        <Image src={Way} alt=""></Image>
         </div>
         <div className="Page">
             <span>3</span>
         </div>
         <div className="Right">
       <Image src={Way} alt=""></Image>
        </div> 

  </div>



  </div>
  </>
  )
}