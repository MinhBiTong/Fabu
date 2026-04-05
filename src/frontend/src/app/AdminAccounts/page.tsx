"use client"

import Image from "next/image";
import Way from "../../styles/images/Way.png"

export default function AdminAccounts() {


  return ( 
  <>
  <div className="AccountsContainer">
      <h1>Accounts</h1> 

   <div className="SearchTools">
      <input className="Search" placeholder="Search Gmail/Name"></input>

    


    </div>

 <div className="ChooseButtons">
    <button>Customers</button>
      <button>Staffs</button>
   </div>
   <div className="TableList">
     <table>
        <thead>
          <tr>
              <th>Email</th>
              <th>UserNAme</th>
              <th>Full Name</th>
              <th>Role</th>
              <th>Date Birth</th>
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
               <td>Details</td>

            </tr>
              <tr>
              <td>APK123</td>
               <td>160GB</td>
               <td>12GB</td>
               <td>9000$</td>
               <td>30 Dáy</td>
               <td>Details</td>

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