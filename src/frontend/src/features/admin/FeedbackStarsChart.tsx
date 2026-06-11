"use client";

import { Bar, BarChart, ResponsiveContainer, Tooltip, XAxis, YAxis } from "recharts";

type FeedbackStarsChartProps = {
  data: Array<{ star: string; total: number }>;
};

export function FeedbackStarsChart({ data }: FeedbackStarsChartProps) {
  return (
    <ResponsiveContainer width="100%" height="100%" minWidth={0}>
      <BarChart data={data} layout="vertical">
        <XAxis type="number" />
        <YAxis dataKey="star" type="category" />
        <Tooltip />
        <Bar dataKey="total" fill="#EE0033" radius={[0, 8, 8, 0]} />
      </BarChart>
    </ResponsiveContainer>
  );
}
