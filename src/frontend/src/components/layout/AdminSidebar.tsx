"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { memo, useState } from "react";
import { useAuth } from "@/hooks/use-auth";

const adminLinks = [
  { href: "/admin/AdminDashboard", label: "Dashboard" },
  { href: "/admin/AdminTransactions", label: "Transactions" },
  { href: "/admin/AdminFeedbacks", label: "Feedbacks" },
  { href: "/admin/AdminPackages", label: "Packages" },
  { href: "/admin/AdminAccounts", label: "Accounts" },
];

function AdminSidebar() {
  const pathname = usePathname();
  const { hasRole } = useAuth();
  const [isOpen, setIsOpen] = useState(false);

  if (!pathname.startsWith("/admin")) return null;

  return (
    <>
      <button
        type="button"
        className="fixed left-4 top-28 z-30 flex h-11 w-11 items-center justify-center rounded-full border border-fabu-border bg-white shadow-elevated hover:border-fabu-red hover:text-fabu-red"
        onClick={() => setIsOpen((value) => !value)}
        aria-label="Toggle admin navigation"
      >
        {isOpen ? "<" : ">"}
      </button>

      <aside
        className={`fixed left-0 top-20 z-20 h-[calc(100vh-80px)] w-64 border-r border-fabu-border bg-white p-4 shadow-elevated transition-transform ${
          isOpen ? "translate-x-0" : "-translate-x-full"
        }`}
      >
        <div className="mb-4">
          <h3 className="text-lg">Admin</h3>
          <p className="text-xs text-fabu-gray">
            {hasRole("Admin") ? "Full access" : "Operational view"}
          </p>
        </div>
        <nav className="grid gap-2">
          {adminLinks.map((link) => {
            const active = pathname.startsWith(link.href);
            return (
              <Link
                key={link.href}
                href={link.href}
                className={`flex min-h-11 items-center rounded px-3 text-sm transition ${
                  active
                    ? "bg-fabu-red text-white"
                    : "text-fabu-charcoal hover:bg-fabu-muted hover:text-fabu-red"
                }`}
                onClick={() => setIsOpen(false)}
              >
                {link.label}
              </Link>
            );
          })}
        </nav>
      </aside>
    </>
  );
}

export default memo(AdminSidebar);
