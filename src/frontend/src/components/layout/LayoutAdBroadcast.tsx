"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { useQuery } from "@tanstack/react-query";
import { ChevronRight, Megaphone, Pause, Play, X } from "lucide-react";
import { AnimatePresence, motion, useReducedMotion } from "framer-motion";
import { memo, useCallback, useEffect, useMemo, useRef, useState } from "react";
import { cn } from "@/lib/utils/utils";
import {
  promotionService,
  type PromotionCreative,
  type PromotionPlacement,
} from "@/services/promotion-service";
import { usePromotionStore } from "@/store/promotion.store";

const placement: PromotionPlacement = "layout-broadcast";
const hiddenRoutePrefixes = ["/login", "/register", "/not-found"];

const toneClass: Record<PromotionCreative["tone"], string> = {
  brand: "border-fabu-red bg-fabu-red text-white",
  info: "border-fabu-blue bg-fabu-info text-fabu-ink",
  warning: "border-fabu-orange bg-[#FFF5E6] text-fabu-ink",
  neutral: "border-fabu-border bg-white text-fabu-ink",
};

function LayoutAdBroadcast() {
  const pathname = usePathname();
  const reduceMotion = useReducedMotion();
  const [activeIndex, setActiveIndex] = useState(0);
  const [paused, setPaused] = useState(false);
  const containerRef = useRef<HTMLDivElement | null>(null);
  const trackedImpressions = useRef<Set<string>>(new Set());
  const hidden = usePromotionStore((state) => state.hiddenPlacements[placement]);
  const hidePlacement = usePromotionStore((state) => state.hidePlacement);
  const recordImpression = usePromotionStore((state) => state.recordImpression);
  const recordClick = usePromotionStore((state) => state.recordClick);

  const { data } = useQuery({
    queryKey: ["promotions"],
    queryFn: promotionService.getActivePromotions,
    staleTime: 1000 * 60 * 5,
  });

  const creatives = useMemo(
    () =>
      (data ?? [])
        .filter((item) => item.placement === placement)
        .sort((a, b) => a.priority - b.priority),
    [data]
  );

  const activeCreative = creatives.length
    ? creatives[activeIndex % creatives.length]
    : undefined;
  const shouldHideForRoute = hiddenRoutePrefixes.some((prefix) => pathname.startsWith(prefix));

  useEffect(() => {
    if (reduceMotion || paused || creatives.length <= 1) return;

    const intervalId = window.setInterval(() => {
      setActiveIndex((index) => index + 1);
    }, 7000);

    return () => window.clearInterval(intervalId);
  }, [creatives.length, paused, reduceMotion]);

  useEffect(() => {
    const element = containerRef.current;
    if (!element || !activeCreative || trackedImpressions.current.has(activeCreative.id)) return;

    const observer = new IntersectionObserver(
      ([entry]) => {
        if (!entry?.isIntersecting) return;

        trackedImpressions.current.add(activeCreative.id);
        recordImpression(activeCreative.id);
        promotionService.track({
          creativeId: activeCreative.id,
          placement,
          eventType: "impression",
          pathname,
        });
        observer.disconnect();
      },
      { threshold: 0.55 }
    );

    observer.observe(element);
    return () => observer.disconnect();
  }, [activeCreative, pathname, recordImpression]);

  const handleClick = useCallback(() => {
    if (!activeCreative) return;

    recordClick(activeCreative.id);
    promotionService.track({
      creativeId: activeCreative.id,
      placement,
      eventType: "click",
      pathname,
    });
  }, [activeCreative, pathname, recordClick]);

  if (hidden || shouldHideForRoute || !activeCreative) return null;

  const isBrand = activeCreative.tone === "brand";

  return (
    <div className="border-b border-fabu-border bg-white px-4 py-3 md:px-5 lg:px-8">
      <div className="mx-auto w-full max-w-[1400px]">
        <motion.div
          ref={containerRef}
          className={cn(
            "relative overflow-hidden rounded-card border px-4 py-3 shadow-subtle",
            toneClass[activeCreative.tone]
          )}
          initial={reduceMotion ? false : { opacity: 0, y: -8 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.22, ease: "easeOut" }}
          onMouseEnter={() => setPaused(true)}
          onMouseLeave={() => setPaused(false)}
        >
          <div className="grid gap-3 pr-20 md:grid-cols-[auto_1fr_auto] md:items-center">
            <div className="flex items-center gap-3">
              <span
                className={cn(
                  "flex h-10 w-10 shrink-0 items-center justify-center rounded-full",
                  isBrand ? "bg-white/15 text-white" : "bg-fabu-muted text-fabu-red"
                )}
              >
                <Megaphone className="h-5 w-5" />
              </span>
              <span
                className={cn(
                  "rounded-full px-3 py-1 text-xs font-bold uppercase",
                  isBrand ? "bg-white/15 text-white" : "bg-fabu-muted text-fabu-gray"
                )}
              >
                {activeCreative.eyebrow}
              </span>
            </div>

            <AnimatePresence mode="wait">
              <motion.div
                key={activeCreative.id}
                initial={reduceMotion ? false : { opacity: 0, x: 10 }}
                animate={{ opacity: 1, x: 0 }}
                exit={reduceMotion ? undefined : { opacity: 0, x: -10 }}
                transition={{ duration: 0.2 }}
                className="min-w-0"
              >
                <p
                  className={cn(
                    "truncate text-sm font-bold md:text-base",
                    isBrand ? "text-white" : "text-fabu-ink"
                  )}
                >
                  {activeCreative.title}
                </p>
                <p
                  className={cn(
                    "mt-1 line-clamp-1 text-sm",
                    isBrand ? "text-white/85" : "text-fabu-gray"
                  )}
                >
                  {activeCreative.body}
                </p>
              </motion.div>
            </AnimatePresence>

            <Link
              href={activeCreative.href}
              onClick={handleClick}
              className={cn(
                "inline-flex min-h-11 items-center justify-center gap-2 rounded-fabu px-4 text-sm font-semibold transition active:scale-[0.98]",
                isBrand
                  ? "bg-white text-fabu-red hover:bg-fabu-muted"
                  : "border border-fabu-border bg-white text-fabu-charcoal hover:border-fabu-red hover:text-fabu-red"
              )}
              prefetch
            >
              {activeCreative.ctaLabel}
              <ChevronRight className="h-4 w-4" />
            </Link>
          </div>

          <div className="absolute right-3 top-3 flex items-center gap-1">
            {creatives.length > 1 ? (
              <button
                type="button"
                className={cn(
                  "flex h-8 w-8 items-center justify-center rounded-full transition",
                  isBrand ? "bg-white/15 text-white hover:bg-white/25" : "bg-white text-fabu-gray hover:text-fabu-red"
                )}
                aria-label={paused ? "Resume advertisement" : "Pause advertisement"}
                onClick={() => setPaused((value) => !value)}
              >
                {paused ? <Play className="h-4 w-4" /> : <Pause className="h-4 w-4" />}
              </button>
            ) : null}
            <button
              type="button"
              className={cn(
                "flex h-8 w-8 items-center justify-center rounded-full transition",
                isBrand ? "bg-white/15 text-white hover:bg-white/25" : "bg-white text-fabu-gray hover:text-fabu-red"
              )}
              aria-label="Hide advertisement"
              onClick={() => hidePlacement(placement)}
            >
              <X className="h-4 w-4" />
            </button>
          </div>

          {creatives.length > 1 ? (
            <div className="mt-3 flex gap-2">
              {creatives.map((creative, index) => (
                <span
                  key={creative.id}
                  className={cn(
                    "h-1.5 rounded-full transition-all",
                    index === activeIndex % creatives.length ? "w-8" : "w-3",
                    isBrand ? "bg-white/70" : "bg-fabu-red/60"
                  )}
                />
              ))}
            </div>
          ) : null}
        </motion.div>
      </div>
    </div>
  );
}

export default memo(LayoutAdBroadcast);
