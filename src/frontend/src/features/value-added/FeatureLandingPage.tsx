import Link from "next/link";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { cn } from "@/lib/utils/utils";
import { featureRoutes } from "./feature-routes";
import type { FeaturePageConfig } from "./feature-pages";

type FeatureLandingPageProps = {
  config: FeaturePageConfig;
};

export function FeatureLandingPage({ config }: FeatureLandingPageProps) {
  const heroClassName =
    config.tone === "ink" ? "bg-fabu-ink text-white" : "bg-fabu-red text-white";

  return (
    <div className="fabu-page">
      <section className={cn("relative overflow-hidden", heroClassName)}>
        <div className="fabu-container grid gap-8 px-4 py-16 md:px-5 lg:grid-cols-[1.08fr_0.92fr] lg:px-8 lg:py-[96px]">
          <div className="max-w-3xl">
            <p className="text-sm font-bold uppercase tracking-normal text-white/80">
              {config.eyebrow}
            </p>
            <h1 className="mt-3 text-white">{config.route.title}</h1>
            <p className="mt-4 max-w-2xl text-lg font-semibold leading-8 text-white/90">
              {config.lead}
            </p>
            <div className="mt-8 flex flex-col gap-3 sm:flex-row">
              <Button asChild variant="secondary">
                <Link href={config.primaryAction.href}>{config.primaryAction.label}</Link>
              </Button>
              <Button asChild className="border-white bg-white text-fabu-red hover:bg-fabu-muted">
                <Link href={config.secondaryAction.href}>{config.secondaryAction.label}</Link>
              </Button>
            </div>
          </div>

          <div className="grid gap-3 sm:grid-cols-3 lg:grid-cols-1">
            {config.stats.map((stat) => (
              <div
                key={`${stat.value}-${stat.label}`}
                className="rounded-card bg-white/12 p-5 text-white ring-1 ring-white/20"
              >
                <p className="text-3xl font-bold leading-10 text-white">{stat.value}</p>
                <p className="mt-1 text-sm leading-5 text-white/80">{stat.label}</p>
              </div>
            ))}
          </div>
        </div>
        <div className="h-8 rounded-t-[50%] bg-white" />
      </section>

      <section className="fabu-section">
        <div className="fabu-container grid gap-8 lg:grid-cols-[0.86fr_1.14fr]">
          <div>
            <h2>Built For The Current Fabu System</h2>
            <p className="mt-3 text-sm leading-6 text-fabu-gray">
              This screen keeps the established layout rhythm, card language, CTA behavior, and
              backend-aligned routing already used by recharge, service plans, account, and
              support pages.
            </p>
            <div className="mt-6 grid gap-3">
              {config.backend.map((item) => (
                <div key={item.title} className="rounded-card border border-fabu-border bg-white p-4">
                  <p className="text-sm font-bold text-fabu-ink">{item.title}</p>
                  <p className="mt-1 text-sm leading-6 text-fabu-gray">{item.description}</p>
                </div>
              ))}
            </div>
          </div>

          <div className="grid gap-5 md:grid-cols-3">
            {config.benefits.map((benefit) => (
              <Card key={benefit.title}>
                <CardHeader>
                  <CardTitle className="text-xl leading-7">{benefit.title}</CardTitle>
                  <CardDescription>{benefit.description}</CardDescription>
                </CardHeader>
              </Card>
            ))}
          </div>
        </div>
      </section>

      <section className="fabu-section bg-fabu-muted">
        <div className="fabu-container">
          <div className="mb-8 flex flex-col gap-2 md:flex-row md:items-end md:justify-between">
            <div>
              <h2>Available Options</h2>
              <p className="mt-2 text-sm text-fabu-gray">
                Compact option cards follow the same spacing, border, radius, and shadow system.
              </p>
            </div>
            <Button asChild variant="outline">
              <Link href="/contact">Talk to Fabu</Link>
            </Button>
          </div>

          <div className="grid gap-5 md:grid-cols-3">
            {config.packages.map((item) => (
              <Card key={item.title}>
                <CardHeader>
                  <div className="mb-2 flex items-center justify-between gap-3">
                    <span className="rounded-full bg-fabu-info px-3 py-1 text-xs font-bold text-fabu-link">
                      {item.badge}
                    </span>
                    <span className="text-sm font-semibold text-fabu-gray">{item.price}</span>
                  </div>
                  <CardTitle className="text-xl leading-7">{item.title}</CardTitle>
                </CardHeader>
                <CardContent>
                  <p className="text-sm leading-6 text-fabu-gray">{item.description}</p>
                </CardContent>
              </Card>
            ))}
          </div>
        </div>
      </section>

      <section className="fabu-section">
        <div className="fabu-container grid gap-8 lg:grid-cols-[0.78fr_1.22fr]">
          <div>
            <h2>Customer Flow</h2>
            <p className="mt-3 text-sm leading-6 text-fabu-gray">
              The page avoids unsupported branches by routing customers into existing REST-backed
              workflows.
            </p>
          </div>
          <div className="grid gap-4 md:grid-cols-3">
            {config.workflow.map((step, index) => (
              <div key={step.title} className="fabu-card">
                <span className="rounded-full bg-fabu-muted px-3 py-1 text-xs font-bold text-fabu-red">
                  {String(index + 1).padStart(2, "0")}
                </span>
                <h3 className="mt-5 text-xl">{step.title}</h3>
                <p className="mt-2 text-sm leading-6 text-fabu-gray">{step.description}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      <section className="fabu-section bg-fabu-muted">
        <div className="fabu-container">
          <div className="mb-8">
            <h2>More Fabu Features</h2>
            <p className="mt-2 text-sm text-fabu-gray">
              Related feature pages use the same shared component and data contract.
            </p>
          </div>
          <div className="grid gap-5 sm:grid-cols-2 lg:grid-cols-4">
            {featureRoutes.map((route) => {
              const isCurrent = route.href === config.route.href;

              return (
                <Link
                  key={route.href}
                  href={route.href}
                  className={cn(
                    "fabu-card min-h-44",
                    isCurrent ? "border-fabu-red bg-white" : "bg-white"
                  )}
                >
                  <span className="rounded-full bg-fabu-info px-3 py-1 text-xs font-bold text-fabu-link">
                    {route.accent}
                  </span>
                  <h3 className="mt-5 text-xl">{route.title}</h3>
                  <p className="mt-2 text-sm leading-6 text-fabu-gray">{route.description}</p>
                </Link>
              );
            })}
          </div>
        </div>
      </section>
    </div>
  );
}
