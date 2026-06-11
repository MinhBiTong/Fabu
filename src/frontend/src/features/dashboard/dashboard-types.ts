import type { ReactNode } from "react";

export type DashboardKind = "executive" | "revenue" | "realtime" | "merchant" | "rider";

export type KpiCardModel = {
  id: string;
  label: string;
  value: number;
  formatter: "currency" | "number" | "percent" | "duration";
  change: string;
  sentiment: "positive" | "neutral" | "warning";
};

export type TrendPoint = {
  label: string;
  revenue: number;
  orders: number;
  conversion: number;
};

export type ChannelPoint = {
  name: string;
  value: number;
};

export type ActivityItem = {
  id: string;
  time: string;
  actor: string;
  action: string;
  amount: number;
  status: "success" | "pending" | "warning";
};

export type RankingItem = {
  name: string;
  value: string;
  change: string;
};

export type QuickAction = {
  id: string;
  label: string;
  description: string;
  icon: ReactNode;
};

export type Insight = {
  id: string;
  title: string;
  body: string;
  severity: "info" | "warning" | "success";
};

export type DashboardProfile = {
  id: DashboardKind;
  label: string;
  description: string;
  kpis: KpiCardModel[];
  trends: TrendPoint[];
  channels: ChannelPoint[];
  activity: ActivityItem[];
  rankings: RankingItem[];
  insights: Insight[];
};
