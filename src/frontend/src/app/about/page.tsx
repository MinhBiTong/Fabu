import Image from "next/image";
import FabuAbout from "@/styles/images/FabuAbout.png";
import Blender from "@/styles/images/newblenderlogo.png";
import Mobifone from "@/styles/images/mobifone.png";
import Viettel from "@/styles/images/viettel.png";
import MBBank from "@/styles/images/mbbanklogo.png";

const investors = [
  { name: "Blender", image: Blender },
  { name: "Mobifone", image: Mobifone },
  { name: "Viettel", image: Viettel },
  { name: "MB Bank", image: MBBank },
];

export default function AboutPage() {
  return (
    <section className="fabu-section">
      <div className="fabu-container grid gap-12">
        <div className="grid gap-8 lg:grid-cols-[0.9fr_1.1fr] lg:items-center">
          <Image
            src={FabuAbout}
            alt="Fabu service illustration"
            className="w-full rounded-[15px] object-cover"
            priority
          />
          <div>
            <h1>About Fabu</h1>
            <p className="mt-4 text-sm leading-7 text-fabu-gray">
              Fabu provides a modern platform that makes 5G recharge fast, simple,
              and reliable. The frontend is now aligned with typed REST services so
              users can purchase data plans, manage usage, and stay connected with
              fewer UI surprises.
            </p>
          </div>
        </div>

        <div>
          <h2>Trusted Partners</h2>
          <div className="mt-6 grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
            {investors.map((investor) => (
              <div key={investor.name} className="fabu-card flex h-36 items-center justify-center">
                <Image
                  src={investor.image}
                  alt={investor.name}
                  className="max-h-20 w-auto object-contain"
                />
              </div>
            ))}
          </div>
        </div>
      </div>
    </section>
  );
}
