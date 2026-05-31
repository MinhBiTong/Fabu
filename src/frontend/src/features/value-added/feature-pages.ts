import type { FeatureRoute } from "./feature-routes";
import { featureRoutes } from "./feature-routes";

export type FeatureStat = {
  label: string;
  value: string;
};

export type FeatureSectionItem = {
  title: string;
  description: string;
};

export type FeaturePackage = FeatureSectionItem & {
  badge: string;
  price: string;
};

export type FeaturePageConfig = {
  route: FeatureRoute;
  eyebrow: string;
  lead: string;
  tone: "red" | "ink";
  primaryAction: {
    href: string;
    label: string;
  };
  secondaryAction: {
    href: string;
    label: string;
  };
  stats: FeatureStat[];
  benefits: FeatureSectionItem[];
  packages: FeaturePackage[];
  workflow: FeatureSectionItem[];
  backend: FeatureSectionItem[];
};

const [digitalRoute, simRoute, promotionRoute, enterpriseRoute] = featureRoutes;

export const featurePages = {
  digitalServices: {
    route: digitalRoute,
    eyebrow: "Fabu digital ecosystem",
    lead: "A structured service hub for customers who need content, cloud, family controls, and account support without leaving the Fabu experience.",
    tone: "red",
    primaryAction: {
      href: "/P5GDataPlan",
      label: "Browse data plans",
    },
    secondaryAction: {
      href: "/contact",
      label: "Ask support",
    },
    stats: [
      { value: "1", label: "Fabu account" },
      { value: "24/7", label: "support access" },
      { value: "4", label: "service groups" },
    ],
    benefits: [
      {
        title: "Unified account flow",
        description: "Customers can move from account, recharge, data plan, and support pages without changing interaction patterns.",
      },
      {
        title: "Service-first layout",
        description: "Cards, actions, and content hierarchy match the existing Fabu recharge and 5G plan surfaces.",
      },
      {
        title: "Support-ready content",
        description: "Each product block points customers to the current contact and feedback flows backed by the REST API.",
      },
    ],
    packages: [
      {
        badge: "Entertainment",
        title: "TV & Music",
        price: "From current plan",
        description: "Bundle entertainment add-ons with high-speed mobile data packages.",
      },
      {
        badge: "Cloud",
        title: "Storage Plus",
        price: "Account add-on",
        description: "Position storage and backup products beside existing customer profile workflows.",
      },
      {
        badge: "Family",
        title: "Family Connect",
        price: "Shared access",
        description: "Support shared usage, account supervision, and assisted customer care.",
      },
    ],
    workflow: [
      {
        title: "Discover",
        description: "Customer scans the digital catalog from a familiar Fabu card grid.",
      },
      {
        title: "Choose",
        description: "Primary actions route to live plan, recharge, or support flows.",
      },
      {
        title: "Resolve",
        description: "Feedback and contact pages keep support escalation consistent with the backend.",
      },
    ],
    backend: [
      {
        title: "Service",
        description: "Plan discovery continues to use the existing service catalog endpoint.",
      },
      {
        title: "v1/Users/me",
        description: "Account-aware flows stay aligned with the current profile contract.",
      },
      {
        title: "Feedbacks",
        description: "Support requests use the same customer feedback surface.",
      },
    ],
  },
  simStore: {
    route: simRoute,
    eyebrow: "SIM discovery and activation",
    lead: "A focused storefront for memorable numbers, reservation intent, and assisted activation while preserving the current Fabu card, form, and CTA language.",
    tone: "red",
    primaryAction: {
      href: "/contact",
      label: "Reserve a SIM",
    },
    secondaryAction: {
      href: "/Profile",
      label: "View account",
    },
    stats: [
      { value: "10K+", label: "number patterns" },
      { value: "3", label: "search modes" },
      { value: "Fast", label: "activation flow" },
    ],
    benefits: [
      {
        title: "Pattern-based browsing",
        description: "Customers can compare easy-to-remember, premium, and business number groups from one catalog view.",
      },
      {
        title: "Reservation intent",
        description: "The page keeps the CTA connected to support so incomplete checkout logic does not create a broken flow.",
      },
      {
        title: "Activation handoff",
        description: "Support and profile pages remain the source for identity and customer data after SIM selection.",
      },
    ],
    packages: [
      {
        badge: "Popular",
        title: "Easy Memory",
        price: "Reserved pricing",
        description: "Balanced numbers for everyday personal use and quick customer recall.",
      },
      {
        badge: "Premium",
        title: "Lucky Pattern",
        price: "Quote by support",
        description: "Special endings and repeating digits for customers who want a standout SIM.",
      },
      {
        badge: "Business",
        title: "Team Numbers",
        price: "Bulk support",
        description: "Number sets for teams that need consistent contact and billing ownership.",
      },
    ],
    workflow: [
      {
        title: "Search",
        description: "Customer narrows intent by pattern, number type, or business need.",
      },
      {
        title: "Reserve",
        description: "Primary action sends the request into the existing contact flow.",
      },
      {
        title: "Activate",
        description: "Account information and support handling complete the onboarding process.",
      },
    ],
    backend: [
      {
        title: "v1/Users/me",
        description: "Authenticated identity is reused for future activation and ownership checks.",
      },
      {
        title: "Feedbacks",
        description: "Reservation and support intent can be captured through the existing feedback API.",
      },
      {
        title: "AIChatbot/chat",
        description: "The chatbot can assist customers before they submit a support request.",
      },
    ],
  },
  promotions: {
    route: promotionRoute,
    eyebrow: "Campaign and coupon center",
    lead: "A promotion surface for active recharge campaigns, data package offers, and loyalty hooks that maps customers back to transactional pages already backed by REST services.",
    tone: "ink",
    primaryAction: {
      href: "/recharge",
      label: "Recharge with offer",
    },
    secondaryAction: {
      href: "/P5GDataPlan",
      label: "Compare data plans",
    },
    stats: [
      { value: "Active", label: "offer state" },
      { value: "Coupon", label: "ready field" },
      { value: "REST", label: "transaction flow" },
    ],
    benefits: [
      {
        title: "Promotion cards",
        description: "Campaigns follow the same dense, scan-friendly card pattern used across the current homepage.",
      },
      {
        title: "Recharge connection",
        description: "Offer actions lead customers directly to the recharge form where coupon codes are already supported.",
      },
      {
        title: "Plan comparison",
        description: "Data promotions link back to live 5G package data instead of duplicating backend state.",
      },
    ],
    packages: [
      {
        badge: "Recharge",
        title: "Bonus Balance",
        price: "Coupon enabled",
        description: "Drive customers to the recharge flow with a visible coupon-ready campaign.",
      },
      {
        badge: "5G",
        title: "Data Discount",
        price: "Plan dependent",
        description: "Promote high-speed packages while keeping plan details in the service catalog.",
      },
      {
        badge: "Loyalty",
        title: "Member Rewards",
        price: "Account based",
        description: "Create a destination for reward messaging without changing profile data contracts.",
      },
    ],
    workflow: [
      {
        title: "Scan offers",
        description: "Customers compare campaign type, benefit, and next step in one section.",
      },
      {
        title: "Apply",
        description: "Recharge offers route to the existing transaction form and coupon field.",
      },
      {
        title: "Track",
        description: "Completed activity remains visible through profile and transaction history flows.",
      },
    ],
    backend: [
      {
        title: "RechargePlans/active",
        description: "Active recharge plan data remains the source for recharge-related offers.",
      },
      {
        title: "v1/Transaction/recharge",
        description: "Coupon-aware transaction payloads continue through the recharge endpoint.",
      },
      {
        title: "Service",
        description: "Data plan campaign details link back to the service catalog.",
      },
    ],
  },
  enterprise: {
    route: enterpriseRoute,
    eyebrow: "Business connectivity",
    lead: "A professional enterprise entry point for multi-line mobile service, data packages, account control, and support escalation without introducing a separate visual language.",
    tone: "ink",
    primaryAction: {
      href: "/contact",
      label: "Contact enterprise care",
    },
    secondaryAction: {
      href: "/P5GDataPlan",
      label: "Review plans",
    },
    stats: [
      { value: "Multi", label: "line support" },
      { value: "SLA", label: "care model" },
      { value: "Central", label: "billing intent" },
    ],
    benefits: [
      {
        title: "Team-ready hierarchy",
        description: "Business content is structured for fast scanning by admins, buyers, and support operators.",
      },
      {
        title: "Shared service logic",
        description: "Enterprise discovery reuses the same plan, contact, and account flows already in production.",
      },
      {
        title: "Operational path",
        description: "Customers are guided from solution selection into support instead of an unsupported checkout branch.",
      },
    ],
    packages: [
      {
        badge: "Teams",
        title: "Business Mobile",
        price: "By package",
        description: "Mobile lines and data packages for small teams and field employees.",
      },
      {
        badge: "Control",
        title: "Central Account",
        price: "Account based",
        description: "A front door for central ownership, profile checks, and support requests.",
      },
      {
        badge: "Support",
        title: "Priority Care",
        price: "Assisted",
        description: "Escalation-ready messaging aligned with contact and feedback endpoints.",
      },
    ],
    workflow: [
      {
        title: "Assess",
        description: "Business users review solution groups and required support scope.",
      },
      {
        title: "Select",
        description: "Plan-related needs route to live catalog pages for current package data.",
      },
      {
        title: "Escalate",
        description: "Contact and feedback tools capture the request for back-office handling.",
      },
    ],
    backend: [
      {
        title: "Service",
        description: "Enterprise package discovery stays connected to the service endpoint.",
      },
      {
        title: "v1/Users/me",
        description: "Account context remains available for authenticated business users.",
      },
      {
        title: "Feedbacks",
        description: "Enterprise contact intent can move through the existing support contract.",
      },
    ],
  },
} satisfies Record<string, FeaturePageConfig>;
