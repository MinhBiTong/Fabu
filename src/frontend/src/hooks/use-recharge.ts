"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { useRouter } from "next/navigation";
import { useRechargeStore } from "@/store/recharge.store";
import { formatCurrency } from "@/lib/utils/format";

const phoneRegex = /^(03|05|07|08|09)[0-9]{8}$/;
const methodDiscounts: Record<string, number> = {
  VNPay: 0.025,
  PayPal: 0.05,
};

export function getCarrier(phone: string) {
  if (phone.startsWith("03") || phone.startsWith("09")) return "Viettel";
  if (phone.startsWith("05")) return "Vietnamobile";
  if (phone.startsWith("07")) return "Mobifone";
  if (phone.startsWith("08")) return "Vinaphone";
  return "Unknown";
}

export const useRecharge = () => {
  const router = useRouter();
  const { plans, loadPlans, submitRecharge, isLoading, error } = useRechargeStore();
  const [phone, setPhone] = useState("");
  const [amount, setAmount] = useState<number | null>(null);
  const [customAmount, setCustomAmount] = useState("");
  const [paymentMethod, setPaymentMethod] = useState("VNPay");
  const [coupon, setCoupon] = useState("");
  const [formError, setFormError] = useState<string | null>(null);

  useEffect(() => {
    loadPlans();
  }, [loadPlans]);

  const finalPrice = useMemo(() => {
    if (!amount) return 0;
    const discount = amount * (methodDiscounts[paymentMethod] ?? 0);
    return Math.max(amount - discount, 0);
  }, [amount, paymentMethod]);

  const formattedFinalPrice = useMemo(() => formatCurrency(finalPrice), [finalPrice]);

  const selectAmount = useCallback((value: number) => {
    setAmount(value);
    setCustomAmount("");
    setFormError(null);
  }, []);

  const updateCustomAmount = useCallback((value: string) => {
    setCustomAmount(value);
    setAmount(value ? Number(value) : null);
    setFormError(null);
  }, []);

  const handleRecharge = useCallback(async () => {
    if (!phone) {
      setFormError("Please enter your phone number");
      return;
    }

    if (!phoneRegex.test(phone)) {
      setFormError("Invalid phone number");
      return;
    }

    if (!amount || amount <= 0) {
      setFormError("Please select an amount");
      return;
    }

    const transaction = await submitRecharge({
      phone,
      amount: finalPrice || amount,
      coupon,
      paymentMethod: paymentMethod === "PayPal" ? 3 : 2,
    });

    if (transaction) {
      router.push(
        `/billpayment?phone=${phone}&amount=${transaction.amount}&id=${transaction.transactionRef}`
      );
    }
  }, [amount, coupon, finalPrice, paymentMethod, phone, router, submitRecharge]);

  return {
    plans,
    phone,
    setPhone,
    amount,
    customAmount,
    updateCustomAmount,
    selectAmount,
    paymentMethod,
    setPaymentMethod,
    coupon,
    setCoupon,
    carrier: phone.length >= 2 ? getCarrier(phone) : null,
    finalPrice,
    formattedFinalPrice,
    isLoading,
    error: formError ?? error,
    handleRecharge,
  };
};
