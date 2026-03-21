"use client";
import { useEffect, useState } from "react";
import "../../styles/css/payment.css";

type Props = {
  phone: string;
  amount: number | null;
  selectedCoupon: string;
  onFinalChange: (value: number) => void;
};

export default function Payment({
  phone,
  amount,
  selectedCoupon,
  onFinalChange
}: Props) {
  const [method, setMethod] = useState("");
  const [discount, setDiscount] = useState(0);
  const [final, setFinal] = useState(0);

  const methods = [
    { id: "001", method: "Viettel", label: "Viettel Gate", discount: 0.025 },
    { id: "002", method: "Zalopay", label: "ZaloPay", discount: 0.05 },
  ];

  const couponMap: any = {
    SALE10: 0.1,
    SALE5: 0.05,
  };

  useEffect(() => {
    if (!amount) {
      onFinalChange(0);
      return;
    }

    const methodDiscount =
      methods.find((m) => m.id === method)?.discount || 0;

    const couponDiscount = couponMap[selectedCoupon] || 0;

    const totalDiscount = amount * (methodDiscount + couponDiscount);
    const finalValue = amount - totalDiscount;

    setDiscount(totalDiscount);
    setFinal(finalValue);

    onFinalChange(finalValue);
  }, [amount, method, selectedCoupon]);

  return (
    <div className="payment-container">
      {/* METHODS */}
      {phone && amount && (
        <div className="methods">
          {methods.map((m) => (
            <div
              key={m.id}
              className={`method ${method === m.id ? "active" : ""
                }`}
              onClick={() => setMethod(m.id)}
            >
              {m.label} (-{m.discount * 100}%)
            </div>
          ))}
        </div>
      )}

      {/* SUMMARY */}
      <div className="summary">
        <p>Total: {amount?.toLocaleString("vi-VN")} VND</p>
        <p>Discount: -{discount.toLocaleString("vi-VN")} VND</p>
        <h3>
          Total payment: {final.toLocaleString("vi-VN")} VND
        </h3>
      </div>
    </div>
  );
}