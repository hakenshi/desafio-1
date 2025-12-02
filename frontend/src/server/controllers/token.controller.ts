"use server";

import { redirect } from "next/navigation";
import { TokenService } from "../services/token.service";

const tokenService = new TokenService();

/**
 * Gets a valid auth token, refreshing if necessary
 * Redirects to login if unable to get a valid token
 */
export async function getValidAuthToken(): Promise<string> {
  const token = await tokenService.getValidToken();

  if (!token) {
    redirect("/login");
  }

  return token;
}

/**
 * Gets a valid auth token without redirecting
 * Returns null if unable to get a valid token
 */
export async function getValidAuthTokenOrNull(): Promise<string | null> {
  return await tokenService.getValidToken();
}

/**
 * Checks if user is authenticated
 */
export async function isAuthenticated(): Promise<boolean> {
  return await tokenService.isAuthenticated();
}

/**
 * Clears auth cookies (used during logout)
 */
export async function clearAuth(): Promise<void> {
  await tokenService.clearAuthCookies();
}

/**
 * Refreshes the auth token manually
 * Returns true if refresh was successful
 */
export async function refreshAuth(): Promise<boolean> {
  const refreshToken = await tokenService.getRefreshToken();

  if (!refreshToken) {
    return false;
  }

  const newTokens = await tokenService.refreshAccessToken(refreshToken);

  if (newTokens) {
    await tokenService.updateAuthCookies(newTokens);
    return true;
  }

  return false;
}
