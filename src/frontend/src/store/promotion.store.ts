import { create } from "zustand";
import { persist } from "zustand/middleware";
import type { PromotionPlacement } from "@/services/promotion-service";

type PromotionStore = {
  hiddenPlacements: Partial<Record<PromotionPlacement, boolean>>;
  impressionCounts: Record<string, number>;
  clickCounts: Record<string, number>;
  hidePlacement: (placement: PromotionPlacement) => void;
  showPlacement: (placement: PromotionPlacement) => void;
  recordImpression: (creativeId: string) => void;
  recordClick: (creativeId: string) => void;
};

export const usePromotionStore = create<PromotionStore>()(
  persist(
    (set) => ({
      hiddenPlacements: {},
      impressionCounts: {},
      clickCounts: {},
      hidePlacement: (placement) =>
        set((state) => ({
          hiddenPlacements: { ...state.hiddenPlacements, [placement]: true },
        })),
      showPlacement: (placement) =>
        set((state) => ({
          hiddenPlacements: { ...state.hiddenPlacements, [placement]: false },
        })),
      recordImpression: (creativeId) =>
        set((state) => ({
          impressionCounts: {
            ...state.impressionCounts,
            [creativeId]: (state.impressionCounts[creativeId] ?? 0) + 1,
          },
        })),
      recordClick: (creativeId) =>
        set((state) => ({
          clickCounts: {
            ...state.clickCounts,
            [creativeId]: (state.clickCounts[creativeId] ?? 0) + 1,
          },
        })),
    }),
    { name: "fabu-promotion-ui" }
  )
);
