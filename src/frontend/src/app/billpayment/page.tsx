"use client";

import { useSearchParams, useRouter } from "next/navigation";
import "../../styles/css/billpayment.css";

export default function BillPayment() {
  const params = useSearchParams();
  const router = useRouter();

  const phone = params.get("phone");
  const amount = params.get("amount");
  const transactionId = params.get("id");

  return (
    <div className="bill-wrapper">
      <div className="bill-card">
        
        {/* ICON */}
        <div className="success-icon">
          ✓
        </div>

        {/* TITLE */}
        <h2 className="bill-title">Giao dịch thành công</h2>

        {/* INFO */}
        <div className="bill-info">
          <p>
            Mã giao dịch: 
            <span className="highlight"> #{transactionId || "260322001113561"}</span>
          </p>

          <p>
            Nạp ĐT: 
            <span className="highlight"> {phone}</span>
          </p>
        </div>

        {/* AMOUNT */}
        <div className="bill-amount">
          {Number(amount || 0).toLocaleString("vi-VN")}đ
        </div>

        {/* ACTIONS */}
        <div className="bill-actions">
          <button onClick={() => router.push("/")}>
            Đóng
          </button>

          <button
            className="primary"
            onClick={() => router.push("/recharge")}
          >
            Thanh toán thêm
          </button>
        </div>
      </div>
    </div>
  );
}