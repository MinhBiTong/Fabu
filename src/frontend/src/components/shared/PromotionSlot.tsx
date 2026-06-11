"use client";

import Link from "next/link";
import { X } from "lucide-react";
import { memo, useCallback, useEffect, useMemo } from "react";
import { usePathname } from "next/navigation";
import { useQuery } from "@tanstack/react-query";
import { motion, useReducedMotion } from "framer-motion";
import { Button } from "@/components/ui/button";
import { useMotionOneReveal } from "@/hooks/use-motion-one-reveal";
import { cn } from "@/lib/utils/utils";
import {
  promotionService,
  type PromotionCreative,
  type PromotionPlacement,
} from "@/services/promotion-service";
import { usePromotionStore } from "@/store/promotion.store";

type PromotionSlotProps = {
  placement: PromotionPlacement;
  className?: string;
  compact?: boolean;
  maxItems?: number;
};

const toneClass: Record<PromotionCreative["tone"], string> = {
  brand: "border-fabu-red bg-fabu-red text-white",
  info: "border-fabu-blue bg-fabu-info text-fabu-ink",
  warning: "border-fabu-orange bg-[#FFF5E6] text-fabu-ink",
  neutral: "border-fabu-border bg-white text-fabu-ink",
};

function PromotionSlotComponent({
  placement,
  className,
  compact = false,
  maxItems = 1,
}: PromotionSlotProps) {
  const pathname = usePathname();
  const reduceMotion = useReducedMotion();
  const revealRef = useMotionOneReveal<HTMLDivElement>();
  const hidden = usePromotionStore((state) => state.hiddenPlacements[placement]);
  const hidePlacement = usePromotionStore((state) => state.hidePlacement);
  const recordImpression = usePromotionStore((state) => state.recordImpression);
  const recordClick = usePromotionStore((state) => state.recordClick);

  const { data } = useQuery({
    queryKey: ["promotions"],
    queryFn: promotionService.getActivePromotions,
    staleTime: 1000 * 60 * 5,
  });

  const promotions = useMemo(
    () =>
      (data ?? [])
        .filter((item) => item.placement === placement)
        .sort((a, b) => a.priority - b.priority)
        .slice(0, maxItems),
    [data, maxItems, placement]
  );

  useEffect(() => {
    const element = revealRef.current;
    if (!element || promotions.length === 0 || hidden) return;

    const observer = new IntersectionObserver(
      ([entry]) => {
        if (!entry?.isIntersecting) return;
        promotions.forEach((creative) => {
          recordImpression(creative.id);
          promotionService.track({
            creativeId: creative.id,
            placement,
            eventType: "impression",
            pathname,
          });
        });
        observer.disconnect();
      },
      { threshold: 0.45 }
    );

    observer.observe(element);
    return () => observer.disconnect();
  }, [hidden, pathname, placement, promotions, recordImpression, revealRef]);

  const handleClick = useCallback(
    (creative: PromotionCreative) => {
      recordClick(creative.id);
      promotionService.track({
        creativeId: creative.id,
        placement,
        eventType: "click",
        pathname,
      });
    },
    [pathname, placement, recordClick]
  );

  if (hidden || promotions.length === 0) return null;

  return (
    <div ref={revealRef} className={cn("grid gap-3", className)}>
      {promotions.map((creative) => {
        const isBrand = creative.tone === "brand";

        return (
          <motion.article
            key={creative.id}
            whileHover={reduceMotion ? undefined : { y: -2 }}
            className={cn(
              "relative overflow-hidden rounded-card border p-4 shadow-subtle",
              compact ? "min-h-32" : "min-h-40",
              toneClass[creative.tone]
            )}
          >
            <button
              type="button"
              className={cn(
                "absolute right-3 top-3 flex h-8 w-8 items-center justify-center rounded-full transition",
                isBrand ? "bg-white/15 text-white hover:bg-white/25" : "bg-white text-fabu-gray hover:text-fabu-red"
              )}
              aria-label="Hide promotion"
              onClick={() => hidePlacement(placement)}
            >
              <X className="h-4 w-4" />
            </button>

            <div className="flex h-full flex-col justify-between gap-5 pr-10">
              <div>
                <p
                  className={cn(
                    "text-xs font-bold uppercase",
                    isBrand ? "text-white/80" : "text-fabu-gray"
                  )}
                >
                  {creative.eyebrow}
                </p>
                <h3
                  className={cn(
                    "mt-2 text-xl leading-7",
                    isBrand ? "text-white" : "text-fabu-ink"
                  )}
                >
                  {creative.title}
                </h3>
                <p
                  className={cn(
                    "mt-2 text-sm leading-6",
                    isBrand ? "text-white/90" : "text-fabu-gray"
                  )}
                >
                  {creative.body}
                </p>
              </div>

              <div className="flex flex-wrap items-center gap-3">
                <Button asChild variant={isBrand ? "secondary" : "outline"} size="sm">
                  <Link href={creative.href} onClick={() => handleClick(creative)}>
                    {creative.ctaLabel}
                  </Link>
                </Button>
                {creative.metric ? (
                  <span
                    className={cn(
                      "rounded-full px-3 py-1 text-xs font-bold",
                      isBrand ? "bg-white/15 text-white" : "bg-fabu-muted text-fabu-charcoal"
                    )}
                  >
                    {creative.metric}
                  </span>
                ) : null}
              </div>
            </div>
          </motion.article>
        );
      })}
    </div>
  );
}

export const PromotionSlot = memo(PromotionSlotComponent);
