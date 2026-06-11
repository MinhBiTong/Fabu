"use client";

import dynamic from "next/dynamic";
import { useVirtualizer } from "@tanstack/react-virtual";
import { AnimatePresence, motion, useReducedMotion } from "framer-motion";
import {
  BarChart3,
  Bell,
  ChevronRight,
  Megaphone,
  Package,
  RefreshCcw,
  ShieldCheck,
  Truck,
  WalletCards,
  Zap,
  X,
} from "lucide-react";
import { memo, useCallback, useMemo, useRef, useState } from "react";
import { toast } from "react-toastify";
import { MotionCard } from "@/components/motion/MotionCard";
import { MotionPage } from "@/components/motion/MotionPage";
import { AnimatedNumber } from "@/components/shared/AnimatedNumber";
import { PromotionSlot } from "@/components/shared/PromotionSlot";
import { Button } from "@/components/ui/button";
import { ChartSkeleton } from "@/components/ui/skeleton";
import { cn } from "@/lib/utils/utils";
import { drawerSlide, scaleIn } from "@/lib/animation/motion-presets";
import { dashboardProfiles } from "./dashboard-data";
import type {
  ActivityItem,
  DashboardKind,
  DashboardProfile,
  Insight,
  KpiCardModel,
} from "./dashboard-types";

const RevenueTrendChart = dynamic(
  () => import("./DashboardCharts").then((module) => module.RevenueTrendChart),
  { ssr: false, loading: () => <ChartSkeleton /> }
);
const OrderBarChart = dynamic(
  () => import("./DashboardCharts").then((module) => module.OrderBarChart),
  { ssr: false, loading: () => <ChartSkeleton /> }
);
const ChannelMixChart = dynamic(
  () => import("./DashboardCharts").then((module) => module.ChannelMixChart),
  { ssr: false, loading: () => <ChartSkeleton /> }
);

const dashboardIcons: Record<DashboardKind, typeof BarChart3> = {
  executive: ShieldCheck,
  revenue: BarChart3,
  realtime: Zap,
  merchant: Package,
  rider: Truck,
};

function formatKpiValue(value: number, formatter: KpiCardModel["formatter"]) {
  if (formatter === "currency") {
    return `${Math.round(value / 1000000).toLocaleString("vi-VN")}M`;
  }

  if (formatter === "percent") {
    return `${value.toFixed(1)}%`;
  }

  if (formatter === "duration") {
    return `${value.toFixed(value < 10 ? 1 : 0)} phút`;
  }

  return Math.round(value).toLocaleString("vi-VN");
}

function sentimentClass(sentiment: KpiCardModel["sentiment"]) {
  if (sentiment === "positive") return "bg-[#EAF8F2] text-[#03A678]";
  if (sentiment === "warning") return "bg-[#FFF5E6] text-fabu-orange";
  return "bg-fabu-muted text-fabu-gray";
}

function statusClass(status: ActivityItem["status"]) {
  if (status === "success") return "bg-[#EAF8F2] text-[#03A678]";
  if (status === "warning") return "bg-[#FFF5E6] text-fabu-orange";
  return "bg-fabu-muted text-fabu-gray";
}

const DashboardTabs = memo(function DashboardTabs({
  active,
  onChange,
}: {
  active: DashboardKind;
  onChange: (next: DashboardKind) => void;
}) {
  return (
    <div className="flex gap-2 overflow-x-auto rounded-card border border-fabu-border bg-white p-2 shadow-subtle">
      {dashboardProfiles.map((profile) => {
        const Icon = dashboardIcons[profile.id];
        const selected = active === profile.id;
        return (
          <button
            key={profile.id}
            type="button"
            className={cn(
              "flex min-h-11 shrink-0 items-center gap-2 rounded px-4 text-sm font-semibold transition",
              selected
                ? "bg-fabu-red text-white"
                : "text-fabu-charcoal hover:bg-fabu-muted hover:text-fabu-red"
            )}
            onClick={() => onChange(profile.id)}
          >
            <Icon className="h-4 w-4" />
            {profile.label}
          </button>
        );
      })}
    </div>
  );
});

