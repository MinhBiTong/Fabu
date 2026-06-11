import type { HTMLAttributes } from "react";
import { cn } from "@/lib/utils/utils";

type SkeletonProps = HTMLAttributes<HTMLDivElement>;

export function Skeleton({ className, ...props }: SkeletonProps) {
  return <div className={cn("fabu-skeleton rounded-card", className)} {...props} />;
}

export function ChartSkeleton() {
  return (
    <div className="grid h-full min-h-[280px] gap-3 rounded-card border border-fabu-border bg-white p-5 shadow-elevated">
      <Skeleton className="h-6 w-40" />
      <Skeleton className="h-[220px] w-full" />
    </div>
  );
}
