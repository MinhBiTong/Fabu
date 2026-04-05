"use client";
import { useState } from "react";

import { useRouter } from "next/navigation";
import Image from "next/image";
import Way from "../../styles/images/Way.png"


function AdminSidebar(){
   const router = useRouter()
   const [isOpen, setIsOpen] = useState(false);

  return (
    <>
    <Image className={`AdminPanelButton ${isOpen ? "open" : ""}`} src={Way} alt=""  onClick={() => setIsOpen(prev => !prev)}></Image>

  
   {isOpen && (  
      <div className="AdminBar">
       <button onClick={() => router.push("/AdminDashboard")}>Dashboard</button>
        <button onClick={() => router.push("/AdminTransactions")}> Transactions</button>
        <button onClick={() => router.push("/AdminFeedbacks")}>Feedbacks</button>
        <button onClick={() => router.push("/AdminPackages")}>Packages</button>
        <button onClick={() => router.push("/AdminAccounts")}>Accounts</button>
    </div>
      )}    
</>
  )
}

export default AdminSidebar;