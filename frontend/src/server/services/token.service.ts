import { cookies } from "next/headers";
import { apiClient } from "./api-client.service";

interface TokenResponse {
  accessToken: string;
  refreshToken?: string;
  expiresIn: number;
  tokenType: string;
}

export class TokenService {
  /**
   * Decodes a JWT token and returns the payload
   */
  decodeToken(token: string): { exp?: number; sub?: string } | null {
    try {
      const parts = token.split(".");
      if (parts.length !== 3) return null;
      const payload = JSON.parse(Buffer.from(parts[1], "base64").toString());
      return payload;
    } catch {
      return null;
    }
  }

  /**
   * Checks if a token is expired or about to expire (within 30 seconds)
   */
  isTokenExpired(token: string): boolean {
    const payload = this.decodeToken(token);
    if (!payload?.exp) return true;

    // Consider expired if less than 30 seconds remaining
    const expiresAt = payload.exp * 1000;
    const now = Date.now();
    return expiresAt - now < 30000;
  }

  /**
   * Refreshes the access token using the refresh token
   */
  async refreshAccessToken(refreshToken: string): Promise<TokenResponse | null> {
    try {
      const response = await apiClient.post<TokenResponse>("/auth/refresh", {
        refresh_token: refreshToken,
      });
      return response;
    } catch {
      return null;
    }
  }

  /**
   * Updates the auth cookies with new tokens
   */
  async updateAuthCookies(tokenResponse: TokenResponse): Promise<void> {
    const cookieStore = await cookies();
    const expiresAt = new Date(Date.now() + tokenResponse.expiresIn * 1000);

    cookieStore.set({
      name: "authToken",
      value: tokenResponse.accessToken,
      httpOnly: true,
      secure: process.env.NODE_ENV === "production",
      expires: expiresAt,
    });

    if (tokenResponse.refreshToken) {
      cookieStore.set({
        name: "refreshToken",
        value: tokenResponse.refreshToken,
        httpOnly: true,
        secure: process.env.NODE_ENV === "production",
        expires: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000),
      });
    }
  }

  /**
   * Gets the current auth token from cookies
   */
  async getAuthToken(): Promise<string | undefined> {
    const cookieStore = await cookies();
    return cookieStore.get("authToken")?.value;
  }

  /**
   * Gets the current refresh token from cookies
   */
  async getRefreshToken(): Promise<string | undefined> {
    const cookieStore = await cookies();
    return cookieStore.get("refreshToken")?.value;
  }

  /**
   * Clears all auth cookies
   */
  async clearAuthCookies(): Promise<void> {
    const cookieStore = await cookies();
    cookieStore.delete("authToken");
    cookieStore.delete("refreshToken");
  }

  /**
   * Gets a valid auth token, refreshing if necessary
   * Returns the token or null if unable to get a valid token
   */
  async getValidToken(): Promise<string | null> {
    const authToken = await this.getAuthToken();
    const refreshToken = await this.getRefreshToken();

    // No tokens at all
    if (!authToken && !refreshToken) {
      return null;
    }

    // Auth token exists and is valid
    if (authToken && !this.isTokenExpired(authToken)) {
      return authToken;
    }

    // Auth token expired or missing, try to refresh
    if (refreshToken) {
      const newTokens = await this.refreshAccessToken(refreshToken);

      if (newTokens) {
        await this.updateAuthCookies(newTokens);
        return newTokens.accessToken;
      }
    }

    // Refresh failed
    return null;
  }

  /**
   * Checks if user has valid authentication
   */
  async isAuthenticated(): Promise<boolean> {
    const authToken = await this.getAuthToken();
    const refreshToken = await this.getRefreshToken();

    if (!authToken && !refreshToken) {
      return false;
    }

    if (authToken && !this.isTokenExpired(authToken)) {
      return true;
    }

    // Has refresh token, might be able to refresh
    return !!refreshToken;
  }
}
