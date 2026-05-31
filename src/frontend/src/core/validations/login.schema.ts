import { z } from "zod";

export const loginSchema = z.object({
  email: z.string().min(1, "Email is required").email("Invalid email"),
  password: z.string().min(1, "Password is required").min(6, "Password is too short"),
});

export type LoginFormData = z.infer<typeof loginSchema>;
