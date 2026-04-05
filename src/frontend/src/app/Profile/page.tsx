import Image from "next/image"
import PUser from "../../styles/images/Puser.png";

import Way from "../../styles/images/Way.png"

export default function Profile() {
  return (
    <>
    <div className="ProfileContainer">
    <div className="MainInformation">
     <Image src={PUser} alt="sdf"></Image>
     <h2> Usernamehere</h2>
     <div className="BasicInfos">
     
        <div className="Lineeach">
        <p>FullName :</p>
        <p>Somethisng saldnladsafaldnlaksdkjl</p>
        </div>
         <div className="Lineeach">
        <p>Email :</p>
        <p>Somethisng saldnladsafaldnlaksdkjl</p>
        </div>
         <div className="Lineeach">
        <p>PhoneNumber :</p>
        <p>Somethisng saldnladsafaldnlaksdkjl</p>
        </div>
           <div className="Lineeach">
        <p>Date of Birth :</p>
        <p>Somethisng saldnladsafaldnlaksdkjl</p>
        </div>
        <div className="Lineeach">
        <p>Address :</p>
        <p>Somethisng saldnladsafaldnlaksdkjl</p>
        </div>
        <div className="Lineeach">
        <p>Role :</p>
        <p>Adinity</p>
        </div>
     </div>
    </div>
    <div className="AccountsInfo">
      <div className="Balances">
        <p>Balances :</p> <p> 3000$</p>
      </div>
       <div className="Balance">
        <p>Credit Limit :</p> <p> 3000$</p>
      </div>
         <h1>Transactions</h1> 
    <table className="HistoryList">
  <thead>
    <tr>
      <th>DateTime</th>
      <th>Service</th>
      <th>Price</th>
      <th>Status</th>
      <th>Options</th>
    </tr>
  </thead>

  <tbody>
    <tr>
      <td>21/21/1999</td>
      <td>Recharge</td>
      <td>100000VND</td>
      <td>Pending</td>
      <td>Details</td>
    </tr>

    <tr>
      <td>20/02/2222</td>
      <td>DataPlan</td>
      <td>90$</td>
      <td>Completed</td>
      <td>Details</td>
    </tr>
      <tr>
      <td>20/02/2222</td>
      <td>DataPlan</td>
      <td>90$</td>
      <td>Completed</td>
     <td>Details</td>
    </tr>
      <tr>
      <td>20/02/2222</td>
      <td>DataPlan</td>
      <td>90$</td>
      <td>Completed</td>
    <td>Details</td>
    </tr>
      <tr>
      <td>20/02/2222</td>
      <td>DataPlan</td>
      <td>90$</td>
      <td>Completed</td>
     <td>Details</td>
    </tr>
      <tr>
      <td>20/02/2222</td>
      <td>DataPlan</td>
      <td>90$</td>
      <td>Completed</td>
     <td>Details</td>
    </tr>
      <tr>
      <td>20/02/2222</td>
      <td>DataPlan</td>
      <td>90$</td>
      <td>Completed</td>
      <td>Details</td>
    </tr>
      <tr>
      <td>20/02/2222</td>
      <td>DataPlan</td>
      <td>90$</td>
      <td>Completed</td>
      <td>Options</td>
    </tr>
      <tr>
      <td>20/02/2222</td>
      <td>DataPlan</td>
      <td>90$</td>
      <td>Completed</td>
      <td>Options</td>
    </tr>
      <tr>
      <td>20/02/2222</td>
      <td>DataPlan</td>
      <td>90$</td>
      <td>Completed</td>
      <td>Options</td>
    </tr>
   
  </tbody>
</table>

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
    



    </div>

     <div className="Datetime">






     </div>
    </>
  );
}