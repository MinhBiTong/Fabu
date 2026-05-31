import Link from "next/link";
import { Button } from "@/components/ui/button";

const options = [
  "Prepaid recharge",
  "Postpaid payment",
  "Mobile data",
  "Customer support",
];

export default function PhoneServicePage() {
  return (
    <section className="fabu-section">
      <div className="fabu-container">
        <div className="rounded-[0_0_50%_50%] bg-fabu-red px-6 py-16 text-center text-white">
          <h1 className="text-white">Mobile Services</h1>
          <p className="mx-auto mt-3 max-w-2xl text-sm leading-6 text-white/90">
            Manage core phone services through the Fabu REST-powered digital experience.
          </p>
          <div className="mt-8 flex flex-wrap justify-center gap-3">
            {options.map((option) => (
              <span
                key={option}
                className="rounded-full bg-white/15 px-4 py-2 text-sm font-semibold text-white"
              >
                {option}
              </span>
            ))}
          </div>
          <Button asChild className="mt-8 border-white bg-white text-fabu-red hover:bg-fabu-muted">
            <Link href="/recharge">Recharge phone</Link>
          </Button>
        </div>
      </div>
    </section>
  );
}
