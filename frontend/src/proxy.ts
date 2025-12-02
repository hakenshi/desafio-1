import { NextRequest, NextResponse } from "next/server";

// Decode JWT to check expiration (edge runtime compatible)
function isTokenExpired(token: string): boolean {
    try {
        const parts = token.split(".");
        if (parts.length !== 3) return true;

        // Base64 decode (edge runtime compatible)
        const payload = JSON.parse(atob(parts[1]));
        if (!payload.exp) return true;

        // Consider expired if less than 30 seconds remaining
        const expiresAt = payload.exp * 1000;
        const now = Date.now();
        return expiresAt - now < 30000;
    } catch {
        return true;
    }
}

export function proxy(request: NextRequest) {
    const { pathname } = request.nextUrl;
    const authToken = request.cookies.get("authToken")?.value;
    const refreshToken = request.cookies.get("refreshToken")?.value;

    // Redirect root to dashboard
    if (pathname === "/") {
        return NextResponse.redirect(new URL("/dashboard", request.url));
    }

    // No tokens at all - redirect to login
    if (!authToken && !refreshToken) {
        return NextResponse.redirect(new URL("/login", request.url));
    }

    // Auth token valid - allow request
    if (authToken && !isTokenExpired(authToken)) {
        return NextResponse.next();
    }

    // Auth token expired but refresh token exists
    // Let the request through - the server action will handle refresh
    if (refreshToken) {
        return NextResponse.next();
    }

    // No valid tokens - redirect to login
    return NextResponse.redirect(new URL("/login", request.url));
}

export const config = {
    matcher: [
        "/",
        "/dashboard/:path*",
        "/categories/:path*",
        "/products/:path*",
        "/users/:path*",
        "/settings/:path*",
    ],
};