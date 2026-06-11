"use client";

import { useQuery } from "@tanstack/react-query";
import { useMemo } from "react";
import type { TelecomProduct } from "@/core/types/api.types";
import { commerceService, type ProductQuery } from "@/services/commerce-service";
import { fallbackProducts } from "./commerce-data";

export function useProducts(filters: ProductQuery) {
  const query = useQuery({
    queryKey: ["commerce-products", filters],
    queryFn: async () => {
      const response = await commerceService.getProducts(filters);
      return response.data;
    },
    placeholderData: (previous) => previous,
  });

  const products = useMemo<TelecomProduct[]>(() => {
    const source = query.data?.length ? query.data : fallbackProducts;
    const keyword = filters.keyword?.trim().toLowerCase();
    const category = filters.category?.trim();

    return source.filter((product) => {
      const matchesKeyword = keyword
        ? [product.name, product.category, product.brand, product.sku]
            .filter(Boolean)
            .some((value) => String(value).toLowerCase().includes(keyword))
        : true;
      const matchesCategory = category ? product.category === category : true;
      return matchesKeyword && matchesCategory;
    });
  }, [filters.category, filters.keyword, query.data]);

  const categories = useMemo(
    () => Array.from(new Set((query.data?.length ? query.data : fallbackProducts).map((item) => item.category))),
    [query.data]
  );

  return {
    ...query,
    products,
    categories,
    isUsingFallback: !query.data?.length,
  };
}
