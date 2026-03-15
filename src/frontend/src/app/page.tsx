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
          </Link>

          <div className="service-card">
            <span>💳</span>
            <p>Thanh toán hóa đơn</p>
          </div>

          <div className="service-card">
            <span>📊</span>
            <p>Mua gói data</p>
          </div>

          <div className="service-card">
            <span>🧾</span>
            <p>Lịch sử giao dịch</p>
          </div>

          <div className="service-card">
            <span>👤</span>
            <p>Tài khoản</p>
          </div>

        </div>

      </section>


      {/* Promotions */}
      <section className="promotions">

        <h2>Ưu đãi nổi bật</h2>

        <div className="promo-grid">

          <div className="promo-card">
            <h3>Bonus Recharge</h3>
            <p>Nhận thêm tiền khi nạp.</p>
          </div>

          <div className="promo-card">
            <h3>Data Discount</h3>
            <p>Giảm giá gói data 5G.</p>
          </div>

          <div className="promo-card">
            <h3>Loyalty Points</h3>
            <p>Tích điểm cho mỗi giao dịch.</p>
          </div>

        </div>

      </section>

      {/* Service Categories */}

      <section className="categories">

        <h2>Sản phẩm dịch vụ đa dạng</h2>

        <div className="category-grid">

          <div className="category-card">
            <div className="img-box"></div>
            <h3>Di động</h3>
            <p>Các dịch vụ nạp tiền và data</p>
          </div>

          <div className="category-card">
            <div className="img-box"></div>
            <h3>Internet</h3>
            <p>Internet tốc độ cao</p>
          </div>

          <div className="category-card">
            <div className="img-box"></div>
            <h3>Thiết bị</h3>
            <p>Thiết bị thông minh</p>
          </div>

          <div className="category-card">
            <div className="img-box"></div>
            <h3>Doanh nghiệp</h3>
            <p>Giải pháp cho doanh nghiệp</p>
          </div>

        </div>

      </section>



      {/* SIM Section */}

      <section className="sim-section">

        <h2>SIM đẹp phủ sóng toàn quốc</h2>

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

        <h2>Gói cước Internet</h2>

        <div className="package-grid">

          <div className="package-card">
            <h3>Basic</h3>
            <p>300 Mbps</p>
            <span>250.000đ / tháng</span>
            <button>Xem chi tiết</button>
          </div>

          <div className="package-card">
            <h3>Standard</h3>
            <p>500 Mbps</p>
            <span>289.000đ / tháng</span>
            <button>Xem chi tiết</button>
          </div>

          <div className="package-card">
            <h3>Premium</h3>
            <p>1 Gbps</p>
            <span>359.000đ / tháng</span>
            <button>Xem chi tiết</button>
          </div>

        </div>

      </section>



      {/* Devices */}

      <section className="devices">

        <h2>Thiết bị thông minh</h2>

        <div className="device-grid">

          <div className="device-card">
            <div className="device-img"></div>
            <p>Smartphone</p>
            <h3>1.999.000đ</h3>
          </div>

          <div className="device-card">
            <div className="device-img"></div>
            <p>Tablet</p>
            <h3>1.999.000đ</h3>
          </div>

          <div className="device-card">
            <div className="device-img"></div>
            <p>Smart Watch</p>
            <h3>1.999.000đ</h3>
          </div>

          <div className="device-card">
            <div className="device-img"></div>
            <p>Camera</p>
            <h3>1.999.000đ</h3>
          </div>

          <div className="device-card">
            <div className="device-img"></div>
            <p>AirPods</p>
            <h3>799.000đ</h3>
          </div>

          <div className="device-card">
            <div className="device-img"></div>
            <p>Laptop</p>
            <h3>12.999.000đ</h3>
          </div>

          <div className="device-card">
            <div className="device-img"></div>
            <p>Router Wifi</p>
            <h3>599.000đ</h3>
          </div>

          <div className="device-card">
            <div className="device-img"></div>
            <p>AirPods</p>
            <h3>799.000đ</h3>
          </div>

          <div className="device-card">
            <div className="device-img"></div>
            <p>Laptop</p>
            <h3>12.999.000đ</h3>
          </div>

          <div className="device-card">
            <div className="device-img"></div>
            <p>Router Wifi</p>
            <h3>599.000đ</h3>
          </div>

        </div>

      </section>



      {/* Customer Care */}

      <section className="customer-care">

        <h2>Chăm sóc khách hàng</h2>

        <div className="care-grid">

          <div className="care-card">
            <h3>Tích điểm</h3>
            <p>Nhận điểm thưởng khi giao dịch</p>
          </div>

          <div className="care-card">
            <h3>Ưu đãi</h3>
            <p>Nhiều chương trình giảm giá</p>
          </div>

          <div className="care-card">
            <h3>Hỗ trợ</h3>
            <p>Hỗ trợ khách hàng 24/7</p>
          </div>

        </div>

      </section>

      {/* More Packages */}

    </div>
  );
}