const KpiCard = memo(function KpiCard({ item }: { item: KpiCardModel }) {
  return (
    <MotionCard className="min-h-40 p-5">
      <div className="flex items-start justify-between gap-3">
        <p className="text-sm font-semibold text-fabu-gray">{item.label}</p>
        <span className={cn("rounded-full px-3 py-1 text-xs font-bold", sentimentClass(item.sentiment))}>
          {item.change}
        </span>
      </div>
      <p className="mt-5 text-3xl font-bold text-fabu-ink">
        <AnimatedNumber
          value={item.value}
          formatter={(value) => formatKpiValue(value, item.formatter)}
        />
      </p>
      <div className="mt-5 h-2 overflow-hidden rounded-full bg-fabu-muted">
        <motion.div
          className="h-full rounded-full bg-fabu-red"
          initial={{ width: "12%" }}
          animate={{ width: `${Math.min(96, Math.max(28, item.value % 100))}%` }}
          transition={{ duration: 0.55, ease: "easeOut" }}
        />
      </div>
    </MotionCard>
  );
});

function ChartPanel({ profile }: { profile: DashboardProfile }) {
  return (
    <div className="grid gap-5 xl:grid-cols-[1.25fr_0.75fr]">
      <MotionCard className="min-h-[390px] p-5 md:p-6">
        <div className="mb-5 flex flex-col gap-2 sm:flex-row sm:items-end sm:justify-between">
          <div>
            <h2 className="text-2xl">Trend Analysis</h2>
            <p className="mt-1 text-sm text-fabu-gray">Doanh thu và vận hành theo chu kỳ gần nhất.</p>
          </div>
          <Button variant="outline" size="sm">
            Export
          </Button>
        </div>
        <div className="h-[300px]">
          <RevenueTrendChart data={profile.trends} />
        </div>
      </MotionCard>

      <div className="grid gap-5">
        <MotionCard className="min-h-[190px] p-5" interactive={false}>
          <h3 className="text-xl">Realtime Counters</h3>
          <div className="mt-5 grid grid-cols-2 gap-3">
            <div className="rounded-card bg-fabu-muted p-4">
              <p className="text-xs font-bold uppercase text-fabu-gray">Live orders</p>
              <p className="mt-2 text-2xl font-bold text-fabu-ink">
                <AnimatedNumber value={86} />
              </p>
            </div>
            <div className="rounded-card bg-fabu-info p-4">
              <p className="text-xs font-bold uppercase text-fabu-link">Payment OK</p>
              <p className="mt-2 text-2xl font-bold text-fabu-ink">
                <AnimatedNumber value={97.2} formatter={(value) => `${value.toFixed(1)}%`} />
              </p>
            </div>
          </div>
        </MotionCard>

        <MotionCard className="min-h-[180px] p-5" interactive={false}>
          <h3 className="text-xl">Channel Mix</h3>
          <div className="mt-3 grid h-[180px] grid-cols-[1fr_0.9fr] items-center gap-3">
            <ChannelMixChart data={profile.channels} />
            <div className="grid gap-2">
              {profile.channels.map((item) => (
                <div key={item.name} className="flex items-center justify-between gap-2 text-sm">
                  <span className="truncate text-fabu-gray">{item.name}</span>
                  <span className="font-bold text-fabu-ink">{item.value}%</span>
                </div>
              ))}
            </div>
          </div>
        </MotionCard>
      </div>
    </div>
  );
}

function ActivityVirtualTable({
  items,
  onInspect,
}: {
  items: ActivityItem[];
  onInspect: (item: ActivityItem) => void;
}) {
  const parentRef = useRef<HTMLDivElement | null>(null);
  // eslint-disable-next-line react-hooks/incompatible-library
  const rowVirtualizer = useVirtualizer({
    count: items.length,
    getScrollElement: () => parentRef.current,
    estimateSize: () => 72,
    overscan: 5,
  });

  return (
    <div className="fabu-dashboard-card">
      <div className="mb-4 flex items-center justify-between gap-3">
        <div>
          <h2 className="text-2xl">Activity Feed</h2>
          <p className="mt-1 text-sm text-fabu-gray">Virtualized để giữ dashboard mượt khi feed dài.</p>
        </div>
        <span className="flex items-center gap-2 rounded-full bg-fabu-muted px-3 py-1 text-xs font-bold text-fabu-gray">
          <span className="fabu-live-dot h-2 w-2 rounded-full bg-fabu-red" />
          Live
        </span>
      </div>

      <div ref={parentRef} className="h-[360px] overflow-auto rounded-card border border-fabu-border">
        <div
          className="relative w-full"
          style={{ height: `${rowVirtualizer.getTotalSize()}px` }}
        >
          {rowVirtualizer.getVirtualItems().map((virtualRow) => {
            const item = items[virtualRow.index];
            return (
              <div
                key={item.id}
                className="absolute left-0 top-0 w-full border-b border-fabu-border bg-white px-4 py-3"
                style={{ transform: `translateY(${virtualRow.start}px)` }}
              >
                <div className="grid gap-3 md:grid-cols-[72px_1fr_auto] md:items-center">
                  <span className="text-sm font-bold text-fabu-gray">{item.time}</span>
                  <div>
                    <p className="text-sm font-bold text-fabu-ink">{item.actor}</p>
                    <p className="mt-1 text-sm text-fabu-gray">{item.action}</p>
                  </div>
                  <div className="flex items-center gap-2">
                    <span className={cn("rounded-full px-3 py-1 text-xs font-bold", statusClass(item.status))}>
                      {item.status}
                    </span>
                    <Button variant="ghost" size="sm" onClick={() => onInspect(item)}>
                      Inspect
                    </Button>
                  </div>
                </div>
              </div>
            );
          })}
        </div>
      </div>
    </div>
  );
}

