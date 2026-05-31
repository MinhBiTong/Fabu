import * as React from "react";
import { cn } from "@/lib/utils/utils";

const Input = React.forwardRef<HTMLInputElement, React.ComponentProps<"input">>(
  ({ className, type, ...props }, ref) => {
    return (
      <input
        type={type}
        className={cn(
          "flex h-11 w-full rounded border border-fabu-border bg-white px-4 py-3 text-sm text-fabu-charcoal ring-offset-background transition file:border-0 file:bg-transparent file:text-sm file:font-medium file:text-foreground placeholder:text-neutral-400 focus-visible:border-fabu-red focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[rgba(238,0,51,0.1)] disabled:cursor-not-allowed disabled:bg-fabu-muted disabled:text-fabu-gray",
          className
        )}
        ref={ref}
        {...props}
      />
    );
  }
);
Input.displayName = "Input";

export { Input };
