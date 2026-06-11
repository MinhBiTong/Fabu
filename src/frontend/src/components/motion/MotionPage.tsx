"use client";

import type { ReactNode } from "react";
import { motion, useReducedMotion } from "framer-motion";
import { pageTransition } from "@/lib/animation/motion-presets";

type MotionPageProps = {
  children: ReactNode;
  className?: string;
};

export function MotionPage({ children, className }: MotionPageProps) {
  const reduceMotion = useReducedMotion();

  return (
    <motion.div
      className={className}
      variants={reduceMotion ? undefined : pageTransition}
      initial={reduceMotion ? false : "hidden"}
      animate="show"
    >
      {children}
    </motion.div>
  );
}
