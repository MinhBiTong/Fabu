import { z } from "zod";

export const rechargeSchema = z.object({
  phone: z.string().regex(/^(03|05|07|08|09)[0-9]{8}$/, "Invalid phone number"),
  amount: z.coerce.number().min(1, "Amount is required"),
  coupon: z.string().optional(),
});

export type RechargeFormData = z.infer<typeof rechargeSchema>;
