import { z } from "zod";

//dinh nghia schema validate form login
export const authSchema = z.object({
    email: z
    .string()
    .min(1, "Email is required")
    .email("Invalid email address"),
    password: z
    .string()
    .min(6, "Password must be at least 6 characters")
    .max(20, "Password must be at most 20 characters")
});

//tu dong tao type cho form login tu schema
export type LoginFormData = z.infer<typeof authSchema>;