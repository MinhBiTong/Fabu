"use client"

import Image from "next/image";
import Way from "../../styles/images/Way.png"

import { useEffect, useState } from "react";
import { globalApiClient } from "@/app/api/api-client";
import { useRouter } from "next/navigation";


export default function AdminAccounts() {

 const router = useRouter();

  const [accounts, setAccounts] = useState<User[]>([]);

  type User = {
    id: number;
    email: string;
    userName: string;
    fullName: string;
    role: string;
    dateOfBirth: string;
  };

  useEffect(() => {
    const fetchAccounts = async () => {
      try {
        const token = localStorage.getItem("accessToken");
        globalApiClient.setToken(token);

       
        const res = await globalApiClient.get<User[]>("Users");

        console.log("ACCOUNTS:", res.data);

        setAccounts(res.data);
      } catch (err) {
        console.error(err);
      }
    };

    fetchAccounts();
  }, []);

  

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

       {accounts.map((acc) => (
                <tr key={acc.id}>
                  <td>{acc.email}</td>
                  <td>{acc.userName}</td>
                  <td>{acc.fullName}</td>
                  <td>{acc.role}</td>
                  <td>
                    {new Date(acc.dateOfBirth).toLocaleDateString()}
                  </td>
                  <td>
                    <span
                      className="Clickablewords"
                      onClick={() =>
                        router.push(`/AdminAccounts/${acc.id}`)
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