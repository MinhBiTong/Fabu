"use client";

import Link from "next/link";
import { useMemo, useState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { AdminTable } from "@/features/admin/AdminTable";
import { useServicePlans } from "@/hooks/use-service-plans";
import { formatCurrency } from "@/lib/utils/format";

const itemsPerPage = 10;

export default function AdminPackagesPage() {
  const { groups, categories, isLoading, error } = useServicePlans();
  const allPlans = useMemo(() => groups.flatMap((group) => group.plans), [groups]);
  const [searchTerm, setSearchTerm] = useState("");
  const [selectedCategory, setSelectedCategory] = useState("");
  const [currentPage, setCurrentPage] = useState(1);

  const filteredPackages = useMemo(() => {
    return allPlans.filter((pkg) => {
      const matchSearch = pkg.serviceName.toLowerCase().includes(searchTerm.toLowerCase());
      const matchCategory = !selectedCategory || pkg.category === selectedCategory;
      return matchSearch && matchCategory;
    });
  }, [allPlans, searchTerm, selectedCategory]);

  const totalPages = Math.max(Math.ceil(filteredPackages.length / itemsPerPage), 1);
  const currentPackages = filteredPackages.slice(
    (currentPage - 1) * itemsPerPage,
    currentPage * itemsPerPage
  );

  return (
    <section className="fabu-section">
      <div className="fabu-container grid gap-6">
        <div className="flex flex-col gap-4 md:flex-row md:items-end md:justify-between">
          <div>
            <h1>Packages</h1>
            <p className="mt-2 text-sm text-fabu-gray">
              Manage backend `Service` records.
            </p>
          </div>
          <Button asChild>
            <Link href="/admin/AdminPackages/AddPackage">Add Package</Link>
          </Button>
        </div>

        <div className="grid gap-3 md:grid-cols-[1fr_220px]">
          <Input
            placeholder="Search package name"
            value={searchTerm}
            onChange={(event) => {
              setSearchTerm(event.target.value);
              setCurrentPage(1);
            }}
          />
          <select
            className="fabu-input"
            value={selectedCategory}
            onChange={(event) => {
              setSelectedCategory(event.target.value);
              setCurrentPage(1);
            }}
          >
            <option value="">All Categories</option>
            {categories.map((category) => (
              <option key={category} value={category}>
                {category}
              </option>
            ))}
          </select>
        </div>

        {error ? <p className="fabu-error">{error}</p> : null}

        <AdminTable
          headers={["Package Name", "Amount", "Category", "Price", "Length", "Options"]}
          isEmpty={!isLoading && currentPackages.length === 0}
          empty="No packages found"
        >
          {currentPackages.map((pkg) => (
            <tr key={pkg.id} className="border-t border-fabu-border">
              <td className="p-4 font-semibold">{pkg.serviceName}</td>
              <td className="p-4">{pkg.dataAmountMB.toLocaleString()} MB</td>
              <td className="p-4">{pkg.category}</td>
              <td className="p-4">{formatCurrency(pkg.price)}</td>
              <td className="p-4">{pkg.validityDays} days</td>
              <td className="p-4">
                <Link
                  className="font-semibold text-fabu-red hover:text-fabu-red-hover"
                  href={`/admin/AdminPackages/PackagesDetails/${pkg.id}`}
                >
                  Details
                </Link>
              </td>
            </tr>
          ))}
        </AdminTable>

        <div className="flex items-center justify-center gap-3">
          <Button
            variant="outline"
            onClick={() => setCurrentPage((page) => Math.max(page - 1, 1))}
            disabled={currentPage === 1}
          >
            Previous
          </Button>
          <span className="text-sm text-fabu-gray">
            {currentPage} / {totalPages}
          </span>
          <Button
            variant="outline"
            onClick={() => setCurrentPage((page) => Math.min(page + 1, totalPages))}
            disabled={currentPage === totalPages}
          >
            Next
          </Button>
        </div>
      </div>
    </section>
  );
}
