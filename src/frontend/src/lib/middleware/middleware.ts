import { NextResponse } from "next/server";
import type { JWTPayload } from "jose";
import type { NextRequest } from "next/server";
import { jwtVerify } from "jose";

const JWT_SECRET = new TextEncoder().encode(process.env.JWT_SECRET || "secret");

const PUBLIC_ROUTES = ["/login", "/register"];
const AUTH_ROUTES = ["/dashboard", "/recharge", "/bill-payment"];
const ADMIN_ROUTES = ["/admin"];

type RolePayload = JWTPayload & {
  role?: string;
};

async function isBlacklisted(token: string) {
  try {
    const response = await fetch(`${process.env.API_URL}/auth/check-blacklist`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ token }),
    });

    const data = (await response.json()) as { blacklisted?: boolean };
    return Boolean(data.blacklisted);
  } catch {
    return false;
  }
}

export async function middleware(request: NextRequest) {
  const { pathname } = request.nextUrl;

  if (
    pathname.startsWith("/_next") ||
    pathname.startsWith("/favicon.ico") ||
    pathname.startsWith("/api")
  ) {
    return NextResponse.next();
  }

  if (PUBLIC_ROUTES.some((route) => pathname.startsWith(route))) {
    return NextResponse.next();
  }

  const token = request.cookies.get("fabu_at")?.value;
  if (!token) {
    return NextResponse.redirect(new URL("/login", request.url));
  }

  let payload: RolePayload;
  try {
    const verified = await jwtVerify(token, JWT_SECRET);
    payload = verified.payload as RolePayload;
  } catch {
    return NextResponse.redirect(new URL("/login", request.url));
  }

  const blacklisted = await isBlacklisted(token);
  if (blacklisted) {
    return NextResponse.redirect(new URL("/login", request.url));
  }

  const role = payload.role;
  if (ADMIN_ROUTES.some((route) => pathname.startsWith(route)) && role !== "Admin") {
    return NextResponse.redirect(new URL("/403", request.url));
  }

  if (AUTH_ROUTES.some((route) => pathname.startsWith(route)) && !role) {
    return NextResponse.redirect(new URL("/login", request.url));
  }

  return NextResponse.next();
}

export const config = {
  matcher: ["/dashboard/:path*", "/admin/:path*", "/recharge/:path*", "/bill-payment/:path*"],
};
