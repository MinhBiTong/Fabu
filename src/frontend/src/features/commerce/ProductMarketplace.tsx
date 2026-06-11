"use client";

import Image from "next/image";
import Link from "next/link";
import { useMutation } from "@tanstack/react-query";
import { AnimatePresence, motion } from "framer-motion";
import {
  CheckCircle2,
  ChevronRight,
  Filter,
  Package,
  Search,
  ShoppingCart,
  Sparkles,
  Trash2,
  X,
} from "lucide-react";
import { memo, useCallback, useEffect, useMemo, useRef, useState } from "react";
import { toast } from "react-toastify";
import { MotionCard } from "@/components/motion/MotionCard";
import { MotionPage } from "@/components/motion/MotionPage";
import { PromotionSlot } from "@/components/shared/PromotionSlot";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import type { TelecomProduct } from "@/core/types/api.types";
import { useAuth } from "@/hooks/use-auth";
import { cn } from "@/lib/utils/utils";
import { drawerSlide } from "@/lib/animation/motion-presets";
import { commerceService, type ProductQuery } from "@/services/commerce-service";
import { useCartStore } from "@/store/cart.store";
import walletImage from "@/styles/images/wallet.png";
import { useProducts } from "./use-products";

function formatCurrency(value: number) {
  return `${value.toLocaleString("vi-VN")}đ`;
}

function useDebouncedValue(value: string, delay = 220) {
  const [debounced, setDebounced] = useState(value);

  useEffect(() => {
    const timer = window.setTimeout(() => setDebounced(value), delay);
    return () => window.clearTimeout(timer);
  }, [delay, value]);

  return debounced;
}

function useInfiniteWindow(total: number, step = 6) {
  const [visibleCount, setVisibleCount] = useState(step);
  const sentinelRef = useRef<HTMLDivElement | null>(null);

  useEffect(() => {
    const element = sentinelRef.current;
    if (!element) return;

    const observer = new IntersectionObserver(
      ([entry]) => {
        if (entry?.isIntersecting) {
          setVisibleCount((count) => Math.min(total, count + step));
        }
      },
      { rootMargin: "240px" }
    );

    observer.observe(element);
    return () => observer.disconnect();
  }, [step, total]);

  return { visibleCount, sentinelRef };
}

const ProductCard = memo(function ProductCard({
  product,
  onAdd,
}: {
  product: TelecomProduct;
  onAdd: (product: TelecomProduct) => void;
}) {
  const stockTone =
    product.stockQuantity <= 20
      ? "bg-[#FFF5E6] text-fabu-orange"
      : "bg-[#EAF8F2] text-[#03A678]";

  return (
    <MotionCard className="grid min-h-[330px] content-between p-0">
      <div className="border-b border-fabu-border bg-fabu-muted p-4">
        <div className="flex aspect-[4/3] items-center justify-center rounded-card bg-white">
          <Package className="h-14 w-14 text-fabu-red" />
        </div>
      </div>

      <div className="grid gap-4 p-5">
        <div className="flex flex-wrap items-center gap-2">
          <span className="rounded-full bg-fabu-info px-3 py-1 text-xs font-bold text-fabu-link">
            {product.category}
          </span>
          {product.isFeatured ? (
            <span className="rounded-full bg-fabu-red px-3 py-1 text-xs font-bold text-white">
              Featured
            </span>
          ) : null}
          <span className={cn("rounded-full px-3 py-1 text-xs font-bold", stockTone)}>
            {product.stockQuantity} còn
          </span>
        </div>

        <div>
          <h3 className="text-xl">{product.name}</h3>
          <p className="mt-2 line-clamp-2 text-sm leading-6 text-fabu-gray">
            {product.description}
          </p>
        </div>

        <div className="flex items-end justify-between gap-3">
          <div>
            <p className="text-xl font-bold text-fabu-ink">{formatCurrency(product.price)}</p>
            {product.originalPrice ? (
              <p className="text-sm text-fabu-gray line-through">
                {formatCurrency(product.originalPrice)}
              </p>
            ) : null}
          </div>
          <Button size="sm" onClick={() => onAdd(product)}>
            <ShoppingCart className="h-4 w-4" />
            Add
          </Button>
        </div>
      </div>
    </MotionCard>
  );
});

