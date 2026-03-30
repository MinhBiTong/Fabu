import { NextResponse } from "next/server";
import type { NextRequest } from "next/server";
import { jwtVerify } from "jose";

// 🔐 ENV
const JWT_SECRET = new TextEncoder().encode(process.env.JWT_SECRET || "secret");

// 🔹 Route config
const PUBLIC_ROUTES = ["/login", "/register"];
const AUTH_ROUTES = ["/dashboard", "/recharge", "/bill-payment"];
const ADMIN_ROUTES = ["/admin"];

// 🔹 Fake Redis 
async function isBlacklisted(token: string) {
  try {
    const res = await fetch(`${process.env.API_URL}/auth/check-blacklist`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ token }),
    });

    const data = await res.json();
    return data.blacklisted;
  } catch (err) {
    return false; // fail-open 
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

  const token = request.cookies.get("token")?.value;

  // ========================
  // ❌ No token → login
  // ========================
  if (!token) {
    return NextResponse.redirect(new URL("/login", request.url));
  }

  // ========================
  // 🔐 Verify JWT
  // ========================
  let payload: any = null;

  try {
    const { payload: decoded } = await jwtVerify(token, JWT_SECRET);
    payload = decoded;
  } catch (err) {
    return NextResponse.redirect(new URL("/login", request.url));
  }

  // ========================
  // 🚫 Check blacklist (Redis)
  // ========================
  const blacklisted = await isBlacklisted(token);

  if (blacklisted) {
    return NextResponse.redirect(new URL("/login", request.url));
  }

  // ========================
  // 🧑‍💼 Role check
  // ========================
  const role = payload?.role;

  if (ADMIN_ROUTES.some((route) => pathname.startsWith(route))) {
    if (role !== "admin") {
      return NextResponse.redirect(new URL("/403", request.url));
    }
  }

  if (AUTH_ROUTES.some((route) => pathname.startsWith(route))) {
    if (!role) {
      return NextResponse.redirect(new URL("/login", request.url));
    }
  }

  // ========================
  // ✅ OK -> skip
  // ========================
  return NextResponse.next();
}

export const config = {
  matcher: [
    "/dashboard/:path*",
    "/admin/:path*",
    "/recharge/:path*",
    "/bill-payment/:path*",
  ],
};