import type { ServicePlan, ServicePlanPayload } from "@/core/types/api.types";
import { endpoints } from "@/lib/api/endpoints";
import { globalApiClient } from "@/lib/api/http-client";

export const servicePlanService = {
  async list() {
    const response = await globalApiClient.get<ServicePlan[]>(endpoints.services.list);
    return response.data ?? [];
  },

  async getById(id: number | string) {
    const response = await globalApiClient.get<ServicePlan>(endpoints.services.detail(id));
    return response.data;
  },

  async create(payload: ServicePlanPayload) {
    const response = await globalApiClient.post<ServicePlan>(endpoints.services.create, payload);
    return response.data;
  },

  async update(id: number | string, payload: ServicePlanPayload) {
    const response = await globalApiClient.put<ServicePlan>(endpoints.services.update(id), payload);
    return response.data;
  },

  async remove(id: number | string) {
    const response = await globalApiClient.delete<boolean>(endpoints.services.delete(id));
    return response.data;
  },
};
