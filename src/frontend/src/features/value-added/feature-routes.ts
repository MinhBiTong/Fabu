export type FeatureRoute = {
  href: string;
  label: string;
  title: string;
  description: string;
  accent: string;
};

export const featureRoutes: FeatureRoute[] = [
  {
    href: "/features/digital-services",
    label: "Digital",
    title: "Digital Services",
    description: "Entertainment, storage, family, and lifestyle add-ons managed with one Fabu account.",
    accent: "Digital",
  },
  {
    href: "/features/sim-store",
    label: "SIM Store",
    title: "Beautiful SIM Store",
    description: "Search memorable numbers, reserve a SIM, and complete activation with assisted support.",
    accent: "SIM",
  },
  {
    href: "/features/promotions",
    label: "Offers",
    title: "Promotions & Coupons",
    description: "Browse recharge bonuses, package campaigns, loyalty perks, and coupon-ready offers.",
    accent: "Offers",
  },
  {
    href: "/features/enterprise",
    label: "Enterprise",
    title: "Enterprise Solutions",
    description: "Connectivity, multi-line recharge, account controls, and priority care for teams.",
    accent: "Business",
  },
];
