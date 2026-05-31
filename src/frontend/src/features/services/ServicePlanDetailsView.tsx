"use client";

import { useEffect } from "react";
import { useServicePlanStore } from "@/store/service-plan.store";
import { Button } from "@/components/ui/button";
import { EmptyState } from "@/components/common/EmptyState";
import { LoadingState } from "@/components/common/LoadingState";
import { formatCurrency } from "@/lib/utils/format";

type ServicePlanDetailsViewProps = {
  id: string;
};

export function ServicePlanDetailsView({ id }: ServicePlanDetailsViewProps) {
  const { activePlan, loadPlan, isLoading, error } = useServicePlanStore();

  useEffect(() => {
    loadPlan(id);
  }, [id, loadPlan]);

  if (isLoading) return <LoadingState label="Loading plan details..." />;
  if (error) return <EmptyState title="Could not load plan" description={error} />;
  if (!activePlan) return <EmptyState title="Plan not found" />;

  const details = [
    ["Plan name", activePlan.serviceName],
    ["Service code", activePlan.serviceCode],
    ["Price", formatCurrency(activePlan.price)],
    ["Duration", `${activePlan.validityDays} days`],
    ["Amount", `${activePlan.dataAmountMB.toLocaleString()} MB`],
    ["Category", activePlan.category],
    ["Auto renew", activePlan.isAutoRenew ? "Yes" : "No"],
    ["Monthly limit", `${activePlan.maxActivationsPerMonth}`],
  ];

  return (
    <div className="grid gap-8">
      <div>
        <h1>Data Plan</h1>
        <p className="mt-2 text-sm text-fabu-gray">{activePlan.description}</p>
      </div>

      <div className="grid gap-5 lg:grid-cols-[0.8fr_1.2fr]">
        <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-1">
          {details.map(([label, value]) => (
            <div key={label} className="fabu-card p-4">
              <p className="text-xs font-bold uppercase text-fabu-gray">{label}</p>
              <p className="mt-1 text-base font-bold text-fabu-ink">{value}</p>
            </div>
          ))}
        </div>
        <div className="fabu-card">
          <h2 className="text-2xl">Description</h2>
          <p className="mt-4 text-sm leading-7 text-fabu-charcoal">{activePlan.description}</p>
        </div>
      </div>

      <Button className="w-full sm:w-fit">Subscribe</Button>
    </div>
  );
}
