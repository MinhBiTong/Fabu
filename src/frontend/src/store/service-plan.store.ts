import { create } from "zustand";
import type { ServicePlan, ServicePlanPayload } from "@/core/types/api.types";
import { servicePlanService } from "@/services/service-service";

type ServicePlanStore = {
  plans: ServicePlan[];
  activePlan: ServicePlan | null;
  isLoading: boolean;
  error: string | null;
  loadPlans: () => Promise<void>;
  loadPlan: (id: number | string) => Promise<ServicePlan | null>;
  createPlan: (payload: ServicePlanPayload) => Promise<ServicePlan | null>;
  updatePlan: (id: number | string, payload: ServicePlanPayload) => Promise<ServicePlan | null>;
  deletePlan: (id: number | string) => Promise<boolean>;
};

function getErrorMessage(error: unknown, fallback: string) {
  return error instanceof Error ? error.message : fallback;
}

export const useServicePlanStore = create<ServicePlanStore>((set, get) => ({
  plans: [],
  activePlan: null,
  isLoading: false,
  error: null,

  loadPlans: async () => {
    set({ isLoading: true, error: null });
    try {
      const plans = await servicePlanService.list();
      set({ plans, isLoading: false });
    } catch (error) {
      set({ error: getErrorMessage(error, "Could not load service plans"), isLoading: false });
    }
  },

  loadPlan: async (id) => {
    set({ isLoading: true, error: null });
    try {
      const activePlan = await servicePlanService.getById(id);
      set({ activePlan, isLoading: false });
      return activePlan;
    } catch (error) {
      set({ error: getErrorMessage(error, "Could not load service plan"), isLoading: false });
      return null;
    }
  },

  createPlan: async (payload) => {
    set({ isLoading: true, error: null });
    try {
      const created = await servicePlanService.create(payload);
      set({ plans: [created, ...get().plans], isLoading: false });
      return created;
    } catch (error) {
      set({ error: getErrorMessage(error, "Could not create service plan"), isLoading: false });
      return null;
    }
  },

  updatePlan: async (id, payload) => {
    set({ isLoading: true, error: null });
    try {
      const updated = await servicePlanService.update(id, payload);
      set({
        plans: get().plans.map((plan) => (String(plan.id) === String(id) ? updated : plan)),
        activePlan: updated,
        isLoading: false,
      });
      return updated;
    } catch (error) {
      set({ error: getErrorMessage(error, "Could not update service plan"), isLoading: false });
      return null;
    }
  },

  deletePlan: async (id) => {
    set({ isLoading: true, error: null });
    try {
      await servicePlanService.remove(id);
      set({
        plans: get().plans.filter((plan) => String(plan.id) !== String(id)),
        activePlan: null,
        isLoading: false,
      });
      return true;
    } catch (error) {
      set({ error: getErrorMessage(error, "Could not delete service plan"), isLoading: false });
      return false;
    }
  },
}));