function RankingsPanel({ profile }: { profile: DashboardProfile }) {
  return (
    <MotionCard className="p-5 md:p-6" interactive={false}>
      <h2 className="text-2xl">Top Rankings</h2>
      <div className="mt-5 grid gap-3">
        {profile.rankings.map((item, index) => (
          <div key={item.name} className="flex items-center justify-between gap-3 rounded-card bg-fabu-muted p-4">
            <div className="flex min-w-0 items-center gap-3">
              <span className="flex h-9 w-9 shrink-0 items-center justify-center rounded-full bg-white text-sm font-bold text-fabu-red">
                {index + 1}
              </span>
              <div className="min-w-0">
                <p className="truncate text-sm font-bold text-fabu-ink">{item.name}</p>
                <p className="text-xs text-fabu-gray">{item.change}</p>
              </div>
            </div>
            <span className="shrink-0 text-sm font-bold text-fabu-ink">{item.value}</span>
          </div>
        ))}
      </div>
    </MotionCard>
  );
}

function InsightList({
  insights,
  onOpen,
}: {
  insights: Insight[];
  onOpen: (insight: Insight) => void;
}) {
  return (
    <MotionCard className="p-5 md:p-6" interactive={false}>
      <div className="flex items-center justify-between gap-3">
        <div>
          <h2 className="text-2xl">Progressive Insights</h2>
          <p className="mt-1 text-sm text-fabu-gray">Mở chi tiết khi cần, không làm dashboard quá tải.</p>
        </div>
        <Bell className="h-5 w-5 text-fabu-red" />
      </div>

      <div className="mt-5 grid gap-3">
        {insights.map((insight) => (
          <button
            key={insight.id}
            type="button"
            className="flex min-h-16 w-full items-center justify-between gap-4 rounded-card border border-fabu-border bg-white p-4 text-left transition hover:border-fabu-red hover:shadow-subtle"
            onClick={() => onOpen(insight)}
          >
            <div>
              <p className="text-sm font-bold text-fabu-ink">{insight.title}</p>
              <p className="mt-1 line-clamp-1 text-sm text-fabu-gray">{insight.body}</p>
            </div>
            <ChevronRight className="h-4 w-4 shrink-0 text-fabu-gray" />
          </button>
        ))}
      </div>
    </MotionCard>
  );
}

