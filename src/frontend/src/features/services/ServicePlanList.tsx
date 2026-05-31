"use client";

import { Button } from "@/components/ui/button";
import { EmptyState } from "@/components/common/EmptyState";
import { LoadingState } from "@/components/common/LoadingState";
import { useServicePlans } from "@/hooks/use-service-plans";
import { ServicePlanCard } from "./ServicePlanCard";

export function ServicePlanList() {
  const {
    categories,
    groups,
    selectedCategory,
    setSelectedCategory,
    setExpandedCategories,
    toggleCategory,
    toggleSort,
    sortOrder,
    isLoading,
    error,
  } = useServicePlans();

  if (isLoading) return <LoadingState label="Loading data plans..." />;

  if (error) {
    return <EmptyState title="Could not load plans" description={error} />;
  }

  return (
    <div className="grid gap-8">
      <div className="flex flex-col gap-4 lg:flex-row lg:items-center lg:justify-between">
        <div className="flex flex-wrap gap-2">
          <button
            type="button"
            className={`min-h-11 rounded px-4 text-sm ${
              selectedCategory === "All"
                ? "bg-fabu-red text-white"
                : "bg-fabu-muted text-fabu-charcoal hover:text-fabu-red"
            }`}
            onClick={() => {
              setSelectedCategory("All");
              setExpandedCategories([]);
            }}
          >
            All
          </button>
          {categories.map((category) => (
            <button
              type="button"
              key={category}
              className={`min-h-11 rounded px-4 text-sm ${
                selectedCategory === category
                  ? "bg-fabu-red text-white"
                  : "bg-fabu-muted text-fabu-charcoal hover:text-fabu-red"
              }`}
              onClick={() => {
                setSelectedCategory(category);
                setExpandedCategories([category]);
              }}
            >
              {category}
            </button>
          ))}
        </div>
        <Button variant="outline" onClick={toggleSort}>
          Price {sortOrder === "asc" ? "low to high" : "high to low"}
        </Button>
      </div>

      {groups.length === 0 ? (
        <EmptyState title="No plans found" description="Try another category." />
      ) : (
        groups.map((group) => {
          const visiblePlans = group.isExpanded ? group.plans : group.plans.slice(0, 3);
          return (
            <section key={group.category} className="grid gap-4">
              <div className="flex items-center justify-between gap-4">
                <h2 className="text-2xl">{group.category}</h2>
                <Button variant="link" onClick={() => toggleCategory(group.category)}>
                  {group.isExpanded ? "Show less" : "View all"}
                </Button>
              </div>
              <div className="grid gap-5 md:grid-cols-2 xl:grid-cols-3">
                {visiblePlans.map((plan) => (
                  <ServicePlanCard
                    key={plan.id}
                    plan={plan}
                    detailsHref={`/P5GDataPlan/Details/${plan.id}`}
                  />
                ))}
              </div>
            </section>
          );
        })
      )}
    </div>
  );
}
