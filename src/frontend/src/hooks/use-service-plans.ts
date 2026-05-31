"use client";

import { useEffect, useMemo, useState } from "react";
import { useServicePlanStore } from "@/store/service-plan.store";

export const useServicePlans = () => {
  const { plans, isLoading, error, loadPlans } = useServicePlanStore();
  const [sortOrder, setSortOrder] = useState<"asc" | "desc">("asc");
  const [selectedCategory, setSelectedCategory] = useState("All");
  const [expandedCategories, setExpandedCategories] = useState<string[]>([]);

  useEffect(() => {
    loadPlans();
  }, [loadPlans]);

  const categories = useMemo(
    () => Array.from(new Set(plans.map((plan) => plan.category).filter(Boolean))),
    [plans]
  );

  const groupedPlans = useMemo(() => {
    return plans
      .filter((plan) => selectedCategory === "All" || plan.category === selectedCategory)
      .reduce<Record<string, typeof plans>>((acc, plan) => {
        const category = plan.category || "Other";
        acc[category] ??= [];
        acc[category].push(plan);
        return acc;
      }, {});
  }, [plans, selectedCategory]);

  const sortedGroups = useMemo(() => {
    return Object.entries(groupedPlans).map(([category, items]) => ({
      category,
      plans: [...items].sort((a, b) =>
        sortOrder === "asc" ? a.price - b.price : b.price - a.price
      ),
      isExpanded: expandedCategories.includes(category),
    }));
  }, [expandedCategories, groupedPlans, sortOrder]);

  return {
    categories,
    groups: sortedGroups,
    selectedCategory,
    setSelectedCategory,
    expandedCategories,
    setExpandedCategories,
    sortOrder,
    toggleSort: () => setSortOrder((order) => (order === "asc" ? "desc" : "asc")),
    toggleCategory: (category: string) =>
      setExpandedCategories((current) =>
        current.includes(category)
          ? current.filter((item) => item !== category)
          : [...current, category]
      ),
    isLoading,
    error,
  };
};
