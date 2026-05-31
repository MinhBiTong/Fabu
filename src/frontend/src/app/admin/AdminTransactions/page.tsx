"use client";

import Link from "next/link";
import { useMemo, useState } from "react";
import { Input } from "@/components/ui/input";
import { AdminTable } from "@/features/admin/AdminTable";

const rows = [
  {
    id: "RCH-DEMO-001",
    email: "customer@fabu.vn",
    username: "fabu_customer",
    serviceType: "Recharge",
    price: "100,000 VND",
    status: "Success",
  },
  {
    id: "DATA-DEMO-002",
    email: "data@fabu.vn",
    username: "data_user",
    serviceType: "Data Plan",
    price: "90,000 VND",
    status: "Pending",
  },
];

export default function AdminTransactionsPage() {
  const [search, setSearch] = useState("");
  const [status, setStatus] = useState("");

  const filteredRows = useMemo(
    () =>
      rows.filter((row) => {
        const matchSearch = `${row.email} ${row.username}`.toLowerCase().includes(search.toLowerCase());
        const matchStatus = !status || row.status === status;
        return matchSearch && matchStatus;
      }),
    [search, status]
  );

  return (
    <section className="fabu-section">
      <div className="fabu-container grid gap-6">
        <div>
          <h1>Transactions</h1>
          <p className="mt-2 text-sm text-fabu-gray">
            Recharge transaction creation is wired; list endpoint needs customerId from profile.
          </p>
        </div>

        <div className="grid gap-3 md:grid-cols-[1fr_220px]">
          <Input
            placeholder="Search username or email"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
          />
          <select className="fabu-input" value={status} onChange={(event) => setStatus(event.target.value)}>
            <option value="">Choose Status</option>
            <option value="Success">Success</option>
            <option value="Pending">Pending</option>
            <option value="Failed">Failed</option>
          </select>
        </div>

        <AdminTable
          headers={["Email", "Username", "Services Type", "Price", "Status", "Options"]}
          isEmpty={filteredRows.length === 0}
          empty="No transactions found"
        >
          {filteredRows.map((row) => (
            <tr key={row.id} className="border-t border-fabu-border">
              <td className="p-4">{row.email}</td>
              <td className="p-4">{row.username}</td>
              <td className="p-4">{row.serviceType}</td>
              <td className="p-4">{row.price}</td>
              <td className="p-4">{row.status}</td>
              <td className="p-4">
                <Link className="font-semibold text-fabu-red" href="/admin/AdminTransactions/TransactionsDetails">
                  Details
                </Link>
              </td>
            </tr>
          ))}
        </AdminTable>
      </div>
    </section>
  );
}
