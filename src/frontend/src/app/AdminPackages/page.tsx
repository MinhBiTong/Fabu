"use client"

import Image from "next/image";
import Way from "../../styles/images/Way.png"
import { useRouter } from "next/navigation";

export default function AdminPackages() {
const router = useRouter()

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
              <th>Bonus</th>
              <th>Price</th>
              <th>Length</th>
              <th>Options</th>
          </tr>
        </thead>
         <tbody>
          <tr>
              <td>APK123</td>
               <td>160GB</td>
               <td>12GB</td>
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
               <td>12GB</td>
               <td>9000$</td>
               <td>30 Dáy</td>
               <td><span
    className="Clickablewords"
    onClick={() => router.push("/AdminPackages/PackagesDetails")}
  >
    Details
  </span></td>

            </tr>


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