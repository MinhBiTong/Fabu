import { z } from "zod";

export const feedbackSchema = z.object({
  subject: z.string().min(1, "Subject is required"),
  message: z.string().min(1, "Message is required"),
  rating: z.number().min(1, "Please select a rating").max(5)
});