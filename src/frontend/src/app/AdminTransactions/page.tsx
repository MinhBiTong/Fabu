"use client"

import Image from "next/image";
import Way from "../../styles/images/Way.png"

export default function AdminTransactions() {


  return ( 
  <>
  <div className="TransactionsContainer">
      <h1>Transactions</h1> 

   <div className="SearchTools">
      <input className="Search" placeholder="Search username or Email"></input>
     <select className="Statusdropdown">
  <option value="">Choose Status</option>
  <option value="Cancelled">Cancelled</option>
  <option value="Completed">Completed</option>
  <option value="Pending">Pending</option>
     </select>
    


    </div>

   <div className="ChooseButtons">
      <button>Recharges</button>
      <button>Data Pack</button>
   </div>

   <div className="TableList">
     <table>
        <thead>
          <tr>
              <th>Email</th>
              <th>Username</th>
              <th>Services Type</th>
              <th>Price</th>
              <th>Status</th>
              <th>Options</th>
          </tr>
        </thead>
         <tbody>
          <tr>
              <td>Bingchill123@gmail.com</td>
               <td>BBingchillguy</td>
               <td>Data Plan</td>
               <td>9000$</td>
               <td>Completed</td>
               <td>asdasd</td>

            </tr>
             <tr>
              <td>Bingchill123@gmail.com</td>
               <td>BBingchillguy</td>
               <td>Data Plan</td>
               <td>9000$</td>
               <td>Completed</td>
               <td>asdasd</td>

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