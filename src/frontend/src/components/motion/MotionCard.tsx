"use client";

import type { ReactNode } from "react";
import { motion, type HTMLMotionProps, useReducedMotion } from "framer-motion";
import { cn } from "@/lib/utils/utils";
import { fadeInUp } from "@/lib/animation/motion-presets";

type MotionCardProps = HTMLMotionProps<"article"> & {
  children: ReactNode;
  interactive?: boolean;
};

export function MotionCard({
  children,
  className,
  interactive = true,
  ...props
}: MotionCardProps) {
  const reduceMotion = useReducedMotion();

  return (
    <motion.article
      variants={reduceMotion ? undefined : fadeInUp}
      whileHover={interactive && !reduceMotion ? { y: -3, scale: 1.005 } : undefined}
      whileTap={interactive && !reduceMotion ? { scale: 0.99 } : undefined}
      className={cn("fabu-card", className)}
      {...props}
    >
      {children}
    </motion.article>
  );
}
