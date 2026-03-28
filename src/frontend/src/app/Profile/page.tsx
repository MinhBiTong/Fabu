import Image from "next/image"
import PUser from "../../styles/images/Puser.png";

export default function Profile() {
  return (
    <>
    <div className="ProfileContainer">
    <div className="MainInformation">
     <Image src={PUser} alt="sdf"></Image>
     <div className="BasicInfos">
        <div className="Lineeach">
        <h2>UserName :</h2>
        <p>Somethisng saldnladsafaldnlaksdkjl</p>
        </div>
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

     </div>
    </div>
    <div className="AccountsInfo">
      <div className="Balances">
        <p>Balances :</p> <p> 3000$</p>
      </div>
       <div className="Balance">
        <p>Credit Limit :</p> <p> 3000$</p>
      </div>
         <h1>History</h1> 
             <p className="Bot">View All</p>
      <div className="HistoryList">
    <div className="HistoryBox">
      <p>DateTime</p>
      <p>Service</p>
      <p>Price</p>
      <p>Status</p>
      <p>Something</p>
    </div>
 <div className="HistoryBox">
      <p>DateTime</p>
      <p>Service</p>
      <p>Price</p>
      <p>Status</p>
      <p>Something</p>
    </div>
     <div className="HistoryBox">
      <p>DateTime</p>
      <p>Service</p>
      <p>Price</p>
      <p>Status</p>
      <p>Something</p>
    </div>
      </div>

     </div>
    



    </div>


    </>
  );
}