"use client";

import {
  Area,
  AreaChart,
  Bar,
  BarChart,
  CartesianGrid,
  Cell,
  Pie,
  PieChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";
import type { ChannelPoint, TrendPoint } from "./dashboard-types";

const chartColors = ["#EE0033", "#1890FF", "#FE9A00", "#576C8A"];

type TrendChartProps = {
  data: TrendPoint[];
};

type ChannelChartProps = {
  data: ChannelPoint[];
};

const currencyTick = (value: number) => `${Math.round(value / 1000000)}M`;

export function RevenueTrendChart({ data }: TrendChartProps) {
  return (
    <ResponsiveContainer width="100%" height="100%" minWidth={0}>
      <AreaChart data={data} margin={{ left: -12, right: 8, top: 10, bottom: 0 }}>
        <defs>
          <linearGradient id="fabuRevenue" x1="0" x2="0" y1="0" y2="1">
            <stop offset="5%" stopColor="#EE0033" stopOpacity={0.22} />
            <stop offset="95%" stopColor="#EE0033" stopOpacity={0} />
          </linearGradient>
        </defs>
        <CartesianGrid stroke="#EEEEEE" strokeDasharray="3 3" vertical={false} />
        <XAxis dataKey="label" tickLine={false} axisLine={false} tick={{ fill: "#666666", fontSize: 12 }} />
        <YAxis tickFormatter={currencyTick} tickLine={false} axisLine={false} tick={{ fill: "#666666", fontSize: 12 }} />
        <Tooltip
          formatter={(value) => [`${Number(value).toLocaleString("vi-VN")}đ`, "Doanh thu"]}
          contentStyle={{ borderRadius: 8, borderColor: "#D1D1D1" }}
        />
        <Area
          type="monotone"
          dataKey="revenue"
          stroke="#EE0033"
          strokeWidth={2}
          fill="url(#fabuRevenue)"
          animationDuration={650}
        />
      </AreaChart>
    </ResponsiveContainer>
  );
}

export function OrderBarChart({ data }: TrendChartProps) {
  return (
    <ResponsiveContainer width="100%" height="100%" minWidth={0}>
      <BarChart data={data} margin={{ left: -10, right: 8, top: 10, bottom: 0 }}>
        <CartesianGrid stroke="#EEEEEE" strokeDasharray="3 3" vertical={false} />
        <XAxis dataKey="label" tickLine={false} axisLine={false} tick={{ fill: "#666666", fontSize: 12 }} />
        <YAxis tickLine={false} axisLine={false} tick={{ fill: "#666666", fontSize: 12 }} />
        <Tooltip contentStyle={{ borderRadius: 8, borderColor: "#D1D1D1" }} />
        <Bar dataKey="orders" fill="#1890FF" radius={[8, 8, 0, 0]} animationDuration={620} />
      </BarChart>
    </ResponsiveContainer>
  );
}

export function ChannelMixChart({ data }: ChannelChartProps) {
  const total = data.reduce((sum, item) => sum + item.value, 0);

  return (
    <ResponsiveContainer width="100%" height="100%" minWidth={0}>
      <PieChart>
        <Pie
          data={data}
          dataKey="value"
          nameKey="name"
          innerRadius={58}
          outerRadius={88}
          paddingAngle={3}
          animationDuration={650}
          label={({ value }) => `${Math.round((Number(value) / total) * 100)}%`}
        >
          {data.map((entry, index) => (
            <Cell key={entry.name} fill={chartColors[index % chartColors.length]} />
          ))}
        </Pie>
        <Tooltip contentStyle={{ borderRadius: 8, borderColor: "#D1D1D1" }} />
      </PieChart>
    </ResponsiveContainer>
  );
}
