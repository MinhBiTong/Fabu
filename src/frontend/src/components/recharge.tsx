"use client";

import { useState } from "react";
import "../styles/recharge.css";

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

    const phoneRegex = /^[0-9]+$/;

    if (!phone) {
      setError("Please enter your phone number");
      return;
    }

    if (!phoneRegex.test(phone)) {
      setError("Phone number must contain only digits");
      return;
    }

    if (phone.length > 10) {
      setError("Phone number must not exceed 10 digits");
      return;
    }

    if (phone.length < 10) {
      setError("Phone number must be exactly 10 digits");
      return;
    }

    setError("");
    alert("Recharge successful!");
  };

  return (
    <div className="nap-wrapper">

      <h1 className="nap-title">Mobile Recharge</h1>

      <div className="nap-layout">

        {/* LEFT */}
        <div className="nap-left">

          <label>Phone Number:</label>

          <input
            type="text"
            placeholder="Enter phone number"
            className="nap-input"
            value={phone}
            onChange={(e) => {
              setPhone(e.target.value);
              setError("");
            }}
          />

          {error && <p className="input-error">{error}</p>}

          <label>Select Amount:</label>

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
              <span>Total:</span>
              <span>
                {amount ? amount.toLocaleString() + "đ" : "0đ"}
              </span>
            </div>

            <button
              className="nap-submit"
              onClick={handleNapTien}
              disabled={!amount}
            >
              Recharge
            </button>

            <p className="payment-note">
              By clicking "Recharge", you agree to the terms and conditions.
            </p>

          </div>

        </div>

      </div>

    </div>
  );
}