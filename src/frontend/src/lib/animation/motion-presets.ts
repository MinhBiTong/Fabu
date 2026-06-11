import type { Variants } from "framer-motion";

export const pageTransition: Variants = {
  hidden: { opacity: 0, y: 12 },
  show: {
    opacity: 1,
    y: 0,
    transition: {
      duration: 0.32,
      ease: [0.22, 1, 0.36, 1],
      staggerChildren: 0.05,
    },
  },
};

export const fadeInUp: Variants = {
  hidden: { opacity: 0, y: 16 },
  show: {
    opacity: 1,
    y: 0,
    transition: { duration: 0.28, ease: [0.22, 1, 0.36, 1] },
  },
};

export const scaleIn: Variants = {
  hidden: { opacity: 0, scale: 0.97 },
  show: {
    opacity: 1,
    scale: 1,
    transition: { duration: 0.22, ease: [0.22, 1, 0.36, 1] },
  },
  exit: {
    opacity: 0,
    scale: 0.97,
    transition: { duration: 0.16 },
  },
};

export const drawerSlide: Variants = {
  hidden: { x: "100%" },
  show: {
    x: 0,
    transition: { duration: 0.28, ease: [0.22, 1, 0.36, 1] },
  },
  exit: {
    x: "100%",
    transition: { duration: 0.2, ease: [0.4, 0, 1, 1] },
  },
};
