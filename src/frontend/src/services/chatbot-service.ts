import type {
  ChatbotMessageRequest,
  ChatbotMessageResponse,
} from "@/core/types/api.types";
import { endpoints } from "@/lib/api/endpoints";
import { globalApiClient } from "@/lib/api/http-client";

export const chatbotService = {
  async sendMessage(payload: ChatbotMessageRequest) {
    const response = await globalApiClient.post<ChatbotMessageResponse>(
      endpoints.chatbot.chat,
      {
        CustomerId: payload.customerId ?? null,
        SessionId: payload.sessionId ?? null,
        Message: payload.message,
        ResetContext: payload.resetContext ?? false,
      }
    );

    return response.data;
  },
};
