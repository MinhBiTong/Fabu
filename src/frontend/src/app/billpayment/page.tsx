"use client";

import { useSearchParams, useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import "../../styles/css/billpayment.css";

export default function BillPayment() {
  const params = useSearchParams();
  const router = useRouter();

  const phone = params.get("phone");
  const amount = params.get("amount");
  const transactionId = params.get("id");

  const [loading, setLoading] = useState(true);
  const [success, setSuccess] = useState(false);

  const handlePayment = async () => {
    try {
      // 👉 fake API (nếu chưa có backend)
      await new Promise((res) => setTimeout(res, 1500));

      setSuccess(true);
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    handlePayment();
  }, []);

  if (loading || !success) {
  return <h2 style={{ textAlign: "center" }}>Processing payment...</h2>;
}

  return (
    <div className="bill-wrapper">
      <div className="bill-card">
        <div className="success-icon">✓</div>

        <h2>Transaction Successful</h2>

        <p>Transaction ID: #{transactionId}</p>
        <p>Phone: {phone}</p>

        <h3>{Number(amount).toLocaleString("vi-VN")} VND</h3>

        <button onClick={() => router.replace("/")}>
            Done
        </button>
      </div>
    </div>
  );
}