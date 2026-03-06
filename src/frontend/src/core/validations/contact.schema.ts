import { z } from "zod";

export const contactSchema = z.object({
  fullName: z.string().min(2, "Họ tên phải có ít nhất 2 ký tự"),
  email: z.string().email("Email không hợp lệ"),
  subject: z.string().min(5, "Tiêu đề quá ngắn"),
  message: z.string().min(10, "Nội dung phải có ít nhất 10 ký tự"),
});

export type ContactFormValues = z.infer<typeof contactSchema>;