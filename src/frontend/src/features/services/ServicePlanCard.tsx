import Link from "next/link";
import { memo } from "react";
import { Button } from "@/components/ui/button";
import type { ServicePlan } from "@/core/types/api.types";
import { formatCurrency } from "@/lib/utils/format";

type ServicePlanCardProps = {
  plan: ServicePlan;
  detailsHref: string;
};

function ServicePlanCardComponent({ plan, detailsHref }: ServicePlanCardProps) {
  return (
    <article className="overflow-hidden rounded-card border border-fabu-border bg-white shadow-elevated transition hover:border-fabu-red hover:shadow-prominent">
      <div className="bg-fabu-red px-5 py-4">
        <h3 className="text-xl text-white">{plan.serviceName}</h3>
        <p className="mt-1 text-xs font-bold uppercase text-white/80">{plan.serviceCode}</p>
      </div>
      <div className="grid gap-4 p-5">
        <div className="grid grid-cols-2 gap-3 text-sm">
          <div>
            <span className="text-fabu-gray">Data</span>
            <p className="font-bold text-fabu-ink">{plan.dataAmountMB.toLocaleString()} MB</p>
          </div>
          <div>
            <span className="text-fabu-gray">Validity</span>
            <p className="font-bold text-fabu-ink">{plan.validityDays} days</p>
          </div>
        </div>
        <div>
          <span className="text-sm text-fabu-gray">Price</span>
          <p className="text-2xl font-bold text-fabu-red">{formatCurrency(plan.price)}</p>
        </div>
        <div className="flex items-center justify-between gap-3">
          <Button>Subscribe</Button>
          <Button asChild variant="link">
            <Link href={detailsHref}>View details</Link>
          </Button>
        </div>
      </div>
    </article>
  );
}

export const ServicePlanCard = memo(ServicePlanCardComponent);
