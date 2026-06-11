"use client";

import Image from "next/image";
import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";
import { memo, useCallback, useMemo, useState } from "react";
import { Button } from "@/components/ui/button";
import { LoginForm } from "@/features/auth/LoginForm";
import { RegisterForm } from "@/features/auth/RegisterForm";
import { featureRoutes } from "@/features/value-added/feature-routes";
import { useAuth } from "@/hooks/use-auth";
import logo from "@/styles/images/FABUlogo.png";

type NavItem = {
  href: string;
  label: string;
  match?: string;
};

const navItems: NavItem[] = [
  { href: "/", label: "Home" },
  { href: "/P5GDataPlan", label: "5G Data" },
  { href: "/shop", label: "Shop" },
  { href: featureRoutes[0].href, label: "Features", match: "/features" },
  { href: "/recharge", label: "Recharge" },
  { href: "/about", label: "About" },
  { href: "/contact", label: "Contact" },
];

function Header() {
  const pathname = usePathname();
  const router = useRouter();
  const { isAuthenticated, profile, logout } = useAuth();
  const [isMenuOpen, setMenuOpen] = useState(false);
  const [authMode, setAuthMode] = useState<"login" | "register" | null>(null);
  const [isAccountOpen, setAccountOpen] = useState(false);

  const username = useMemo(
    () => profile.username || profile.email || "Account",
    [profile.email, profile.username]
  );

  const closeAuth = useCallback(() => setAuthMode(null), []);

  const handleLogout = useCallback(async () => {
    await logout();
    setAccountOpen(false);
    router.push("/");
  }, [logout, router]);

  return (
    <>
      <header className="fixed left-0 top-0 z-40 w-full border-b border-fabu-border bg-white">
        <div className="mx-auto flex h-20 max-w-[1400px] items-center justify-between gap-4 px-4 md:px-5 lg:px-8">
          <button
            type="button"
            className="flex h-11 w-11 items-center justify-center rounded-full bg-fabu-muted text-xl text-fabu-charcoal hover:bg-[#E7E7E7] lg:hidden"
            onClick={() => setMenuOpen((value) => !value)}
            aria-label="Toggle navigation"
          >
            =
          </button>

          <Link href="/" className="flex min-h-11 items-center gap-3" aria-label="Fabu home">
            <Image src={logo} alt="Fabu" className="h-12 w-auto" priority />
          </Link>

          <nav className="hidden h-full items-center gap-1 lg:flex">
            {navItems.map((item) => {
              const isActive =
                item.href === "/"
                  ? pathname === item.href
                  : pathname.startsWith(item.match ?? item.href);
              return (
                <Link
                  key={item.href}
                  href={item.href}
                  className={`flex min-h-11 items-center rounded px-4 text-sm transition ${
                    isActive
                      ? "bg-fabu-red text-white"
                      : "text-fabu-charcoal hover:bg-fabu-muted hover:text-fabu-red"
                  }`}
                >
                  {item.label}
                </Link>
              );
            })}
          </nav>

          <div className="relative flex items-center gap-2">
            {!isAuthenticated ? (
              <>
                <Button variant="ghost" onClick={() => setAuthMode("login")}>
                  Sign in
                </Button>
                <Button className="hidden sm:inline-flex" onClick={() => setAuthMode("register")}>
                  Register
                </Button>
              </>
            ) : (
              <>
                <Button variant="secondary" onClick={() => setAccountOpen((value) => !value)}>
                  {username}
                </Button>
                {isAccountOpen ? (
                  <div className="absolute right-0 top-14 w-56 rounded border border-fabu-border bg-white p-2 shadow-modal">
                    <button
                      type="button"
                      className="flex min-h-11 w-full items-center rounded px-3 text-left text-sm hover:bg-fabu-muted"
                      onClick={() => {
                        setAccountOpen(false);
                        router.push("/Profile");
                      }}
                    >
                      Profile
                    </button>
                    <button
                      type="button"
                      className="flex min-h-11 w-full items-center rounded px-3 text-left text-sm hover:bg-fabu-muted"
                      onClick={handleLogout}
                    >
                      Log out
                    </button>
                  </div>
                ) : null}
              </>
            )}
          </div>
        </div>

        {isMenuOpen ? (
          <nav className="border-t border-fabu-border bg-white px-4 py-3 lg:hidden">
            <div className="grid gap-2">
              {navItems.map((item) => (
                <Link
                  key={item.href}
                  href={item.href}
                  className="flex min-h-11 items-center rounded px-3 text-sm hover:bg-fabu-muted hover:text-fabu-red"
                  onClick={() => setMenuOpen(false)}
                >
                  {item.label}
                </Link>
              ))}
            </div>
          </nav>
        ) : null}
      </header>

      {authMode ? (
        <div
          className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 px-4 py-8"
          onMouseDown={closeAuth}
        >
          <div onMouseDown={(event) => event.stopPropagation()} className="w-full max-w-lg">
            {authMode === "login" ? (
              <LoginForm
                onClose={closeAuth}
                onSwitchToSignup={() => setAuthMode("register")}
              />
            ) : (
              <RegisterForm
                onClose={closeAuth}
                onSwitchToLogin={() => setAuthMode("login")}
              />
            )}
          </div>
        </div>
      ) : null}
    </>
  );
}

export default memo(Header);
