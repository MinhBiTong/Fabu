"use client";

import { useRouter } from "next/navigation";
import { useState } from "react";
import { serviceSchema } from "@/core/validations/service.schema";
import {
  emptyServicePlanForm,
  ServicePlanForm,
  toServicePlanPayload,
} from "@/features/services/ServicePlanForm";
import { useServicePlanStore } from "@/store/service-plan.store";
import { toastError, toastSuccess } from "@/services/toast-service";

export default function AddPackagePage() {
  const router = useRouter();
  const createPlan = useServicePlanStore((state) => state.createPlan);
  const isLoading = useServicePlanStore((state) => state.isLoading);
  const [form, setForm] = useState(emptyServicePlanForm);
  const [errors, setErrors] = useState<Record<string, string>>({});

  const handleSubmit = async () => {
    const result = serviceSchema.safeParse(form);
    if (!result.success) {
      const fieldErrors: Record<string, string> = {};
      result.error.issues.forEach((issue) => {
        fieldErrors[String(issue.path[0])] = issue.message;
      });
      setErrors(fieldErrors);
      return;
    }

    const created = await createPlan(toServicePlanPayload(form));
    if (created) {
      toastSuccess("Package created");
      router.push("/admin/AdminPackages");
    } else {
      toastError("Could not create package");
    }
  };

  return (
    <section className="fabu-section">
      <div className="fabu-container grid gap-6">
        <div>
          <h1>Add New Package</h1>
          <p className="mt-2 text-sm text-fabu-gray">
            Payload matches backend `ServiceCreateRequest`.
          </p>
        </div>
        <ServicePlanForm
          value={form}
          errors={errors}
          isSubmitting={isLoading}
          submitLabel="Add"
          onChange={(field, value) => setForm((current) => ({ ...current, [field]: value }))}
          onSubmit={handleSubmit}
        />
      </div>
    </section>
  );
}
