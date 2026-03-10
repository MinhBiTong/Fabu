import Image from "next/image";
import logo from "../../styles/images/FABUlogo.png";
import Icon from "../../styles/images/search.png";
import Menu from "../../styles/images/menu.png";
import User from "../../styles/images/user.png";

function Header() {
  return (
    <div className="navibar">

      <div className="menu">
        <Image src={Menu} alt="Menu"  />
      </div>
      
      <div className="logo">
        <Image src={logo} alt="Logo"  />
      </div>

      <div className="navlinks">
        <button>Homasdasde</button>
        <button>Aboasdasdut</button>
        <button>Serasdasdvices</button>
        <button>Contasdasdact</button>
      </div>

      <div className="signin">
      <button className="SearchButton">
        <Image src={Icon} alt="Search" width={20} height={20} />
      </button >
     
        <button className="SigninButton">Sign in</button>
         <button className="ProfileButton">
           <Image src={User} alt="Search"/>
         </button>

      </div>

    </div>
  );
}

export default Header;