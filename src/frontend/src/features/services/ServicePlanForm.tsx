"use client";

import type { ReactNode } from "react";
import type { ServicePlanPayload } from "@/core/types/api.types";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";

type FormValues = {
  serviceName: string;
  serviceCode: string;
  category: string;
  dataAmountMB: string | number;
  price: string | number;
  validityDays: string | number;
  description: string;
  maxActivationsPerMonth: string | number;
  isAutoRenew: boolean;
  isActive: boolean;
};

type ServicePlanFormProps = {
  value: FormValues;
  errors?: Partial<Record<keyof FormValues, string>>;
  isSubmitting?: boolean;
  submitLabel: string;
  onChange: (field: keyof FormValues, value: string | number | boolean) => void;
  onSubmit: () => void;
};

export const emptyServicePlanForm: FormValues = {
  serviceName: "",
  serviceCode: "",
  category: "",
  dataAmountMB: "",
  price: "",
  validityDays: "",
  description: "",
  maxActivationsPerMonth: "1",
  isAutoRenew: false,
  isActive: true,
};

export function toServicePlanPayload(value: FormValues): ServicePlanPayload {
  return {
    serviceName: String(value.serviceName),
    serviceCode: String(value.serviceCode),
    category: String(value.category),
    dataAmountMB: Number(value.dataAmountMB),
    price: Number(value.price),
    validityDays: Number(value.validityDays),
    description: String(value.description),
    maxActivationsPerMonth: Number(value.maxActivationsPerMonth),
    isAutoRenew: Boolean(value.isAutoRenew),
    isActive: Boolean(value.isActive),
  };
}

export function ServicePlanForm({
  value,
  errors,
  isSubmitting,
  submitLabel,
  onChange,
  onSubmit,
}: ServicePlanFormProps) {
  return (
    <div className="fabu-card grid gap-4">
      <div className="grid gap-4 md:grid-cols-2">
        <Field label="Service name" error={errors?.serviceName}>
          <Input
            value={value.serviceName}
            onChange={(event) => onChange("serviceName", event.target.value)}
            placeholder="5G Super"
          />
        </Field>
        <Field label="Service code" error={errors?.serviceCode}>
          <Input
            value={value.serviceCode}
            onChange={(event) => onChange("serviceCode", event.target.value)}
            placeholder="ABC123"
          />
        </Field>
        <Field label="Category" error={errors?.category}>
          <Input
            value={value.category}
            onChange={(event) => onChange("category", event.target.value)}
            placeholder="5G"
          />
        </Field>
        <Field label="Data amount (MB)" error={errors?.dataAmountMB}>
          <Input
            type="number"
            value={value.dataAmountMB}
            onChange={(event) => onChange("dataAmountMB", event.target.value)}
          />
        </Field>
        <Field label="Price" error={errors?.price}>
          <Input
            type="number"
            value={value.price}
            onChange={(event) => onChange("price", event.target.value)}
          />
        </Field>
        <Field label="Validity days" error={errors?.validityDays}>
          <Input
            type="number"
            value={value.validityDays}
            onChange={(event) => onChange("validityDays", event.target.value)}
          />
        </Field>
        <Field label="Max activations/month" error={errors?.maxActivationsPerMonth}>
          <Input
            type="number"
            value={value.maxActivationsPerMonth}
            onChange={(event) => onChange("maxActivationsPerMonth", event.target.value)}
          />
        </Field>
        <div className="grid gap-3 rounded border border-fabu-border p-4">
          <label className="flex min-h-11 items-center gap-3 text-sm">
            <input
              type="checkbox"
              checked={value.isAutoRenew}
              onChange={(event) => onChange("isAutoRenew", event.target.checked)}
            />
            Auto renew
          </label>
          <label className="flex min-h-11 items-center gap-3 text-sm">
            <input
              type="checkbox"
              checked={value.isActive}
              onChange={(event) => onChange("isActive", event.target.checked)}
            />
            Active
          </label>
        </div>
      </div>
      <Field label="Description" error={errors?.description}>
        <textarea
          className="min-h-32 w-full rounded border border-fabu-border px-4 py-3 text-sm outline-none focus:border-fabu-red focus:shadow-[0_0_0_2px_rgba(238,0,51,0.1)]"
          value={value.description}
          onChange={(event) => onChange("description", event.target.value)}
        />
      </Field>
      <Button className="w-full sm:w-fit" onClick={onSubmit} disabled={isSubmitting}>
        {isSubmitting ? "Saving..." : submitLabel}
      </Button>
    </div>
  );
}

function Field({
  label,
  error,
  children,
}: {
  label: string;
  error?: string;
  children: ReactNode;
}) {
  return (
    <label className="grid gap-1.5">
      <span className="fabu-label">{label}</span>
      {children}
      {error ? <span className="fabu-error">{error}</span> : null}
    </label>
  );
}
