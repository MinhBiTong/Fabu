import "../styles/homepage.css";
import Link from "next/link";

export default function HomePage() {
  return (
    <div className="homepage">
      <section className="home">
        <div className="services">

          <Link href="/recharge" className="service-card">
            <span>📱</span>
            <p>Nạp tiền điện thoại</p>
            <p>Mobile Recharge</p>
          </Link>

         <div className="service-card">
            <span>💳</span>
            <p>Thanh toán hóa đơn</p>
         </div>

           <Link href="/P5GDataPlan" className="service-card">
            <span>📊</span>
            <p>Mua gói data</p>
           </Link>

          <div className="service-card">
            <span>🧾</span>
            <p>Transaction History</p>
          </div>

          <Link  href="/Profile" className="service-card">
            <span>👤</span>
            <p>Tài khoản</p>
          </Link>

        </div>
      </section>


      {/* Promotions */}
      <section className="promotions">

        <h2>Featured Promotions</h2>

        <div className="promo-grid">

          <div className="promo-card">
            <h3>Bonus Recharge</h3>
            <p>Get extra balance when recharging.</p>
          </div>

          <div className="promo-card">
            <h3>Data Discount</h3>
            <p>Discount on 5G data packages.</p>
          </div>

          <div className="promo-card">
            <h3>Loyalty Points</h3>
            <p>Earn points for every transaction.</p>
          </div>

        </div>

      </section>


      {/* Service Categories */}
      <section className="categories">

        <h2>Diverse Products & Services</h2>

        <div className="category-grid">

          <div className="category-card">
            <div className="img-box"></div>
            <h3>Mobile</h3>
            <p>Recharge and mobile data services</p>
          </div>

          <div className="category-card">
            <div className="img-box"></div>
            <h3>Internet</h3>
            <p>High-speed internet services</p>
          </div>

          <div className="category-card">
            <div className="img-box"></div>
            <h3>Devices</h3>
            <p>Smart electronic devices</p>
          </div>

          <div className="category-card">
            <div className="img-box"></div>
            <h3>Enterprise</h3>
            <p>Solutions for businesses</p>
          </div>

        </div>

      </section>


      {/* SIM Section */}
      <section className="sim-section">

        <h2>Beautiful SIM Numbers Nationwide</h2>

        <div className="sim-grid">

          <div className="sim-big"></div>

          <div className="sim-small-group">
            <div className="sim-small"></div>
            <div className="sim-small"></div>
          </div>

        </div>

      </section>


      {/* Internet Packages */}
      <section className="packages">

        <h2>Internet Packages</h2>

        <div className="package-grid">

          <div className="package-card">
            <h3>Basic</h3>
            <p>300 Mbps</p>
            <span>250,000 VND / month</span>
            <button>View Details</button>
          </div>

          <div className="package-card">
            <h3>Standard</h3>
            <p>500 Mbps</p>
            <span>289,000 VND / month</span>
            <button>View Details</button>
          </div>

          <div className="package-card">
            <h3>Premium</h3>
            <p>1 Gbps</p>
            <span>359,000 VND / month</span>
            <button>View Details</button>
          </div>

        </div>

      </section>


      {/* Devices */}
      <section className="devices">

        <h2>Smart Devices</h2>

        <div className="device-grid">

          <div className="device-card">
            <div className="device-img"></div>
            <p>Smartphone</p>
            <h3>1.999.000 VND</h3>
          </div>

          <div className="device-card">
            <div className="device-img"></div>
            <p>Tablet</p>
            <h3>1.999.000 VND</h3>
          </div>

          <div className="device-card">
            <div className="device-img"></div>
            <p>Smart Watch</p>
            <h3>1.999.000 VND</h3>
          </div>

          <div className="device-card">
            <div className="device-img"></div>
            <p>Camera</p>
            <h3>1.999.000 VND</h3>
          </div>

          <div className="device-card">
            <div className="device-img"></div>
            <p>AirPods</p>
            <h3>799.000 VND</h3>
          </div>

          <div className="device-card">
            <div className="device-img"></div>
            <p>Laptop</p>
            <h3>12.999.000 VND</h3>
          </div>

          <div className="device-card">
            <div className="device-img"></div>
            <p>WiFi Router</p>
            <h3>599.000 VND</h3>
          </div>

          <div className="device-card">
            <div className="device-img"></div>
            <p>AirPods</p>
            <h3>12.999.000 VND</h3>
          </div>

          <div className="device-card">
            <div className="device-img"></div>
            <p>Laptop</p>
            <h3>599.000 VND</h3>
          </div>

          <div className="device-card">
            <div className="device-img"></div>
            <p>WiFi Router</p>
            <h3>599.000 VND</h3>
          </div>

        </div>

      </section>


      {/* Customer Care */}
      <section className="customer-care">

        <h2>Customer Care</h2>

        <div className="care-grid">

          <div className="care-card">
            <h3>Reward Points</h3>
            <p>Earn points when making transactions</p>
          </div>

          <div className="care-card">
            <h3>Special Offers</h3>
            <p>Various discount programs</p>
          </div>

          <div className="care-card">
            <h3>Support</h3>
            <p>24/7 customer support</p>
          </div>

        </div>

      </section>

    </div>
  );
}