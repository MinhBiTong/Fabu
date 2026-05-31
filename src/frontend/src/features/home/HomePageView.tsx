import Link from "next/link";
import { Button } from "@/components/ui/button";
import { featureRoutes } from "@/features/value-added/feature-routes";

const services = [
  { title: "Mobile Recharge", href: "/recharge", accent: "Recharge" },
  { title: "Bill Payment", href: "/billpayment", accent: "Bills" },
  { title: "5G Data Package", href: "/P5GDataPlan", accent: "Data" },
  { title: "Transaction History", href: "/Profile", accent: "History" },
  { title: "Account", href: "/Profile", accent: "Profile" },
];

const promotions = [
  {
    title: "Bonus Recharge",
    body: "Get extra balance when recharging with selected payment methods.",
  },
  {
    title: "5G Data Discount",
    body: "Compare high-speed service plans and choose the right validity window.",
  },
  {
    title: "Loyalty Points",
    body: "Track customer rewards and transaction activity in one place.",
  },
];

const categories = [
  {
    title: "Mobile",
    description: "Recharge, number, and account journeys for everyday customers.",
    href: "/recharge",
  },
  {
    title: "Internet",
    description: "5G package discovery backed by the live service catalog.",
    href: "/P5GDataPlan",
  },
  {
    title: "Devices",
    description: "Digital services, SIM discovery, and add-on support entry points.",
    href: featureRoutes[0].href,
  },
  {
    title: "Enterprise",
    description: "Business connectivity and support flows for teams.",
    href: "/features/enterprise",
  },
];

export function HomePageView() {
  return (
    <div className="fabu-page">
      <section className="relative overflow-hidden bg-fabu-red text-white">
        <div className="fabu-container grid gap-8 px-4 py-16 md:px-5 lg:grid-cols-[1.1fr_0.9fr] lg:px-8 lg:py-[100px]">
          <div className="max-w-3xl">
            <h1 className="text-white">Fabu</h1>
            <p className="mt-4 max-w-2xl text-lg font-semibold leading-8 text-white/90">
              A modern telecom workspace for recharge, data packages, payments, and customer
              care built around the Fabu backend.
            </p>
            <div className="mt-8 flex flex-col gap-3 sm:flex-row">
              <Button asChild variant="secondary">
                <Link href="/recharge">Recharge now</Link>
              </Button>
              <Button asChild className="border-white bg-white text-fabu-red hover:bg-fabu-muted">
                <Link href="/P5GDataPlan">Browse 5G plans</Link>
              </Button>
            </div>
          </div>

          <div className="grid gap-3 sm:grid-cols-2">
            {services.slice(0, 4).map((item) => (
              <Link
                key={item.title}
                href={item.href}
                className="rounded-card bg-white/12 p-5 text-white ring-1 ring-white/20 transition hover:bg-white/18"
              >
                <span className="text-xs font-bold uppercase">{item.accent}</span>
                <h3 className="mt-4 text-xl text-white">{item.title}</h3>
              </Link>
            ))}
          </div>
        </div>
        <div className="h-8 rounded-t-[50%] bg-white" />
      </section>

      <section className="fabu-section">
        <div className="fabu-container">
          <div className="grid gap-5 sm:grid-cols-2 lg:grid-cols-5">
            {services.map((item) => (
              <Link key={item.title} href={item.href} className="fabu-card min-h-36">
                <span className="rounded-full bg-fabu-info px-3 py-1 text-xs font-bold text-fabu-link">
                  {item.accent}
                </span>
                <h3 className="mt-5 text-xl">{item.title}</h3>
              </Link>
            ))}
          </div>
        </div>
      </section>

      <section className="fabu-section bg-fabu-muted">
        <div className="fabu-container">
          <div className="mb-8 flex flex-col gap-2 md:flex-row md:items-end md:justify-between">
            <div>
              <h2>Featured Promotions</h2>
              <p className="mt-2 text-sm text-fabu-gray">
                Clear, scan-friendly offers aligned with current Fabu services.
              </p>
            </div>
            <Button asChild variant="outline">
              <Link href="/features/promotions">View offers</Link>
            </Button>
          </div>

          <div className="grid gap-5 md:grid-cols-3">
            {promotions.map((item) => (
              <article key={item.title} className="fabu-card">
                <h3 className="text-xl">{item.title}</h3>
                <p className="mt-3 text-sm leading-6 text-fabu-gray">{item.body}</p>
              </article>
            ))}
          </div>
        </div>
      </section>

      <section className="fabu-section">
        <div className="fabu-container">
          <div className="mb-8 flex flex-col gap-2 md:flex-row md:items-end md:justify-between">
            <div>
              <h2>Fabu Feature Suite</h2>
              <p className="mt-2 text-sm text-fabu-gray">
                Four new customer entry points built with the same Fabu layout and component system.
              </p>
            </div>
            <Button asChild variant="outline">
              <Link href={featureRoutes[0].href}>Open features</Link>
            </Button>
          </div>
          <div className="grid gap-5 sm:grid-cols-2 lg:grid-cols-4">
            {featureRoutes.map((feature) => (
              <Link key={feature.href} href={feature.href} className="fabu-card min-h-44">
                <span className="rounded-full bg-fabu-info px-3 py-1 text-xs font-bold text-fabu-link">
                  {feature.accent}
                </span>
                <h3 className="mt-5 text-xl">{feature.title}</h3>
                <p className="mt-2 text-sm leading-6 text-fabu-gray">{feature.description}</p>
              </Link>
            ))}
          </div>
        </div>
      </section>

      <section className="fabu-section bg-fabu-muted">
        <div className="fabu-container grid gap-6 lg:grid-cols-[0.8fr_1.2fr]">
          <div>
            <h2>Diverse Products & Services</h2>
            <p className="mt-3 text-sm leading-6 text-fabu-gray">
              The interface stays focused on real workflows: find a plan, recharge a phone,
              send feedback, and manage operations.
            </p>
          </div>
          <div className="grid gap-4 sm:grid-cols-2">
            {categories.map((category) => (
              <Link key={category.title} href={category.href} className="fabu-card">
                <h3 className="text-xl">{category.title}</h3>
                <p className="mt-2 text-sm text-fabu-gray">
                  {category.description}
                </p>
              </Link>
            ))}
          </div>
        </div>
      </section>
    </div>
  );
}