function ProductSkeletonGrid() {
  return (
    <div className="grid gap-5 md:grid-cols-2 xl:grid-cols-3">
      {Array.from({ length: 6 }).map((_, index) => (
        <div key={index} className="fabu-card grid gap-4">
          <Skeleton className="aspect-[4/3] w-full" />
          <Skeleton className="h-5 w-2/3" />
          <Skeleton className="h-4 w-full" />
          <Skeleton className="h-11 w-32" />
        </div>
      ))}
    </div>
  );
}

function CartDrawer({ open, onClose }: { open: boolean; onClose: () => void }) {
  const { profile } = useAuth();
  const items = useCartStore((state) => state.items);
  const removeItem = useCartStore((state) => state.removeItem);
  const clear = useCartStore((state) => state.clear);
  const total = useMemo(
    () => items.reduce((sum, item) => sum + item.price * item.quantity, 0),
    [items]
  );

  const checkoutMutation = useMutation({
    mutationFn: async () => {
      const customerId = Number(profile.id);
      if (!Number.isFinite(customerId) || customerId <= 0) {
        throw new Error("Cần đăng nhập bằng customer account để checkout backend.");
      }

      return commerceService.checkout({
        customerId,
        paymentMethod: 1,
        note: "Checkout from Fabu marketplace",
      });
    },
    onSuccess: () => {
      clear();
      toast.success("Đã tạo checkout theo backend Cart/Order/Payment flow.");
      onClose();
    },
    onError: (error) => {
      toast.info(error instanceof Error ? error.message : "Cart đang giữ local, backend chưa sẵn sàng.");
    },
  });

  return (
    <AnimatePresence>
      {open ? (
        <motion.div
          className="fixed inset-0 z-50 flex justify-end bg-black/35"
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          exit={{ opacity: 0 }}
          onMouseDown={onClose}
        >
          <motion.aside
            variants={drawerSlide}
            initial="hidden"
            animate="show"
            exit="exit"
            className="h-full w-full max-w-md overflow-auto bg-white p-6 shadow-modal"
            onMouseDown={(event) => event.stopPropagation()}
          >
            <div className="flex items-start justify-between gap-4">
              <div>
                <p className="text-sm font-bold uppercase text-fabu-red">Cart</p>
                <h2 className="mt-2 text-2xl">Giỏ hàng sản phẩm</h2>
              </div>
              <button
                type="button"
                className="flex h-11 w-11 items-center justify-center rounded-full bg-fabu-muted hover:text-fabu-red"
                aria-label="Close cart"
                onClick={onClose}
              >
                <X className="h-5 w-5" />
              </button>
            </div>

            <div className="mt-6 grid gap-3">
              {items.length === 0 ? (
                <div className="rounded-card border border-fabu-border bg-fabu-muted p-5 text-sm text-fabu-gray">
                  Chưa có sản phẩm trong giỏ.
                </div>
              ) : (
                items.map((item) => (
                  <div key={item.productId} className="rounded-card border border-fabu-border p-4">
                    <div className="flex items-start justify-between gap-3">
                      <div>
                        <p className="font-bold text-fabu-ink">{item.name}</p>
                        <p className="mt-1 text-sm text-fabu-gray">
                          {item.category} x {item.quantity}
                        </p>
                      </div>
                      <button
                        type="button"
                        className="flex h-10 w-10 items-center justify-center rounded-full bg-fabu-muted hover:text-fabu-red"
                        onClick={() => removeItem(item.productId)}
                        aria-label={`Remove ${item.name}`}
                      >
                        <Trash2 className="h-4 w-4" />
                      </button>
                    </div>
                    <p className="mt-3 font-bold text-fabu-ink">
                      {formatCurrency(item.price * item.quantity)}
                    </p>
                  </div>
                ))
              )}
            </div>

            <PromotionSlot placement="upsell" className="mt-5" compact />

            <div className="mt-6 rounded-card bg-fabu-muted p-4">
              <div className="flex items-center justify-between gap-3">
                <span className="font-bold text-fabu-ink">Tạm tính</span>
                <span className="text-xl font-bold text-fabu-red">{formatCurrency(total)}</span>
              </div>
            </div>

            <Button
              className="mt-5 w-full"
              disabled={items.length === 0 || checkoutMutation.isPending}
              onClick={() => checkoutMutation.mutate()}
            >
              Checkout
            </Button>
          </motion.aside>
        </motion.div>
      ) : null}
    </AnimatePresence>
  );
}

