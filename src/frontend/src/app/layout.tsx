import type { ReactNode } from "react";
import Header from "@/components/layout/Header";
import Footer from "@/components/layout/Footer";
import Chatbot from "@/components/layout/Chatbot";
import AdminSidebar from "@/components/layout/AdminSidebar";
import LayoutAdBroadcast from "@/components/layout/LayoutAdBroadcast";
import { Providers } from "./providers";

import "react-toastify/dist/ReactToastify.css";
import "./globals.css";

export const metadata = {
  title: "Fabu",
  description: "Fabu digital telecom services",
};

export default function RootLayout({ children }: { children: ReactNode }) {
  return (
    <html lang="vi">
      <body>
        <Providers>
          <Header />
          <AdminSidebar />
          <main className="min-h-screen pt-20">
            <LayoutAdBroadcast />
            {children}
          </main>
          <Chatbot />
          <Footer />
        </Providers>
      </body>
    </html>
  );
}
