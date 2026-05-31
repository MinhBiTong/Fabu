"use client";

import { Suspense } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import { Button } from "@/components/ui/button";
import { formatCurrency } from "@/lib/utils/format";

function BillPaymentContent() {
  const params = useSearchParams();
  const router = useRouter();

  const phone = params.get("phone") ?? "N/A";
  const amount = Number(params.get("amount") ?? 0);
  const transactionId = params.get("id") ?? "pending";

  return (
    <section className="fabu-section">
      <div className="fabu-container flex min-h-[calc(100vh-220px)] items-center justify-center">
        <div className="w-full max-w-md rounded-card border border-fabu-border bg-white p-8 text-center shadow-modal">
          <div className="mx-auto flex h-16 w-16 items-center justify-center rounded-full bg-[rgba(238,0,51,0.08)] text-3xl font-bold text-fabu-red">
            ✓
          </div>
          <h1 className="mt-5 text-3xl">Transaction Successful</h1>
          <div className="mt-5 grid gap-2 text-sm text-fabu-gray">
            <p>Transaction ID: {transactionId}</p>
            <p>Phone: {phone}</p>
          </div>
          <p className="mt-5 text-2xl font-bold text-fabu-red">{formatCurrency(amount)}</p>
          <Button className="mt-6 w-full" onClick={() => router.replace("/")}>
            Done
          </Button>
        </div>
      </div>
    </section>
  );
}

export default function BillPaymentPage() {
  return (
    <Suspense fallback={null}>
      <BillPaymentContent />
    </Suspense>
  );
}
