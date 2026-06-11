"use client";

import { animate, motion, useMotionValue, useTransform } from "framer-motion";
import { memo, useEffect } from "react";

type AnimatedNumberProps = {
  value: number;
  formatter?: (value: number) => string;
  className?: string;
};

function AnimatedNumberComponent({
  value,
  formatter = (next) => Math.round(next).toLocaleString("vi-VN"),
  className,
}: AnimatedNumberProps) {
  const motionValue = useMotionValue(0);
  const displayValue = useTransform(motionValue, (latest) => formatter(latest));

  useEffect(() => {
    const controls = animate(motionValue, value, {
      duration: 0.75,
      ease: [0.22, 1, 0.36, 1],
    });

    return controls.stop;
  }, [motionValue, value]);

  return <motion.span className={className}>{displayValue}</motion.span>;
}

export const AnimatedNumber = memo(AnimatedNumberComponent);
