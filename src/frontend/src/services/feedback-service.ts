import type { FeedbackRequest, FeedbackResponse } from "@/core/types/api.types";
import { endpoints } from "@/lib/api/endpoints";
import { globalApiClient } from "@/lib/api/http-client";

export const feedbackService = {
  async list() {
    const response = await globalApiClient.get<FeedbackResponse[]>(endpoints.feedbacks.list);
    return response.data ?? [];
  },

  async getById(id: number | string) {
    const response = await globalApiClient.get<FeedbackResponse>(endpoints.feedbacks.detail(id));
    return response.data;
  },

  async create(payload: FeedbackRequest) {
    const response = await globalApiClient.post<FeedbackResponse>(endpoints.feedbacks.create, {
      CustomerId: payload.customerId,
      Subject: payload.subject,
      Message: payload.message,
      Rating: payload.rating,
    });
    return response.data;
  },

  async markRead(id: number | string) {
    const response = await globalApiClient.put<boolean>(endpoints.feedbacks.markRead(id));
    return response.data;
  },
};
