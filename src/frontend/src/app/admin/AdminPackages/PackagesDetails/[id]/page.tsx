"use client";

import { useRouter } from "next/navigation";
import { useParams } from "next/navigation";
import { useEffect, useMemo, useState } from "react";
import { Button } from "@/components/ui/button";
import { EmptyState } from "@/components/common/EmptyState";
import { LoadingState } from "@/components/common/LoadingState";
import {
  ServicePlanForm,
  toServicePlanPayload,
  emptyServicePlanForm,
} from "@/features/services/ServicePlanForm";
import { serviceSchema } from "@/core/validations/service.schema";
import { useServicePlanStore } from "@/store/service-plan.store";
import { toastError, toastSuccess } from "@/services/toast-service";
import { formatCurrency } from "@/lib/utils/format";

type FormState = typeof emptyServicePlanForm;

function planToForm(plan: NonNullable<ReturnType<typeof useServicePlanStore.getState>["activePlan"]>): FormState {
  return {
    serviceName: plan.serviceName,
    serviceCode: plan.serviceCode,
    category: plan.category,
    dataAmountMB: plan.dataAmountMB,
    price: plan.price,
    validityDays: plan.validityDays,
    description: plan.description,
    maxActivationsPerMonth: plan.maxActivationsPerMonth,
    isAutoRenew: plan.isAutoRenew,
    isActive: plan.isActive,
  };
}

export default function PackagesDetailsPage() {
  const router = useRouter();
  const params = useParams();
  const { activePlan, loadPlan, updatePlan, deletePlan, isLoading, error } = useServicePlanStore();
  const id = Array.isArray(params.id) ? params.id[0] : params.id;
  const [isEditing, setIsEditing] = useState(false);
  const [form, setForm] = useState<FormState | null>(null);
  const [errors, setErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    if (id) loadPlan(id);
  }, [id, loadPlan]);

  const details = useMemo(() => {
    if (!activePlan) return [];
    return [
      ["Service Name", activePlan.serviceName],
      ["Service Code", activePlan.serviceCode],
      ["Price", formatCurrency(activePlan.price)],
      ["Duration", `${activePlan.validityDays} days`],
      ["Amount", `${activePlan.dataAmountMB.toLocaleString()} MB`],
      ["Category", activePlan.category],
      ["Active", activePlan.isActive ? "Yes" : "No"],
      ["Auto Renew", activePlan.isAutoRenew ? "Yes" : "No"],
    ];
  }, [activePlan]);

  const handleUpdate = async () => {
    const formValue = form ?? (activePlan ? planToForm(activePlan) : null);
    if (!id || !formValue) return;
    const result = serviceSchema.safeParse(formValue);
    if (!result.success) {
      const fieldErrors: Record<string, string> = {};
      result.error.issues.forEach((issue) => {
        fieldErrors[String(issue.path[0])] = issue.message;
      });
      setErrors(fieldErrors);
      return;
    }

    const updated = await updatePlan(id, toServicePlanPayload(formValue));
    if (updated) {
      toastSuccess("Package updated");
      setIsEditing(false);
    } else {
      toastError("Could not update package");
    }
  };

  const handleDelete = async () => {
    if (!id || !window.confirm("Delete this package?")) return;
    const deleted = await deletePlan(id);
    if (deleted) {
      toastSuccess("Package deleted");
      router.push("/admin/AdminPackages");
    } else {
      toastError("Could not delete package");
    }
  };

  if (isLoading && !activePlan) return <LoadingState label="Loading package..." />;
  if (error) return <EmptyState title="Could not load package" description={error} />;
  if (!activePlan) return <EmptyState title="Package not found" />;

  const formValue = form ?? planToForm(activePlan);

  return (
    <section className="fabu-section">
      <div className="fabu-container grid gap-6">
        <div className="flex flex-col gap-4 md:flex-row md:items-end md:justify-between">
          <div>
            <h1>Data Plan</h1>
            <p className="mt-2 text-sm text-fabu-gray">{activePlan.description}</p>
          </div>
          <div className="flex gap-3">
            <Button variant="destructive" onClick={handleDelete}>
              Delete
            </Button>
            <Button
              variant="outline"
              onClick={() => {
                setForm(planToForm(activePlan));
                setIsEditing((value) => !value);
              }}
            >
              {isEditing ? "Cancel" : "Edit"}
            </Button>
          </div>
        </div>

        {isEditing ? (
          <ServicePlanForm
            value={formValue}
            errors={errors}
            isSubmitting={isLoading}
            submitLabel="Save"
            onChange={(field, value) => setForm((current) => current && { ...current, [field]: value })}
            onSubmit={handleUpdate}
          />
        ) : (
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
        )}
      </div>
    </section>
  );
}
