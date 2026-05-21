import { z } from "zod";

export const serviceSchema = z.object({
  serviceName: z.string().min(3, "Service name must be at least 3 characters"),

  serviceCode: z
    .string()
    .regex(/^[A-Z]{3}\d{3}$/, "Service code must be 3 uppercase letters followed by 3 numbers (e.g. ABC123)"),

  category: z.string().min(2, "Category is required"),

  dataAmountMB: z.coerce.number().min(1, "Data amount must be greater than 0"),

  price: z.coerce.number().min(1, "Price must be > 0"),

  validityDays: z.coerce.number().min(1, "Validity must be at least 1 day"),

  description: z.string().min(5, "Description must be at least 5 characters"),

  maxActivationsPerMonth: z.coerce.number().min(1, "Must be at least 1"),
});

export type ServiceSchema = z.infer<typeof serviceSchema>;