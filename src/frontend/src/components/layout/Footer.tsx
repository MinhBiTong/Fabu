import Image from "next/image";
import logo from "../../styles/images/FABUlogo.png";
import FAQ from "../../styles/images/faq.png";
import Intro from "../../styles/images/idea.png";
import Location from "../../styles/images/location.png";
import Feedback from "../../styles/images/feedback.png";
import FaceBook from "../../styles/images/facebook.png";
import Youtube from "../../styles/images/youtube.png";

function Footer(){
return(
<div className="footer">
    
    <h1>Need help Brah ?</h1>
   <div className="footerinfo">
  <div className="topline">
    <Image src={logo} alt="Famu" />
    <p className="title">
      Website online chính thức của Famu.
    </p>
  </div>
  <p className="desc">
    Cơ quan chủ quản: Tổng Công ty Viễn thông Viettel (Viettel Telecom) - 
    Chi nhánh Tập đoàn Công nghiệp - Viễn thông Quân đội. 
    Mã số doanh nghiệp: 0100109106-011 do Sở Kế hoạch và Đầu tư Thành phố Hà Nội 
    cấp lần đầu ngày 18/07/2005, sửa đổi lần thứ 16 ngày 14/02/2025. 
    Chịu trách nhiệm nội dung: Ông Hoàng Trung Thành.
  </p>
</div>
    {/*
    <div className="Choosebox">
        <div className="helpbox">  
        <Image src={Intro}  alt="Choose" />
        <p>Famu Introduction</p>
        </div>
         <div className="helpbox">  
        <Image src={FAQ} alt="Choose" />
        <p>FAQ</p>
        </div>
         <div className="helpbox">  
        <Image src={Feedback} alt="Choose" />
        <p>Feedback</p>
        </div>
         <div className="helpbox">  
        <Image src={Location} alt="Choose" />
        <p>Company's Location</p>
        </div>
         <div className="helpbox">  
        <Image src="" alt="Choose" />
        <p>Famu Introduction</p>
        </div>
         <div className="helpbox">  
        <Image src="" alt="Choose" />
        <p>Famu Introduction</p>
        </div>
    </div>
    */}
    <div className="footerlinks">
        <div className="footerlinks-inner">
          <div className="linkscolumn">
            <h3>Sản phẩm dịch vụ</h3>
            <p>Dịch vụ Di động</p>
            <p>Internet - Truyền hình</p>
            <p>Điện thoại - Thiết bị thông minh</p>
            <p>Dịch vụ quốc tế</p>
        </div>
         <div className="linkscolumn">
            <h3>Sản phẩm dịch vụ</h3>
            <p>Dịch vụ Di động</p>
            <p>Internet - Truyền hình</p>
            <p>Điện thoại - Thiết bị thông minh</p>
            <p>Dịch vụ quốc tế</p>
        </div>
         <div className="linkscolumn">
            <h3>Sản phẩm dịch vụ</h3>
            <p>Dịch vụ Di động</p>
            <p>Internet - Truyền hình</p>
            <p>Điện thoại - Thiết bị thông minh</p>
            <p>Dịch vụ quốc tế</p>
        </div>
         <div className="linkscolumn">
            <h3>Sản phẩm dịch vụ</h3>
            <p>Dịch vụ Di động</p>
            <p>Internet - Truyền hình</p>
            <p>Điện thoại - Thiết bị thông minh</p>
            <p>Dịch vụ quốc tế</p>
             <p>Điện thoại - Thiết bị thông minh</p>
        </div>
     </div>
    </div>
    <div className="socialandcopyright">
        <div className="socialbar">
          <Image src={FaceBook} alt="Choose" />
          <Image src={Youtube} alt="Choose" />
        </div>
        <p>Copyright © 2026 FAMU. All rights reserved.</p>
    </div>
</div>


)
}

export default Footer;