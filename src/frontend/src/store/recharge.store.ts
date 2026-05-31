import { create } from "zustand";
import type { RechargePlan, TransactionResponse } from "@/core/types/api.types";
import { rechargeService, type RechargePayload } from "@/services/recharge-service";

type RechargeStore = {
  plans: RechargePlan[];
  lastTransaction: TransactionResponse | null;
  isLoading: boolean;
  error: string | null;
  loadPlans: () => Promise<void>;
  submitRecharge: (payload: RechargePayload) => Promise<TransactionResponse | null>;
};

function getError(error: unknown, fallback: string) {
  return error instanceof Error ? error.message : fallback;
}

export const useRechargeStore = create<RechargeStore>((set) => ({
  plans: [],
  lastTransaction: null,
  isLoading: false,
  error: null,

  loadPlans: async () => {
    set({ isLoading: true, error: null });
    try {
      const plans = await rechargeService.listPlans();
      set({ plans, isLoading: false });
    } catch (error) {
      set({ error: getError(error, "Could not load recharge plans"), isLoading: false });
    }
  },

  submitRecharge: async (payload) => {
    set({ isLoading: true, error: null });
    try {
      const lastTransaction = await rechargeService.createRecharge(payload);
      set({ lastTransaction, isLoading: false });
      return lastTransaction;
    } catch (error) {
      set({ error: getError(error, "Could not create recharge transaction"), isLoading: false });
      return null;
    }
  },
}));
