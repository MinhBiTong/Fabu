"use client"

import Image from "next/image";
import Way from "../../styles/images/Way.png"


export default function TransactionsDetails() {



  return ( 
  <>
   <div className="TransactionsDetailsContainer">
      <h1>Transaction Details</h1>
      <div className="TransactionsBox">
         <div className="EachLine">
          <span>Transaction Date</span>
          <span>23/12/2024</span>
         </div>
   <div className="EachLine">
          <span>Customer Name</span>
          <span>23/12/2024</span>
         </div>
   <div className="EachLine">
          <span>Phone Numbers</span>
          <span>23/12/2024</span>
         </div>
<div className="EachLine">
          <span>Transactions Status</span>
          <span>23/12/2024</span>
         </div>
<div className="EachLine">
          <span>Services Type</span>
          <span>23/12/2024</span>
         </div>
<div className="EachLine">
          <span>Service Name</span>
          <span>23/12/2024</span>
         </div>
         <div className="EachLine">
          <span>Payment Type</span>
          <span>23/12/2024</span>
         </div>
<div className="EachLine">
          <span>Price</span>
          <span>23/12/2024</span>
         </div>




      </div>
      <div className="Optionsbuttons">
         <button>Edit</button>
      </div>
   </div>
  </>
  )
}