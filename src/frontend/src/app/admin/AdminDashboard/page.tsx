"use client";

export const dynamic = "force-dynamic";

import {
  Bar,
  BarChart,
  CartesianGrid,
  Cell,
  Line,
  LineChart,
  Pie,
  PieChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";

const revenueData = [
  { name: "Jan", value: 1200 },
  { name: "Feb", value: 2100 },
  { name: "Mar", value: 800 },
  { name: "Apr", value: 1600 },
  { name: "May", value: 900 },
  { name: "Jun", value: 1700 },
  { name: "Jul", value: 1400 },
  { name: "Aug", value: 2200 },
  { name: "Sep", value: 1800 },
  { name: "Oct", value: 2000 },
  { name: "Nov", value: 1400 },
  { name: "Dec", value: 2300 },
];

const countryData = [
  { name: "Vietnam", value: 4005 },
  { name: "America", value: 12000 },
  { name: "China", value: 2240 },
  { name: "Arab", value: 13232 },
];

const channelData = [
  { name: "Data plans", value: 1200 },
  { name: "Recharges", value: 600 },
  { name: "Products", value: 2240 },
];

const chartColors = ["#EE0033", "#1890FF", "#FE9A00"];
const total = channelData.reduce((sum, item) => sum + item.value, 0);

const stats = [
  { label: "Total Money", value: "9,844 USD", trend: "increased 30%" },
  { label: "Total Orders", value: "98", trend: "increased 30%" },
  { label: "This Month", value: "2,300 USD", trend: "increased 18%" },
  { label: "Active Plans", value: "36", trend: "stable" },
];

export default function AdminDashboard() {
  return (
    <section className="fabu-section">
      <div className="fabu-container grid gap-6">
        <div>
          <h1>Admin Dashboard</h1>
          <p className="mt-2 text-sm text-fabu-gray">
            Operational overview with restrained Fabu styling.
          </p>
        </div>

        <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
          {stats.map((stat) => (
            <article key={stat.label} className="fabu-card">
              <p className="text-sm text-fabu-gray">{stat.label}</p>
              <p className="mt-3 text-3xl font-bold text-fabu-ink">{stat.value}</p>
              <p className="mt-3 text-sm text-fabu-red">{stat.trend}</p>
            </article>
          ))}
        </div>

        <div className="grid gap-6 xl:grid-cols-[1.2fr_0.8fr]">
          <div className="fabu-card h-[380px]">
            <h2 className="mb-5 text-2xl">Revenue</h2>
            <ResponsiveContainer width="100%" height="85%">
              <LineChart data={revenueData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="name" />
                <YAxis />
                <Tooltip />
                <Line type="monotone" dataKey="value" stroke="#EE0033" strokeWidth={2} dot={false} />
              </LineChart>
            </ResponsiveContainer>
          </div>

          <div className="fabu-card h-[380px]">
            <h2 className="mb-5 text-2xl">Channel Mix</h2>
            <ResponsiveContainer width="100%" height="70%">
              <PieChart>
                <Pie
                  data={channelData}
                  dataKey="value"
                  nameKey="name"
                  cx="50%"
                  cy="50%"
                  outerRadius={90}
                  label={({ value }) => `${((Number(value) / total) * 100).toFixed(0)}%`}
                >
                  {channelData.map((entry, index) => (
                    <Cell key={entry.name} fill={chartColors[index % chartColors.length]} />
                  ))}
                </Pie>
                <Tooltip />
              </PieChart>
            </ResponsiveContainer>
            <div className="grid gap-2">
              {channelData.map((item, index) => (
                <div key={item.name} className="flex items-center gap-2 text-sm">
                  <span
                    className="h-3 w-3 rounded"
                    style={{ backgroundColor: chartColors[index % chartColors.length] }}
                  />
                  <span>{item.name}</span>
                </div>
              ))}
            </div>
          </div>
        </div>

        <div className="fabu-card h-[360px]">
          <h2 className="mb-5 text-2xl">Country Volume</h2>
          <ResponsiveContainer width="100%" height="85%">
            <BarChart data={countryData}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="name" />
              <YAxis />
              <Tooltip />
              <Bar dataKey="value" fill="#1890FF" />
            </BarChart>
          </ResponsiveContainer>
        </div>
      </div>
    </section>
  );
}
