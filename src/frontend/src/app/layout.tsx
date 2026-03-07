import Header from "../components/layout/Header";
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
                <main>{children}</main>
                {/* footer */}
            </body>
        </html>
    )
}
