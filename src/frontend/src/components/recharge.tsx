"use client";

import { useState } from "react";
import "../styles/recharge.css";

export default function Recharge() {

  const [phone, setPhone] = useState("");
  const [amount, setAmount] = useState<number | null>(null);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const [success, setSuccess] = useState("");

  const moneyList = [
    10000, 20000, 50000, 100000, 200000, 500000
  ];

  // Detect carrier
  const getCarrier = (phone: string) => {
    if (phone.startsWith("03")) return "Viettel";
    if (phone.startsWith("05")) return "Vietnammobile";
    if (phone.startsWith("07")) return "Mobifone";
    if (phone.startsWith("08")) return "Vinaphone";
    if (phone.startsWith("09")) return "Viettel";
    return "Unknown";
  };

  const handleRecharge = () => {

    const phoneRegex = /^(03|05|07|08|09)[0-9]{8}$/;

    if (!phone) {
      setError("Please enter your phone number");
      return;
    }

    if (!phoneRegex.test(phone)) {
      setError("Invalid phone number");
      return;
    }

    if (!amount) {
      setError("Please select an amount");
      return;
    }

    setError("");
    setLoading(true);
    setSuccess("");

    // simulate API
    setTimeout(() => {

      setLoading(false);

      const isSuccess = Math.random() > 0.2;

      if (isSuccess) {
        setSuccess(`Recharge ${amount.toLocaleString("vi-VN")} VND successfully!`);
        setPhone("");
        setAmount(null);
      } else {
        setError("Transaction failed. Please try again.");
      }

    }, 1500);
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
                  setSuccess("");
                }}
          />

          {/* Carrier */}
          {phone.length >= 2 && (
            <p className="carrier">
              Carrier: {getCarrier(phone)}
            </p>
          )}

          {error && <p className="input-error">{error}</p>}
          {success && <p className="success-text">{success}</p>}

          <label>Select Amount:</label>

          <div className="money-grid">

            {moneyList.map((money) => (
              <button
                key={money}
                onClick={() => setAmount(money)}
                className={`money-btn ${amount === money ? "active" : ""}`}
              >
                {money.toLocaleString("vi-VN")} VND
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
                {amount ? amount.toLocaleString("vi-VN") + " VND" : "0 VND"}
              </span>
            </div>

            <button
              className={`nap-submit ${amount ? "active" : ""}`}
              onClick={handleRecharge}
              disabled={!amount || loading}
            >
              {loading ? "Processing..." : "Recharge"}
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