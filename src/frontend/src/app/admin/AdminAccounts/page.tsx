"use client";

import { useEffect, useMemo, useState } from "react";
import { Input } from "@/components/ui/input";
import { AdminTable } from "@/features/admin/AdminTable";
import { useUserStore } from "@/store/user.store";

export default function AdminAccountsPage() {
  const { users, loadUsers, isLoading, error } = useUserStore();
  const [search, setSearch] = useState("");

  useEffect(() => {
    loadUsers();
  }, [loadUsers]);

  const filteredUsers = useMemo(() => {
    return users.filter((user) => {
      const text = `${user.email} ${user.username ?? ""} ${user.userName ?? ""} ${
        user.fullName ?? ""
      }`.toLowerCase();
      return text.includes(search.toLowerCase());
    });
  }, [search, users]);

  return (
    <section className="fabu-section">
      <div className="fabu-container grid gap-6">
        <div>
          <h1>Accounts</h1>
          <p className="mt-2 text-sm text-fabu-gray">Users from `v1/Users`.</p>
        </div>

        <Input
          placeholder="Search email or name"
          value={search}
          onChange={(event) => setSearch(event.target.value)}
        />

        {error ? <p className="fabu-error">{error}</p> : null}

        <AdminTable
          headers={["Email", "Username", "Full Name", "Role", "Created", "Options"]}
          isEmpty={!isLoading && filteredUsers.length === 0}
          empty="No accounts found"
        >
          {filteredUsers.map((user, index) => (
            <tr key={`${user.email}-${index}`} className="border-t border-fabu-border">
              <td className="p-4">{user.email}</td>
              <td className="p-4">{user.username ?? user.userName ?? "N/A"}</td>
              <td className="p-4">{user.fullName ?? "N/A"}</td>
              <td className="p-4">{user.role ?? "Customer"}</td>
              <td className="p-4">
                {user.createdDate ? new Date(user.createdDate).toLocaleDateString("vi-VN") : "N/A"}
              </td>
              <td className="p-4 text-fabu-gray">Details</td>
            </tr>
          ))}
        </AdminTable>
      </div>
    </section>
  );
}
