import { z } from "zod";

export const signupSchema = z
  .object({
    username: z.string().min(3, "Username must be at least 3 characters"),
    fullName: z.string().min(2, "Full name is required"),
    email: z.string().email("Invalid email"),
    phoneNumber: z
      .string()
      .regex(/^(03|05|07|08|09)[0-9]{8}$/, "Invalid Vietnamese phone number"),
    password: z.string().min(6, "Password must be at least 6 characters"),
    confirmPassword: z.string().min(6, "Please confirm your password"),
  })
  .refine((data) => data.password === data.confirmPassword, {
    message: "Passwords do not match",
    path: ["confirmPassword"],
  });

export type SignupFormData = z.infer<typeof signupSchema>;
