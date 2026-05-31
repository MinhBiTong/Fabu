"use client";

import { useMemo, useState } from "react";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm, useWatch } from "react-hook-form";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { feedbackSchema, type Feedback } from "@/core/validations/feedback.schema";
import { useFeedbackStore } from "@/store/feedback.store";
import { toastError, toastSuccess } from "@/services/toast-service";

const stars = [1, 2, 3, 4, 5];

export function ContactFeedbackForm() {
  const [rating, setRating] = useState(0);
  const submitFeedback = useFeedbackStore((state) => state.submitFeedback);
  const isLoading = useFeedbackStore((state) => state.isLoading);

  const form = useForm<Feedback>({
    resolver: zodResolver(feedbackSchema),
    defaultValues: {
      subject: "",
      message: "",
      rating: 0,
    },
  });

  const selectedRating = useWatch({ control: form.control, name: "rating" });
  const activeRating = useMemo(() => selectedRating || rating, [rating, selectedRating]);

  const onSubmit = async (data: Feedback) => {
    const created = await submitFeedback({
      customerId: null,
      subject: data.subject,
      message: data.message,
      rating: data.rating,
    });

    if (created) {
      toastSuccess("Feedback submitted successfully");
      form.reset();
      setRating(0);
    } else {
      toastError("Could not submit feedback");
    }
  };

  return (
    <form className="fabu-card grid gap-5" onSubmit={form.handleSubmit(onSubmit)}>
      <div>
        <h2 className="text-2xl">Send Feedback</h2>
        <p className="mt-2 text-sm text-fabu-gray">
          Feedback posts directly to the backend `Feedbacks` endpoint.
        </p>
      </div>

      <div>
        <p className="fabu-label">Experience rating</p>
        <div className="mt-2 flex gap-2">
          {stars.map((star) => (
            <button
              type="button"
              key={star}
              className={`flex h-11 w-11 items-center justify-center rounded-full border text-sm font-bold ${
                star <= activeRating
                  ? "border-fabu-red bg-fabu-red text-white"
                  : "border-fabu-border bg-white text-fabu-gray"
              }`}
              onClick={() => {
                setRating(star);
                form.setValue("rating", star, { shouldValidate: true });
              }}
            >
              {star}
            </button>
          ))}
        </div>
        {form.formState.errors.rating ? (
          <p className="fabu-error mt-1">{form.formState.errors.rating.message}</p>
        ) : null}
      </div>

      <div className="space-y-1.5">
        <label className="fabu-label" htmlFor="feedback-subject">
          Subject
        </label>
        <Input id="feedback-subject" placeholder="Subject" {...form.register("subject")} />
        {form.formState.errors.subject ? (
          <p className="fabu-error">{form.formState.errors.subject.message}</p>
        ) : null}
      </div>

      <div className="space-y-1.5">
        <label className="fabu-label" htmlFor="feedback-message">
          Message
        </label>
        <textarea
          id="feedback-message"
          className="min-h-36 w-full rounded border border-fabu-border bg-white px-4 py-3 text-sm outline-none transition focus:border-fabu-red focus:shadow-[0_0_0_2px_rgba(238,0,51,0.1)]"
          placeholder="Tell us what should be improved"
          {...form.register("message")}
        />
        {form.formState.errors.message ? (
          <p className="fabu-error">{form.formState.errors.message.message}</p>
        ) : null}
      </div>

      <Button type="submit" className="w-full sm:w-fit" disabled={isLoading}>
        {isLoading ? "Submitting..." : "Submit"}
      </Button>
    </form>
  );
}
