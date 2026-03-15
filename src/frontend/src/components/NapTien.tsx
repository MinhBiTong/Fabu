"use client";

import { useState } from "react";
import "../styles/nap-tien.css";

export default function NapTien() {

  const [phone, setPhone] = useState("");
  const [amount, setAmount] = useState<number | null>(null);
  const [error, setError] = useState("");

  const moneyList = [
    10000,
    20000,
    50000,
    100000,
    200000,
    500000
  ];

  const handleNapTien = () => {

    if (!phone) {
      setError("Vui lòng nhập số điện thoại");
      return;
    }

    setError("");
    alert("Nạp tiền thành công!");
  };

  return (
    <div className="nap-wrapper">

      <h1 className="nap-title">Nạp tiền điện thoại</h1>

      <div className="nap-layout">

        {/* LEFT */}
        <div className="nap-left">

          <label>Số điện thoại:</label>

          <input
            type="text"
            placeholder="Nhập số điện thoại"
            className="nap-input"
            value={phone}
            onChange={(e) => {
              setPhone(e.target.value);
              setError("");
            }}
          />

          {error && <p className="input-error">{error}</p>}

          <label>Chọn mệnh giá:</label>

          <div className="money-grid">

            {moneyList.map((money) => (
              <button
                key={money}
                onClick={() => setAmount(money)}
                className={`money-btn ${amount === money ? "active" : ""}`}
              >
                {money.toLocaleString()}đ
              </button>
            ))}

          </div>

        </div>

        {/* RIGHT */}
        <div className="nap-right">

          <div className="payment-box">

            <div className="payment-row total">
              <span>Tổng tiền:</span>
              <span>
                {amount ? amount.toLocaleString() + "đ" : "0đ"}
              </span>
            </div>

            <button
              className="nap-submit"
              onClick={handleNapTien}
              disabled={!amount}
            >
              Nạp tiền
            </button>

            <p className="payment-note">
              Nhấn vào "Nạp tiền" đồng nghĩa với việc bạn đồng ý
              với điều khoản mua hàng.
            </p>

          </div>

        </div>

      </div>

    </div>
  );
}