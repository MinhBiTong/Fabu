import Link from "next/link";
import { Button } from "@/components/ui/button";

export default function NotFoundPage() {
  return (
    <section className="fabu-section">
      <div className="fabu-container flex min-h-[calc(100vh-220px)] items-center justify-center text-center">
        <div>
          <h1>Page Not Found</h1>
          <p className="mt-3 text-sm text-fabu-gray">The page you requested does not exist.</p>
          <Button asChild className="mt-6">
            <Link href="/">Back home</Link>
          </Button>
        </div>
      </div>
    </section>
  );
}
