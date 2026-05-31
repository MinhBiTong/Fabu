import type { UserSummary } from "@/core/types/api.types";
import { endpoints } from "@/lib/api/endpoints";
import { globalApiClient } from "@/lib/api/http-client";

export const userService = {
  async me() {
    const response = await globalApiClient.get<UserSummary>(endpoints.users.me);
    return response.data;
  },

  async list() {
    const response = await globalApiClient.get<UserSummary[]>(endpoints.users.list);
    return response.data ?? [];
  },
};