function QuickActionsPanel({
  profile,
  onOpenModal,
}: {
  profile: DashboardProfile;
  onOpenModal: () => void;
}) {
  const actions = useMemo(
    () => [
      {
        id: "refresh",
        label: "Refresh signals",
        description: "Cập nhật counters và feed",
        icon: RefreshCcw,
      },
      {
        id: "campaign",
        label: "Launch campaign",
        description: "Mở modal chiến dịch",
        icon: Megaphone,
      },
      {
        id: "billing",
        label: "Review payments",
        description: "Kiểm tra trả trước/trả sau",
        icon: WalletCards,
      },
    ],
    []
  );

  const handleAction = useCallback(
    (actionId: string) => {
      if (actionId === "campaign") {
        onOpenModal();
        return;
      }

      toast.info(`${profile.label}: ${actionId} đã được đưa vào hàng chờ xử lý.`);
    },
    [onOpenModal, profile.label]
  );

  return (
    <MotionCard className="p-5 md:p-6" interactive={false}>
      <h2 className="text-2xl">Quick Actions</h2>
      <div className="mt-5 grid gap-3">
        {actions.map((action) => {
          const Icon = action.icon;
          return (
            <button
              key={action.id}
              type="button"
              className="flex min-h-16 items-center gap-3 rounded-card border border-fabu-border bg-white p-4 text-left transition hover:border-fabu-red hover:bg-fabu-muted active:scale-[0.99]"
              onClick={() => handleAction(action.id)}
            >
              <span className="flex h-11 w-11 shrink-0 items-center justify-center rounded-full bg-fabu-muted text-fabu-red">
                <Icon className="h-5 w-5" />
              </span>
              <span>
                <span className="block text-sm font-bold text-fabu-ink">{action.label}</span>
                <span className="mt-1 block text-sm text-fabu-gray">{action.description}</span>
              </span>
            </button>
          );
        })}
      </div>
    </MotionCard>
  );
}

function InsightDrawer({
  insight,
  onClose,
}: {
  insight: Insight | ActivityItem | null;
  onClose: () => void;
}) {
  return (
    <AnimatePresence>
      {insight ? (
        <motion.div
          className="fixed inset-0 z-50 flex justify-end bg-black/35"
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          exit={{ opacity: 0 }}
          onMouseDown={onClose}
        >
          <motion.aside
            variants={drawerSlide}
            initial="hidden"
            animate="show"
            exit="exit"
            className="h-full w-full max-w-xl overflow-auto bg-white p-6 shadow-modal"
            onMouseDown={(event) => event.stopPropagation()}
          >
            <div className="flex items-start justify-between gap-4">
              <div>
                <p className="text-sm font-bold uppercase text-fabu-red">Contextual detail</p>
                <h2 className="mt-2 text-2xl">
                  {"title" in insight ? insight.title : insight.actor}
                </h2>
              </div>
              <button
                type="button"
                className="flex h-11 w-11 items-center justify-center rounded-full bg-fabu-muted hover:text-fabu-red"
                onClick={onClose}
                aria-label="Close detail drawer"
              >
                <X className="h-5 w-5" />
              </button>
            </div>
            <p className="mt-6 text-sm leading-7 text-fabu-gray">
              {"body" in insight ? insight.body : `${insight.action}. Giá trị giao dịch: ${insight.amount.toLocaleString("vi-VN")}đ.`}
            </p>
            <div className="mt-6 rounded-card bg-fabu-muted p-4">
              <p className="text-sm font-bold text-fabu-ink">Suggested next step</p>
              <p className="mt-2 text-sm leading-6 text-fabu-gray">
                Gắn action này vào queue phù hợp, chỉ mở chi tiết khi người vận hành cần ra quyết định.
              </p>
            </div>
          </motion.aside>
        </motion.div>
      ) : null}
    </AnimatePresence>
  );
}

function CampaignModal({ open, onClose }: { open: boolean; onClose: () => void }) {
  return (
    <AnimatePresence>
      {open ? (
        <motion.div
          className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 px-4"
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          exit={{ opacity: 0 }}
          onMouseDown={onClose}
        >
          <motion.div
            variants={scaleIn}
            initial="hidden"
            animate="show"
            exit="exit"
            className="w-full max-w-lg rounded-card bg-white p-6 shadow-modal"
            onMouseDown={(event) => event.stopPropagation()}
          >
            <div className="flex items-start justify-between gap-4">
              <div>
                <p className="text-sm font-bold uppercase text-fabu-red">Campaign Widget</p>
                <h2 className="mt-2 text-2xl">Bundle thiết bị + gói tháng</h2>
              </div>
              <button
                type="button"
                className="flex h-11 w-11 items-center justify-center rounded-full bg-fabu-muted hover:text-fabu-red"
                onClick={onClose}
                aria-label="Close campaign modal"
              >
                <X className="h-5 w-5" />
              </button>
            </div>
            <div className="mt-5 grid gap-3 rounded-card bg-fabu-muted p-4">
              <div className="flex items-center justify-between gap-3">
                <span className="text-sm text-fabu-gray">Budget pacing</span>
                <span className="font-bold text-fabu-ink">72%</span>
              </div>
              <div className="h-2 rounded-full bg-white">
                <div className="h-2 w-[72%] rounded-full bg-fabu-red" />
              </div>
            </div>
            <div className="mt-6 flex flex-col gap-3 sm:flex-row">
              <Button onClick={() => toast.success("Campaign đã được đưa vào lịch chạy.")}>
                Schedule
              </Button>
              <Button variant="outline" onClick={onClose}>
                Review later
              </Button>
            </div>
          </motion.div>
        </motion.div>
      ) : null}
    </AnimatePresence>
  );
}

