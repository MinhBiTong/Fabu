"use client";

import { useParams } from "next/navigation";
import { useEffect } from "react";
import { EmptyState } from "@/components/common/EmptyState";
import { LoadingState } from "@/components/common/LoadingState";
import { useFeedbackStore } from "@/store/feedback.store";

export default function FeedbackDetailsPage() {
  const params = useParams();
  const id = Array.isArray(params.id) ? params.id[0] : params.id;
  const { activeFeedback, loadFeedback, isLoading, error } = useFeedbackStore();

  useEffect(() => {
    if (id) loadFeedback(id);
  }, [id, loadFeedback]);

  if (isLoading) return <LoadingState label="Loading feedback..." />;
  if (error) return <EmptyState title="Could not load feedback" description={error} />;
  if (!activeFeedback) return <EmptyState title="Feedback not found" />;

  return (
    <section className="fabu-section">
      <div className="fabu-container grid gap-6">
        <div>
          <h1>{activeFeedback.email || "Anonymous"} - Feedback</h1>
          <p className="mt-2 text-sm text-fabu-gray">
            Rating: {activeFeedback.rating} / 5
          </p>
        </div>

        <div className="fabu-card">
          <h2 className="text-2xl">{activeFeedback.subject || "Feedback content"}</h2>
          <p className="mt-4 text-sm leading-7 text-fabu-charcoal">
            {activeFeedback.message || activeFeedback.content || "No message"}
          </p>
        </div>
      </div>
    </section>
  );
}
