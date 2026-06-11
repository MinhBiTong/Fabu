import { create } from "zustand";
import { persist } from "zustand/middleware";
import type { TelecomProduct } from "@/core/types/api.types";

export type LocalCartItem = {
  productId: string;
  name: string;
  category: string;
  price: number;
  quantity: number;
};

type CartStore = {
  items: LocalCartItem[];
  addItem: (product: TelecomProduct, quantity?: number) => void;
  updateQuantity: (productId: string, quantity: number) => void;
  removeItem: (productId: string) => void;
  clear: () => void;
};

export const useCartStore = create<CartStore>()(
  persist(
    (set) => ({
      items: [],
      addItem: (product, quantity = 1) =>
        set((state) => {
          const existing = state.items.find((item) => item.productId === product.id);
          if (existing) {
            return {
              items: state.items.map((item) =>
                item.productId === product.id
                  ? { ...item, quantity: item.quantity + quantity }
                  : item
              ),
            };
          }

          return {
            items: [
              ...state.items,
              {
                productId: product.id,
                name: product.name,
                category: product.category,
                price: product.price,
                quantity,
              },
            ],
          };
        }),
      updateQuantity: (productId, quantity) =>
        set((state) => ({
          items: state.items
            .map((item) =>
              item.productId === productId ? { ...item, quantity: Math.max(1, quantity) } : item
            )
            .filter((item) => item.quantity > 0),
        })),
      removeItem: (productId) =>
        set((state) => ({
          items: state.items.filter((item) => item.productId !== productId),
        })),
      clear: () => set({ items: [] }),
    }),
    { name: "fabu-commerce-cart" }
  )
);
