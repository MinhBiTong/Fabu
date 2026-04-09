"use client";
import Header from "../components/layout/Header";
import Footer from "../components/layout/Footer";
import Chatbot from "../components/layout/Chatbot";
import AdminSidebar from "../components/layout/AdminSidebar";

import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

import "../styles/globals.css";
import "../styles/adminglobal.css";
import "../styles/themes/dark.css";
import "../styles/themes/light.css";
import { AuthProvider } from "@/context/auth/AuthProvider";

export default function RootLayout({
    children,
}: {
    children: React.ReactNode
}) {
    return (
        <html lang="en">
            <body>
                <AuthProvider>
                    <Header />
                    <Chatbot />
                    <AdminSidebar />
                    <main>{children}</main>
                    <ToastContainer />
                    <Footer />
                </AuthProvider>
            </body>
        </html>
    )
}
