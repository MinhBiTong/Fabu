"use client";

import { useEffect } from "react";
import { useAuth } from "@/hooks/use-auth";
import { useUserStore } from "@/store/user.store";
import { LoadingState } from "@/components/common/LoadingState";

const placeholderTransactions = [
  { date: "2026-05-31", service: "Recharge", price: "100,000 VND", status: "Success" },
  { date: "2026-05-30", service: "5G Data", price: "90,000 VND", status: "Pending" },
  { date: "2026-05-29", service: "Bill payment", price: "250,000 VND", status: "Success" },
];

export default function ProfilePage() {
  const { profile, isAuthenticated, isLoading } = useAuth();
  const { currentUser, loadCurrentUser } = useUserStore();

  useEffect(() => {
    if (isAuthenticated) {
      loadCurrentUser();
    }
  }, [isAuthenticated, loadCurrentUser]);

  if (isLoading) return <LoadingState label="Loading profile..." />;

  const displayName = currentUser?.fullName || profile.username || "Guest";
  const email = currentUser?.email || profile.email || "Not signed in";
  const username = currentUser?.username || currentUser?.userName || profile.username || "N/A";

  return (
    <section className="fabu-section">
      <div className="fabu-container grid gap-6 lg:grid-cols-[0.8fr_1.2fr]">
        <aside className="fabu-card h-fit">
          <div className="flex h-24 w-24 items-center justify-center rounded-full bg-fabu-red text-3xl font-bold text-white">
            {displayName.slice(0, 1).toUpperCase()}
          </div>
          <h1 className="mt-5 text-3xl">{displayName}</h1>
          <div className="mt-6 grid gap-3 text-sm">
            <div>
              <span className="text-fabu-gray">Username</span>
              <p className="font-bold text-fabu-ink">{username}</p>
            </div>
            <div>
              <span className="text-fabu-gray">Email</span>
              <p className="font-bold text-fabu-ink">{email}</p>
            </div>
            <div>
              <span className="text-fabu-gray">Role</span>
              <p className="font-bold text-fabu-ink">{profile.roles.join(", ") || "Customer"}</p>
            </div>
          </div>
        </aside>

        <section className="fabu-card overflow-hidden p-0">
          <div className="border-b border-fabu-border p-6">
            <h2 className="text-2xl">Transactions</h2>
            <p className="mt-2 text-sm text-fabu-gray">
              Recent activity placeholder until customer transaction lookup is wired with customerId.
            </p>
          </div>
          <div className="overflow-x-auto">
            <table className="w-full min-w-[640px] border-collapse text-sm">
              <thead className="bg-fabu-muted text-left text-fabu-gray">
                <tr>
                  <th className="p-4">Date</th>
                  <th className="p-4">Service</th>
                  <th className="p-4">Price</th>
                  <th className="p-4">Status</th>
                </tr>
              </thead>
              <tbody>
                {placeholderTransactions.map((row) => (
                  <tr key={`${row.date}-${row.service}`} className="border-t border-fabu-border">
                    <td className="p-4">{row.date}</td>
                    <td className="p-4">{row.service}</td>
                    <td className="p-4">{row.price}</td>
                    <td className="p-4">
                      <span className="rounded-full bg-fabu-info px-3 py-1 text-xs font-bold text-fabu-link">
                        {row.status}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </section>
      </div>
    </section>
  );
}
