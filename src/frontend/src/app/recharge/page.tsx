"use client";
import { useState, useEffect } from "react";
import "../../styles/css/recharge.css";
import Payment from "./Payment";
import { useRouter } from "next/navigation";
import { getPackages, rechargeApi } from "../../services/recharge-service";

export default function Recharge() {
  const [phone, setPhone] = useState("");
  const [amount, setAmount] = useState<number | null>(null);
  const [customAmount, setCustomAmount] = useState("");
  const [selectedCoupon, setSelectedCoupon] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const [finalPrice, setFinalPrice] = useState(0);
  const [moneyList, setMoneyList] = useState<any[]>([]);

  const router = useRouter();

  const coupons = [
    { code: "SALE10", discount: 0.1 },
    { code: "SALE5", discount: 0.05 },
  ];

  // 🔥 CALL API LẤY GÓI TIỀN
  useEffect(() => {
    const fetchPackages = async () => {
      try {
        const res = await getPackages();
          setMoneyList(res.data || []);
      } catch (err) {
        console.error(err);
      }
    };

    fetchPackages();
  }, []);

  const getCarrier = (phone: string) => {
    if (phone.startsWith("03")) return "Viettel";
    if (phone.startsWith("05")) return "Vietnammobile";
    if (phone.startsWith("07")) return "Mobifone";
    if (phone.startsWith("08")) return "Vinaphone";
    if (phone.startsWith("09")) return "Viettel";
    return "Unknown";
  };

  // 🔥 CALL API THANH TOÁN
  const handleRecharge = async () => {
    const phoneRegex = /^(03|05|07|08|09)[0-9]{8}$/;

    if (!phone) return setError("Please enter your phone number");
    if (!phoneRegex.test(phone)) return setError("Invalid phone number");
    if (!amount) return setError("Please select an amount");

    setError("");
    setLoading(true);

    try {
      const final = finalPrice || amount;

      const res = await rechargeApi({
        phone,
        amount: final,
        coupon: selectedCoupon,
      });

      const data = res.data;

      // 👉 chuyển sang bill
      router.push(
        `/billpayment?phone=${data.phone}&amount=${data.amount}&id=${data.transactionId}`
      );
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="nap-wrapper">
      <h1 className="nap-title">Mobile Recharge</h1>

      <div className="nap-layout">
        {/* LEFT */}
        <div className="nap-left">
          <input
            type="text"
            placeholder="Enter phone number..."
            className="nap-input"
            value={phone}
            onChange={(e) => setPhone(e.target.value)}
          />

          {phone.length >= 2 && (
            <p className="carrier">Carrier: {getCarrier(phone)}</p>
          )}

          {error && <p className="input-error">{error}</p>}

          <input
            type="number"
            placeholder="Enter the amount..."
            className="nap-input"
            value={customAmount}
            onChange={(e) => {
              setCustomAmount(e.target.value);
              setAmount(Number(e.target.value));
            }}
          />

          <label>Select Amount:</label>

          <div className="money-grid">
            {moneyList.map((item) => (
              <div key={item.value} className="money-item">
                <button
                  onClick={() => {
                    setAmount(item.value);
                    setCustomAmount("");
                  }}
                  className={`money-btn ${
                    amount === item.value ? "active" : ""
                  }`}
                >
                  <span className="money-btn__value">
                    {item.value.toLocaleString("vi-VN")} VND
                  </span>

                  {item.discount > 0 && (
                    <span className="money-btn__badge">
                      🪙 -{item.discount * 100}%
                    </span>
                  )}
                </button>
              </div>
            ))}
          </div>
        </div>

        {/* RIGHT */}
        <div className="nap-right">
          <Payment
            phone={phone}
            amount={amount}
            selectedCoupon={selectedCoupon}
            onFinalChange={setFinalPrice}
          />

          <div className="payment-box">
            <div className="payment-row total">
              <span>Total:</span>
              <span>
                {finalPrice
                  ? finalPrice.toLocaleString("vi-VN") + " VND"
                  : "0 VND"}
              </span>
            </div>

            <button
              className={`nap-submit ${amount ? "active" : ""}`}
              onClick={handleRecharge}
              disabled={!amount || loading}
            >
              {loading ? "Processing..." : "Recharge"}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}