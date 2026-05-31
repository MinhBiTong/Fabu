import { create } from "zustand";
import type { FeedbackRequest, FeedbackResponse } from "@/core/types/api.types";
import { feedbackService } from "@/services/feedback-service";

type FeedbackStore = {
  feedbacks: FeedbackResponse[];
  activeFeedback: FeedbackResponse | null;
  isLoading: boolean;
  error: string | null;
  loadFeedbacks: () => Promise<void>;
  loadFeedback: (id: number | string) => Promise<FeedbackResponse | null>;
  submitFeedback: (payload: FeedbackRequest) => Promise<FeedbackResponse | null>;
};

function errorMessage(error: unknown, fallback: string) {
  return error instanceof Error ? error.message : fallback;
}

export const useFeedbackStore = create<FeedbackStore>((set, get) => ({
  feedbacks: [],
  activeFeedback: null,
  isLoading: false,
  error: null,

  loadFeedbacks: async () => {
    set({ isLoading: true, error: null });
    try {
      const feedbacks = await feedbackService.list();
      set({ feedbacks, isLoading: false });
    } catch (error) {
      set({ error: errorMessage(error, "Could not load feedbacks"), isLoading: false });
    }
  },

  loadFeedback: async (id) => {
    set({ isLoading: true, error: null });
    try {
      const activeFeedback = await feedbackService.getById(id);
      set({ activeFeedback, isLoading: false });
      return activeFeedback;
    } catch (error) {
      set({ error: errorMessage(error, "Could not load feedback"), isLoading: false });
      return null;
    }
  },

  submitFeedback: async (payload) => {
    set({ isLoading: true, error: null });
    try {
      const created = await feedbackService.create(payload);
      set({ feedbacks: [created, ...get().feedbacks], isLoading: false });
      return created;
    } catch (error) {
      set({ error: errorMessage(error, "Could not submit feedback"), isLoading: false });
      return null;
    }
  },
}));
