import { RechargeForm } from "@/features/recharge/RechargeForm";

export default function RechargePage() {
  return (
    <section className="fabu-section">
      <div className="fabu-container">
        <div className="mb-8">
          <h1>Mobile Recharge</h1>
          <p className="mt-2 text-sm text-fabu-gray">
            Recharge plans are loaded from `RechargePlans/active`; transactions are posted to
            `v1/Transaction/recharge`.
          </p>
        </div>
        <RechargeForm />
      </div>
    </section>
  );
}
