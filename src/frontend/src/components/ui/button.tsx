import * as React from "react";
import { Slot } from "@radix-ui/react-slot";
import { cva, type VariantProps } from "class-variance-authority";
import { cn } from "@/lib/utils/utils";

const buttonVariants = cva(
  "inline-flex min-h-11 items-center justify-center gap-2 whitespace-nowrap text-sm font-semibold ring-offset-background transition focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:pointer-events-none disabled:border-fabu-border disabled:bg-fabu-border disabled:text-fabu-gray disabled:opacity-80 [&_svg]:pointer-events-none [&_svg]:size-4 [&_svg]:shrink-0",
  {
    variants: {
      variant: {
        default:
          "rounded-fabu border border-fabu-red bg-fabu-red px-5 text-white hover:border-fabu-red-hover hover:bg-fabu-red-hover active:border-[#B20F2A] active:bg-[#B20F2A]",
        destructive:
          "rounded-fabu border border-fabu-red bg-fabu-red px-5 text-white hover:border-fabu-red-hover hover:bg-fabu-red-hover",
        outline:
          "rounded-fabu border border-fabu-border bg-white px-5 text-fabu-charcoal hover:border-fabu-red hover:text-fabu-red",
        secondary:
          "rounded-full border-0 bg-fabu-muted px-5 text-fabu-charcoal hover:bg-[#E7E7E7]",
        ghost:
          "rounded-full px-4 text-fabu-charcoal hover:bg-[rgba(238,0,51,0.08)] hover:text-fabu-red",
        link: "min-h-0 rounded-none px-0 text-fabu-charcoal underline-offset-4 hover:text-fabu-red hover:underline",
      },
      size: {
        default: "h-11 py-2",
        sm: "h-11 px-4 text-sm",
        lg: "h-12 px-8 text-base",
        icon: "h-11 w-11 rounded-full p-0",
      },
    },
    defaultVariants: {
      variant: "default",
      size: "default",
    },
  }
);

export interface ButtonProps
  extends React.ButtonHTMLAttributes<HTMLButtonElement>,
    VariantProps<typeof buttonVariants> {
  asChild?: boolean;
}

const Button = React.forwardRef<HTMLButtonElement, ButtonProps>(
  ({ className, variant, size, asChild = false, ...props }, ref) => {
    const Comp = asChild ? Slot : "button";
    return (
      <Comp
        className={cn(buttonVariants({ variant, size, className }))}
        ref={ref}
        {...props}
      />
    );
  }
);
Button.displayName = "Button";

export { Button, buttonVariants };
