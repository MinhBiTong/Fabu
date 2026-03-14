import Image from "next/image";
import Arrow from "../../styles/images/upward-arrow.png";
export default function PhoneService() {
  return (
    <div className="p5GContainer">
    <h1>5G Data Plans</h1>
    <div className="ChoosePackage">
      <div className="MobilePackageType" >4G/5G Package</div>  
      <div className="MobilePackageType" >5G Package</div>  
      <div className="MobilePackageType" >Hot Package</div>  
      <div className="MobilePackageType" >Dcom Package</div>  
      <div className="MobilePackageType" >Roaming Package</div>  
    </div>
    <div className="ValueOptions">
      <div className="Opted">
        <p>Price</p>
        <Image src={Arrow} alt="Price" />
      </div>
      <div className="Opted">
        <p>All</p>
        </div>
      <div className="Opted">
        <p>30 days</p>
        </div>
      <div className="Opted">
        <p>Longer days</p>
        </div>
    </div>
      <div className="PackageContain">
        <div className="PackageandViewAll">  
          <h2>Package Set Name </h2>
          <h3>View All</h3>
          </div>
          <div className="PackageSet">
            <div className="Package">
              <div className="PackageTitle">
                <h2> Package Name</h2>
              </div>
              <div className="PackageOffer">
                <p>something new</p>
                  <p>something new</p>
                    <p>something new</p>
              </div>
              <div className="PackagePrice"> 
                <p>$90000</p>
                </div>
              <div className="PackageChoices">
                <button>Subscribe</button>
                <h3>View Details</h3>
              </div>
            </div>
         
        </div>

         <div className="PackageandViewAll">  
          <h2>Package Set Name </h2>
          <h3>View All</h3>
          </div>
          <div className="PackageSet">
            <div className="Package">
              <div className="PackageTitle">
                <h2> Package Name</h2>
              </div>
              <div className="PackageOffer">
                <p>something new</p>
                  <p>something new</p>
                    <p>something new</p>
              </div>
              <div className="PackagePrice"> 
                <p>$90000</p>
                </div>
              <div className="PackageChoices">
                <button>Subscribe</button>
                <h3>View Details</h3>
              </div>
            </div>
            <div className="Package">
              <div className="PackageTitle">
                <h2> Package Name</h2>
              </div>
              <div className="PackageOffer">
                <p>something new</p>
                  <p>something new</p>
                    <p>something new</p>
              </div>
              <div className="PackagePrice"> 
                <p>$90000</p>
                </div>
              <div className="PackageChoices">
                <button>Subscribe</button>
                <h3>View Details</h3>
              </div>
            </div>
               <div className="Package">
              <div className="PackageTitle">
                <h2> Package Name</h2>
              </div>
              <div className="PackageOffer">
                <p>something new</p>
                  <p>something new</p>
                    <p>something new</p>
              </div>
              <div className="PackagePrice"> 
                <p>$90000</p>
                </div>
              <div className="PackageChoices">
                <button>Subscribe</button>
                <h3>View Details</h3>
              </div>
            </div>
        </div>
        {/* Bộ Sản phẩm*/}
        {/* 
         <div className="PackageandViewAll">  
          <h2>Package Set Name </h2>
          <h3>View All</h3>
          </div>
          <div className="PackageSet">

           // hộp từng sản phẩm //

            <div className="Package">
              <div className="PackageTitle">
                <h2> Package Name</h2>
              </div>
              <div className="PackageOffer">
                <p>something new</p>
                  <p>something new</p>
                    <p>something new</p>
              </div>
              <div className="PackagePrice"> 
                <p>$90000</p>
                </div>
              <div className="PackageChoices">
                <button>Subscribe</button>
                <h3>View Details</h3>
              </div>
            </div>

          // hộp từng sản phẩm //

            </div>
             */}
      </div>
   


    </div>


    
  );
}