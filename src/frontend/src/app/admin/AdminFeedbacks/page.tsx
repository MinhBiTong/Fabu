"use client";

export const dynamic = "force-dynamic";

import Link from "next/link";
import nextDynamic from "next/dynamic";
import { useEffect, useMemo, useState } from "react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { ChartSkeleton } from "@/components/ui/skeleton";
import { useFeedbackStore } from "@/store/feedback.store";

const itemsPerPage = 12;
const FeedbackStarsChart = nextDynamic(
  () => import("@/features/admin/FeedbackStarsChart").then((module) => module.FeedbackStarsChart),
  { ssr: false, loading: () => <ChartSkeleton /> }
);

export default function AdminFeedbacksPage() {
  const { feedbacks, loadFeedbacks, isLoading, error } = useFeedbackStore();
  const [selectedStar, setSelectedStar] = useState<number | null>(null);
  const [search, setSearch] = useState("");
  const [currentPage, setCurrentPage] = useState(1);

  useEffect(() => {
    loadFeedbacks();
  }, [loadFeedbacks]);

  const chartData = useMemo(() => {
    return [5, 4, 3, 2, 1].map((star) => ({
      star: String(star),
      total: feedbacks.filter((feedback) => feedback.rating === star).length,
    }));
  }, [feedbacks]);

  const filteredFeedbacks = useMemo(() => {
    return feedbacks.filter((feedback) => {
      const matchStar = !selectedStar || feedback.rating === selectedStar;
      const text = `${feedback.email ?? ""} ${feedback.subject ?? ""} ${
        feedback.message ?? feedback.content ?? ""
      }`.toLowerCase();
      return matchStar && text.includes(search.toLowerCase());
    });
  }, [feedbacks, search, selectedStar]);

  const totalPages = Math.max(Math.ceil(filteredFeedbacks.length / itemsPerPage), 1);
  const paginatedFeedbacks = filteredFeedbacks.slice(
    (currentPage - 1) * itemsPerPage,
    currentPage * itemsPerPage
  );

  return (
    <section className="fabu-section">
      <div className="fabu-container grid gap-6">
        <div>
          <h1>Feedbacks</h1>
          <p className="mt-2 text-sm text-fabu-gray">Customer feedback from `Feedbacks`.</p>
        </div>

        <div className="fabu-card">
          <h2 className="mb-4 text-2xl">Stars Chart</h2>
          <div className="h-[260px] min-w-0">
            <FeedbackStarsChart data={chartData} />
          </div>
        </div>

        <div className="grid gap-3 md:grid-cols-[1fr_160px]">
          <Input
            placeholder="Search feedback"
            value={search}
            onChange={(event) => {
              setSearch(event.target.value);
              setCurrentPage(1);
            }}
          />
          <Input
            type="number"
            min={1}
            max={5}
            placeholder="Stars"
            value={selectedStar ?? ""}
            onChange={(event) => {
              const value = Number(event.target.value);
              setSelectedStar(value >= 1 && value <= 5 ? value : null);
              setCurrentPage(1);
            }}
          />
        </div>

        {error ? <p className="fabu-error">{error}</p> : null}

        <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
          {paginatedFeedbacks.map((feedback) => (
            <Link
              key={feedback.id}
              href={`/admin/AdminFeedbacks/FeedbackDetails/${feedback.id}`}
              className="fabu-card"
            >
              <p className="truncate text-sm font-bold text-fabu-ink">
                {feedback.email?.trim() || "Anonymous"}
              </p>
              <p className="mt-3 text-sm text-fabu-gray">{feedback.rating} / 5 stars</p>
              <p className="mt-3 line-clamp-2 text-sm text-fabu-charcoal">
                {feedback.message || feedback.content || feedback.subject || "No message"}
              </p>
            </Link>
          ))}
        </div>

        {!isLoading && paginatedFeedbacks.length === 0 ? (
          <p className="text-center text-sm text-fabu-gray">No feedbacks found.</p>
        ) : null}

        <div className="flex items-center justify-center gap-3">
          <Button
            variant="outline"
            onClick={() => setCurrentPage((page) => Math.max(page - 1, 1))}
            disabled={currentPage === 1}
          >
            Previous
          </Button>
          <span className="text-sm text-fabu-gray">
            {currentPage} / {totalPages}
          </span>
          <Button
            variant="outline"
            onClick={() => setCurrentPage((page) => Math.min(page + 1, totalPages))}
            disabled={currentPage === totalPages}
          >
            Next
          </Button>
        </div>
      </div>
    </section>
  );
}
