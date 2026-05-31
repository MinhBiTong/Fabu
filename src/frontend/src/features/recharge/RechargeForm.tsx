"use client";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { useRecharge } from "@/hooks/use-recharge";
import { formatCurrency } from "@/lib/utils/format";

const paymentMethods = ["VNPay", "PayPal"] as const;

export function RechargeForm() {
  const recharge = useRecharge();

  return (
    <div className="grid gap-6 lg:grid-cols-[1.1fr_0.9fr]">
      <section className="fabu-card">
        <h2 className="text-2xl">Recharge Details</h2>
        <div className="mt-5 grid gap-4">
          <div className="space-y-1.5">
            <label className="fabu-label" htmlFor="phone">
              Phone number
            </label>
            <Input
              id="phone"
              placeholder="0912345678"
              value={recharge.phone}
              onChange={(event) => recharge.setPhone(event.target.value)}
            />
            {recharge.carrier ? (
              <p className="text-sm text-fabu-gray">Carrier: {recharge.carrier}</p>
            ) : null}
          </div>

          <div className="space-y-1.5">
            <label className="fabu-label" htmlFor="custom-amount">
              Custom amount
            </label>
            <Input
              id="custom-amount"
              type="number"
              placeholder="Enter amount"
              value={recharge.customAmount}
              onChange={(event) => recharge.updateCustomAmount(event.target.value)}
            />
          </div>

          <div className="space-y-3">
            <p className="fabu-label">Select amount</p>
            <div className="grid gap-3 sm:grid-cols-2 xl:grid-cols-3">
              {recharge.plans.map((plan) => (
                <button
                  type="button"
                  key={plan.id}
                  className={`min-h-20 rounded-card border p-4 text-left transition ${
                    recharge.amount === plan.price
                      ? "border-fabu-red bg-[rgba(238,0,51,0.06)]"
                      : "border-fabu-border bg-white hover:border-fabu-red"
                  }`}
                  onClick={() => recharge.selectAmount(plan.price)}
                >
                  <span className="text-sm font-bold text-fabu-ink">
                    {formatCurrency(plan.price)}
                  </span>
                  <span className="mt-1 block text-xs text-fabu-gray">{plan.name}</span>
                </button>
              ))}
            </div>
          </div>
        </div>
      </section>

      <aside className="fabu-card h-fit">
        <h2 className="text-2xl">Payment</h2>
        <div className="mt-5 grid gap-4">
          <div className="grid gap-2">
            {paymentMethods.map((method) => (
              <button
                type="button"
                key={method}
                className={`min-h-11 rounded border px-4 text-left text-sm ${
                  recharge.paymentMethod === method
                    ? "border-fabu-red bg-[rgba(238,0,51,0.06)] text-fabu-red"
                    : "border-fabu-border hover:border-fabu-red"
                }`}
                onClick={() => recharge.setPaymentMethod(method)}
              >
                {method}
              </button>
            ))}
          </div>

          <div className="space-y-1.5">
            <label className="fabu-label" htmlFor="coupon">
              Coupon code
            </label>
            <Input
              id="coupon"
              placeholder="Optional"
              value={recharge.coupon}
              onChange={(event) => recharge.setCoupon(event.target.value)}
            />
          </div>

          <div className="rounded bg-fabu-muted p-4">
            <div className="flex justify-between text-sm text-fabu-gray">
              <span>Selected amount</span>
              <span>{formatCurrency(recharge.amount ?? 0)}</span>
            </div>
            <div className="mt-3 flex justify-between text-lg font-bold text-fabu-ink">
              <span>Total</span>
              <span>{recharge.formattedFinalPrice}</span>
            </div>
          </div>

          {recharge.error ? <p className="fabu-error">{recharge.error}</p> : null}

          <Button
            className="w-full"
            onClick={recharge.handleRecharge}
            disabled={!recharge.amount || recharge.isLoading}
          >
            {recharge.isLoading ? "Processing..." : "Recharge"}
          </Button>
        </div>
      </aside>
    </div>
  );
}
