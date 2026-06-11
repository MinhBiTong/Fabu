"use client";

import { animate, inView, type DOMKeyframesDefinition } from "motion";
import { useEffect, useRef } from "react";

export function useMotionOneReveal<T extends HTMLElement>() {
  const ref = useRef<T | null>(null);

  useEffect(() => {
    const element = ref.current;
    if (!element) return;

    const reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
    if (reduceMotion) return;

    element.style.opacity = "0";
    element.style.transform = "translateY(12px)";

    return inView(
      element,
      () => {
        const keyframes: DOMKeyframesDefinition = {
          opacity: [0, 1],
          y: [12, 0],
        };

        animate(
          element as Element,
          keyframes,
          { duration: 0.36, ease: "easeOut" }
        );
      },
      { amount: 0.25 }
    );
  }, []);

  return ref;
}
