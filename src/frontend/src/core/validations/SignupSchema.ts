import { z } from "zod";

export const signupSchema = z.object({
  Email: z.string().email("Invalid email"),
  Username: z.string().min(3, "Username must be at least 3 characters"),
  PhoneNumber: z.string().min(9, "Invalid phone number"),
//   birthDate: z.string().nonempty("Birth date is required"),
  Password: z.string().min(6, "Password must be at least 6 characters"),
  confirmPassword: z.string()
}).refine((data) => data.Password === data.confirmPassword, {
  message: "Passwords do not match",
  path: ["confirmPassword"]
});