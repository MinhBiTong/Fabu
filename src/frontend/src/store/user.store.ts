import { create } from "zustand";
import type { UserSummary } from "@/core/types/api.types";
import { userService } from "@/services/user-service";

type UserStore = {
  users: UserSummary[];
  currentUser: UserSummary | null;
  isLoading: boolean;
  error: string | null;
  loadUsers: () => Promise<void>;
  loadCurrentUser: () => Promise<void>;
  reset: () => void;
};

export const useUserStore = create<UserStore>((set) => ({
  users: [],
  currentUser: null,
  isLoading: false,
  error: null,

  loadUsers: async () => {
    set({ isLoading: true, error: null });
    try {
      const users = await userService.list();
      set({ users, isLoading: false });
    } catch (error) {
      set({
        error: error instanceof Error ? error.message : "Could not load users",
        isLoading: false,
      });
    }
  },

  loadCurrentUser: async () => {
    set({ isLoading: true, error: null });
    try {
      const currentUser = await userService.me();
      set({ currentUser, isLoading: false });
    } catch (error) {
      set({
        error: error instanceof Error ? error.message : "Could not load profile",
        isLoading: false,
      });
    }
  },

  reset: () => set({ users: [], currentUser: null, isLoading: false, error: null }),
}));
