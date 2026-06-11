export type PromotionPlacement =
  | "layout-broadcast"
  | "top-banner"
  | "sidebar"
  | "dashboard"
  | "campaign"
  | "merchant"
  | "voucher"
  | "cross-sell"
  | "upsell";

export type PromotionCreative = {
  id: string;
  placement: PromotionPlacement;
  title: string;
  eyebrow: string;
  body: string;
  ctaLabel: string;
  href: string;
  enabled: boolean;
  priority: number;
  tone: "brand" | "info" | "warning" | "neutral";
  metric?: string;
};

export type PromotionTrackEvent = {
  creativeId: string;
  placement: PromotionPlacement;
  eventType: "impression" | "click";
  pathname?: string;
  metadata?: Record<string, string | number | boolean | null>;
  occurredAt: string;
};

const fallbackPromotions: PromotionCreative[] = [
  {
    id: "layout-broadcast-device-plan",
    placement: "layout-broadcast",
    eyebrow: "Sponsored",
    title: "Combo Router 5G + gói tháng",
    body: "Ưu đãi nhẹ cho khách đang tìm thiết bị và thuê bao data.",
    ctaLabel: "Khám phá",
    href: "/shop",
    enabled: true,
    priority: 1,
    tone: "brand",
    metric: "New",
  },
  {
    id: "hero-5g-device-bundle",
    placement: "top-banner",
    eyebrow: "5G Bundle",
    title: "Thiết bị 5G kèm gói cước tháng",
    body: "Gợi ý gói router, SIM data và trả sau trong một luồng thanh toán.",
    ctaLabel: "Xem bundle",
    href: "/shop",
    enabled: true,
    priority: 1,
    tone: "brand",
    metric: "Tối ưu ARPU",
  },
  {
    id: "merchant-device-promo",
    placement: "merchant",
    eyebrow: "Merchant Promotion",
    title: "Combo thiết bị bán chạy cho cửa hàng",
    body: "Ưu tiên sản phẩm tồn kho tốt, biên lợi nhuận cao và lượt xem tăng nhanh.",
    ctaLabel: "Mở danh sách",
    href: "/shop?segment=merchant",
    enabled: true,
    priority: 2,
    tone: "neutral",
    metric: "+18% conversion",
  },
  {
    id: "dashboard-campaign-widget",
    placement: "dashboard",
    eyebrow: "Campaign Widget",
    title: "Chiến dịch data + thiết bị",
    body: "Theo dõi impression, click và doanh thu từ cùng một dashboard.",
    ctaLabel: "Phân tích",
    href: "/admin/AdminDashboard",
    enabled: true,
    priority: 1,
    tone: "info",
    metric: "CTR 7.4%",
  },
  {
    id: "voucher-pay-later",
    placement: "voucher",
    eyebrow: "Voucher",
    title: "Ưu đãi trả sau theo tháng",
    body: "Khuyến khích thanh toán gói tháng bằng voucher có giới hạn.",
    ctaLabel: "Áp dụng",
    href: "/billpayment",
    enabled: true,
    priority: 3,
    tone: "warning",
    metric: "Tiết kiệm 12%",
  },
  {
    id: "sidebar-router-plan",
    placement: "sidebar",
    eyebrow: "Sidebar Promotion",
    title: "Router 5G + SIM doanh nghiệp",
    body: "Đặt nhẹ trong sidebar, không chen vào luồng thao tác chính.",
    ctaLabel: "Tư vấn",
    href: "/features/enterprise",
    enabled: true,
    priority: 1,
    tone: "neutral",
  },
  {
    id: "cross-sell-accessory",
    placement: "cross-sell",
    eyebrow: "Cross Selling",
    title: "Phụ kiện bảo vệ thiết bị",
    body: "Gợi ý đúng lúc sau khi người dùng đã quan tâm sản phẩm chính.",
    ctaLabel: "Thêm vào giỏ",
    href: "/shop?category=Accessory",
    enabled: true,
    priority: 2,
    tone: "info",
  },
  {
    id: "upsell-monthly-plan",
    placement: "upsell",
    eyebrow: "Upsell",
    title: "Nâng cấp lên gói tháng",
    body: "Hiển thị khi khách đang xem thiết bị hoặc gói data tương thích.",
    ctaLabel: "Nâng cấp",
    href: "/P5GDataPlan",
    enabled: true,
    priority: 1,
    tone: "brand",
  },
];

function getStorageKey(event: PromotionTrackEvent) {
  return `fabu_promotion_${event.eventType}_${event.creativeId}`;
}

async function postTrackingEvent(event: PromotionTrackEvent) {
  const endpoint = process.env.NEXT_PUBLIC_AD_TRACKING_ENDPOINT;
  if (!endpoint || typeof window === "undefined") return;

  try {
    await fetch(endpoint, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(event),
      keepalive: true,
    });
  } catch {
    window.localStorage.setItem(getStorageKey(event), JSON.stringify(event));
  }
}

export const promotionService = {
  async getActivePromotions() {
    const endpoint = process.env.NEXT_PUBLIC_CMS_PROMOTION_ENDPOINT;
    if (!endpoint) {
      return fallbackPromotions.filter((item) => item.enabled);
    }

    try {
      const response = await fetch(endpoint, { next: { revalidate: 60 } });
      if (!response.ok) throw new Error("Promotion CMS request failed");
      const data = (await response.json()) as PromotionCreative[];
      return data
        .filter((item) => item.enabled)
        .sort((a, b) => a.priority - b.priority);
    } catch {
      return fallbackPromotions.filter((item) => item.enabled);
    }
  },

  track(event: Omit<PromotionTrackEvent, "occurredAt">) {
    const payload = {
      ...event,
      occurredAt: new Date().toISOString(),
    };

    if (typeof window !== "undefined") {
      window.localStorage.setItem(getStorageKey(payload), JSON.stringify(payload));
    }

    void postTrackingEvent(payload);
  },
};
