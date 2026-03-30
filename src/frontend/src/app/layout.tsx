import Header from "../components/layout/Header";
import Footer from "../components/layout/Footer";
import Chatbot from "../components/layout/Chatbot";
import "../styles/globals.css"
import "../styles/themes/dark.css"
import "../styles/themes/light.css"

export default function RootLayout({
    children,
}: {
    children: React.ReactNode
}) {
    return (
        <html lang="en">
            <body>
                <Header />
                <Chatbot />
                <main>{children}</main>
                <Footer />
            </body>
        </html>
    )
}