export function AdminCommerceDashboard() {
  const [activeDashboard, setActiveDashboard] = useState<DashboardKind>("executive");
  const [drawerItem, setDrawerItem] = useState<Insight | ActivityItem | null>(null);
  const [campaignOpen, setCampaignOpen] = useState(false);
  const reduceMotion = useReducedMotion();

  const profile = useMemo(
    () => dashboardProfiles.find((item) => item.id === activeDashboard) ?? dashboardProfiles[0],
    [activeDashboard]
  );

  const handleDashboardChange = useCallback((next: DashboardKind) => {
    setActiveDashboard(next);
  }, []);

  return (
    <MotionPage className="fabu-page">
      <section className="fabu-section bg-fabu-muted">
        <div className="fabu-container grid gap-6">
          <PromotionSlot placement="top-banner" />

          <motion.div
            className="overflow-hidden rounded-card bg-fabu-red p-6 text-white shadow-prominent md:p-8"
            initial={reduceMotion ? false : { opacity: 0, y: 12 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.28 }}
          >
            <div className="grid gap-6 lg:grid-cols-[1fr_auto] lg:items-end">
              <div>
                <span className="inline-flex items-center gap-2 rounded-full bg-white/15 px-3 py-1 text-xs font-bold uppercase text-white">
                  <span className="fabu-live-dot h-2 w-2 rounded-full bg-white" />
                  Realtime Dashboard
                </span>
                <h1 className="mt-4 text-white">Fabu Commerce Operations</h1>
                <p className="mt-3 max-w-3xl text-sm leading-7 text-white/90 md:text-base">
                  Executive, revenue, realtime, merchant và rider dashboard trong một layout vận hành,
                  có promotion placement và contextual action đúng ngữ cảnh.
                </p>
              </div>
              <Button variant="secondary" onClick={() => toast.info("Route prefetch đã sẵn qua Next Link.")}>
                Optimize routes
              </Button>
            </div>
          </motion.div>

          <DashboardTabs active={activeDashboard} onChange={handleDashboardChange} />

          <div>
            <h2 className="text-2xl">{profile.label} Dashboard</h2>
            <p className="mt-2 max-w-3xl text-sm leading-6 text-fabu-gray">{profile.description}</p>
          </div>

          <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
            {profile.kpis.map((item) => (
              <KpiCard key={item.id} item={item} />
            ))}
          </div>

          <div className="grid gap-6 xl:grid-cols-[1fr_320px]">
            <div className="grid gap-6">
              <ChartPanel profile={profile} />

              <div className="grid gap-6 xl:grid-cols-[1fr_0.8fr]">
                <ActivityVirtualTable items={profile.activity} onInspect={setDrawerItem} />
                <div className="grid gap-6">
                  <RankingsPanel profile={profile} />
                  <InsightList insights={profile.insights} onOpen={setDrawerItem} />
                </div>
              </div>

              <MotionCard className="min-h-[300px] p-5 md:p-6" interactive={false}>
                <div className="mb-5 flex flex-col gap-2 sm:flex-row sm:items-end sm:justify-between">
                  <div>
                    <h2 className="text-2xl">Orders vs. Operations</h2>
                    <p className="mt-1 text-sm text-fabu-gray">Chart animation nhẹ cho so sánh tải vận hành.</p>
                  </div>
                </div>
                <div className="h-[260px]">
                  <OrderBarChart data={profile.trends} />
                </div>
              </MotionCard>
            </div>

            <aside className="grid content-start gap-5">
              <PromotionSlot placement="dashboard" compact />
              <PromotionSlot placement="campaign" compact />
              <PromotionSlot placement="sidebar" compact />
              <QuickActionsPanel profile={profile} onOpenModal={() => setCampaignOpen(true)} />
            </aside>
          </div>
        </div>
      </section>

      <InsightDrawer insight={drawerItem} onClose={() => setDrawerItem(null)} />
      <CampaignModal open={campaignOpen} onClose={() => setCampaignOpen(false)} />
    </MotionPage>
  );
}