export function ProductMarketplace() {
  const [keyword, setKeyword] = useState("");
  const [category, setCategory] = useState("");
  const [cartOpen, setCartOpen] = useState(false);
  const debouncedKeyword = useDebouncedValue(keyword);
  const addItem = useCartStore((state) => state.addItem);
  const cartItems = useCartStore((state) => state.items);

  const filters = useMemo<ProductQuery>(
    () => ({ keyword: debouncedKeyword, category: category || undefined }),
    [category, debouncedKeyword]
  );
  const { products, categories, isLoading, isUsingFallback } = useProducts(filters);
  const { visibleCount, sentinelRef } = useInfiniteWindow(products.length, 6);
  const visibleProducts = useMemo(
    () => products.slice(0, visibleCount),
    [products, visibleCount]
  );
  const cartCount = useMemo(
    () => cartItems.reduce((sum, item) => sum + item.quantity, 0),
    [cartItems]
  );

  const handleAdd = useCallback(
    (product: TelecomProduct) => {
      addItem(product, 1);
      toast.success(`${product.name} đã được thêm vào giỏ.`);
    },
    [addItem]
  );

  return (
    <MotionPage className="fabu-page">
      <section className="fabu-section bg-fabu-muted">
        <div className="fabu-container grid gap-6">
          <PromotionSlot placement="top-banner" />

          <div className="grid overflow-hidden rounded-card bg-white shadow-elevated lg:grid-cols-[1fr_360px]">
            <div className="bg-fabu-red p-6 text-white md:p-8">
              <span className="inline-flex items-center gap-2 rounded-full bg-white/15 px-3 py-1 text-xs font-bold uppercase">
                <Sparkles className="h-4 w-4" />
                Fabu Shop
              </span>
              <h1 className="mt-4 text-white">Thiết bị viễn thông, gói cước và bundle bán kèm</h1>
              <p className="mt-3 max-w-3xl text-sm leading-7 text-white/90 md:text-base">
                Bố cục ưu tiên discovery, promotion đúng chỗ, cart drawer và checkout nối với backend
                Cart/Order/Payment khi customer session sẵn sàng.
              </p>
              <div className="mt-6 flex flex-col gap-3 sm:flex-row">
                <Button variant="secondary" onClick={() => setCategory("Router")}>
                  Xem router
                </Button>
                <Button className="border-white bg-white text-fabu-red hover:bg-fabu-muted" onClick={() => setCartOpen(true)}>
                  <ShoppingCart className="h-4 w-4" />
                  Cart {cartCount}
                </Button>
              </div>
            </div>
            <div className="flex items-center justify-center bg-white p-8">
              <div className="relative flex h-48 w-48 items-center justify-center rounded-full bg-fabu-info">
                <Image src={walletImage} alt="Fabu commerce wallet" width={96} height={96} priority />
              </div>
            </div>
          </div>

          <div className="grid gap-6 xl:grid-cols-[280px_1fr_320px]">
            <aside className="grid content-start gap-5">
              <div className="fabu-dashboard-card">
                <div className="flex items-center gap-2">
                  <Filter className="h-5 w-5 text-fabu-red" />
                  <h2 className="text-2xl">Filter</h2>
                </div>
                <label className="mt-5 grid gap-2">
                  <span className="fabu-label">Search</span>
                  <div className="relative">
                    <Search className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-fabu-gray" />
                    <input
                      className="fabu-input pl-10"
                      value={keyword}
                      onChange={(event) => setKeyword(event.target.value)}
                      placeholder="Router, SIM, accessory..."
                    />
                  </div>
                </label>

                <div className="mt-5 grid gap-2">
                  <span className="fabu-label">Category</span>
                  <button
                    type="button"
                    className={cn(
                      "flex min-h-11 items-center rounded px-3 text-left text-sm transition",
                      category === "" ? "bg-fabu-red text-white" : "hover:bg-fabu-muted hover:text-fabu-red"
                    )}
                    onClick={() => setCategory("")}
                  >
                    Tất cả
                  </button>
                  {categories.map((item) => (
                    <button
                      key={item}
                      type="button"
                      className={cn(
                        "flex min-h-11 items-center rounded px-3 text-left text-sm transition",
                        category === item
                          ? "bg-fabu-red text-white"
                          : "hover:bg-fabu-muted hover:text-fabu-red"
                      )}
                      onClick={() => setCategory(item)}
                    >
                      {item}
                    </button>
                  ))}
                </div>
              </div>
              <PromotionSlot placement="sidebar" compact />
            </aside>

            <main className="grid content-start gap-5">
              <div className="flex flex-col gap-3 rounded-card border border-fabu-border bg-white p-5 shadow-subtle md:flex-row md:items-center md:justify-between">
                <div>
                  <h2 className="text-2xl">Product Discovery</h2>
                  <p className="mt-1 text-sm text-fabu-gray">
                    {products.length} sản phẩm {isUsingFallback ? "fallback" : "từ backend"}.
                  </p>
                </div>
                <Button variant="outline" asChild>
                  <Link href="/P5GDataPlan" prefetch>
                    Gói cước liên quan
                    <ChevronRight className="h-4 w-4" />
                  </Link>
                </Button>
              </div>

              {isLoading ? (
                <ProductSkeletonGrid />
              ) : (
                <div className="grid gap-5 md:grid-cols-2 xl:grid-cols-3">
                  {visibleProducts.map((product) => (
                    <ProductCard key={product.id} product={product} onAdd={handleAdd} />
                  ))}
                </div>
              )}

              <div ref={sentinelRef} className="h-8" />

              {visibleProducts.length >= products.length ? (
                <div className="rounded-card border border-fabu-border bg-white p-4 text-center text-sm text-fabu-gray">
                  Đã hiển thị toàn bộ sản phẩm phù hợp.
                </div>
              ) : null}
            </main>

            <aside className="grid content-start gap-5">
              <PromotionSlot placement="merchant" compact />
              <PromotionSlot placement="voucher" compact />
              <PromotionSlot placement="cross-sell" compact />
              <div className="fabu-dashboard-card">
                <div className="flex items-center gap-3">
                  <CheckCircle2 className="h-5 w-5 text-[#03A678]" />
                  <h2 className="text-2xl">UX Guardrails</h2>
                </div>
                <div className="mt-4 grid gap-3 text-sm leading-6 text-fabu-gray">
                  <p>Promotion không chen giữa form checkout.</p>
                  <p>Cart drawer giữ ngữ cảnh mua hàng, không ép đổi route.</p>
                  <p>Filter/search phản hồi nhanh với debounce và memoization.</p>
                </div>
              </div>
            </aside>
          </div>
        </div>
      </section>

      <CartDrawer open={cartOpen} onClose={() => setCartOpen(false)} />
    </MotionPage>
  );
}
