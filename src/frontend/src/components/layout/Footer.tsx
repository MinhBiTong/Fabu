import Image from "next/image";
import Link from "next/link";
import { featureRoutes } from "@/features/value-added/feature-routes";
import logo from "@/styles/images/FABUlogo.png";
import facebook from "@/styles/images/facebook.png";
import youtube from "@/styles/images/youtube.png";

type FooterItem = {
  label: string;
  href?: string;
};

type FooterColumn = {
  title: string;
  items: FooterItem[];
};

const columns = [
  {
    title: "Products",
    items: [
      { label: "Mobile recharge", href: "/recharge" },
      { label: "5G data plans", href: "/P5GDataPlan" },
      { label: "Postpaid bill", href: "/billpayment" },
      { label: featureRoutes[0].title, href: featureRoutes[0].href },
    ],
  },
  {
    title: "Support",
    items: [
      { label: "Contact center", href: "/contact" },
      { label: "Feedback", href: "/contact" },
      { label: "Service status" },
      { label: "Account security", href: "/Profile" },
    ],
  },
  {
    title: "Features",
    items: featureRoutes.slice(1).map((route) => ({
      label: route.title,
      href: route.href,
    })),
  },
  {
    title: "Legal",
    items: [
      { label: "Terms" },
      { label: "Privacy" },
      { label: "Payment policy" },
      { label: "Accessibility" },
    ],
  },
] satisfies FooterColumn[];

export default function Footer() {
  return (
    <footer className="border-t border-fabu-border bg-fabu-muted">
      <div className="mx-auto max-w-[1400px] px-4 py-10 md:px-5 lg:px-8">
        <div className="grid gap-8 lg:grid-cols-[1.3fr_2fr]">
          <div>
            <Image src={logo} alt="Fabu" className="h-14 w-auto" />
            <p className="mt-4 max-w-xl text-sm leading-6 text-fabu-gray">
              Fabu provides mobile recharge, data packages, billing, and customer support
              services with a secure REST backend and modern digital experience.
            </p>
          </div>

          <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-4">
            {columns.map((column) => (
              <div key={column.title}>
                <h3 className="text-base">{column.title}</h3>
                <div className="mt-3 grid gap-2 text-sm text-fabu-gray">
                  {column.items.map((item) => (
                    item.href ? (
                      <Link key={item.label} href={item.href} className="hover:text-fabu-red">
                        {item.label}
                      </Link>
                    ) : (
                      <span key={item.label}>{item.label}</span>
                    )
                  ))}
                </div>
              </div>
            ))}
          </div>
        </div>

        <div className="mt-8 flex flex-col gap-4 border-t border-fabu-border pt-6 text-sm text-fabu-gray sm:flex-row sm:items-center sm:justify-between">
          <p>Copyright 2026 Fabu. All rights reserved.</p>
          <div className="flex items-center gap-3">
            <Link href="#" className="flex h-11 w-11 items-center justify-center rounded-full bg-white">
              <Image src={facebook} alt="Facebook" className="h-5 w-5" />
            </Link>
            <Link href="#" className="flex h-11 w-11 items-center justify-center rounded-full bg-white">
              <Image src={youtube} alt="Youtube" className="h-5 w-5" />
            </Link>
          </div>
        </div>
      </div>
    </footer>
  );
}
