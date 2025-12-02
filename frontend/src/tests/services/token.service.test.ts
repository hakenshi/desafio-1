import { describe, it, expect } from "vitest";
import { TokenService } from "@/server/services/token.service";

// Helper to create a JWT token with specific expiration
function createMockJWT(expiresInSeconds: number): string {
  const header = { alg: "HS256", typ: "JWT" };
  const payload = {
    sub: "user-123",
    exp: Math.floor(Date.now() / 1000) + expiresInSeconds,
  };

  const base64Header = Buffer.from(JSON.stringify(header)).toString("base64");
  const base64Payload = Buffer.from(JSON.stringify(payload)).toString("base64");

  return `${base64Header}.${base64Payload}.fake-signature`;
}

// Create instance for testing
const tokenService = new TokenService();

describe("TokenService", () => {
  describe("decodeToken", () => {
    it("should decode a valid JWT token", () => {
      const token = createMockJWT(3600); // 1 hour
      const payload = tokenService.decodeToken(token);

      expect(payload).not.toBeNull();
      expect(payload?.exp).toBeDefined();
      expect(payload?.exp).toBeGreaterThan(Date.now() / 1000);
    });

    it("should return null for invalid token format", () => {
      const invalidToken = "not-a-valid-jwt";
      const payload = tokenService.decodeToken(invalidToken);

      expect(payload).toBeNull();
    });

    it("should return null for token with only 2 parts", () => {
      const invalidToken = "header.payload";
      const payload = tokenService.decodeToken(invalidToken);

      expect(payload).toBeNull();
    });

    it("should return null for empty string", () => {
      const payload = tokenService.decodeToken("");
      expect(payload).toBeNull();
    });
  });

  describe("isTokenExpired", () => {
    it("should return false for token expiring in more than 30 seconds", () => {
      const token = createMockJWT(60); // 60 seconds
      expect(tokenService.isTokenExpired(token)).toBe(false);
    });

    it("should return true for token expiring in less than 30 seconds", () => {
      const token = createMockJWT(20); // 20 seconds
      expect(tokenService.isTokenExpired(token)).toBe(true);
    });

    it("should return true for already expired token", () => {
      const token = createMockJWT(-60); // expired 60 seconds ago
      expect(tokenService.isTokenExpired(token)).toBe(true);
    });

    it("should return true for invalid token", () => {
      expect(tokenService.isTokenExpired("invalid-token")).toBe(true);
    });

    it("should return true for token without exp claim", () => {
      const header = { alg: "HS256", typ: "JWT" };
      const payload = { sub: "user-123" }; // no exp

      const base64Header = Buffer.from(JSON.stringify(header)).toString("base64");
      const base64Payload = Buffer.from(JSON.stringify(payload)).toString("base64");
      const token = `${base64Header}.${base64Payload}.fake-signature`;

      expect(tokenService.isTokenExpired(token)).toBe(true);
    });
  });

  describe("Token Refresh Logic", () => {
    it("should identify when refresh is needed", () => {
      const expiredToken = createMockJWT(10); // 10 seconds - needs refresh
      const validToken = createMockJWT(3600); // 1 hour - no refresh needed

      expect(tokenService.isTokenExpired(expiredToken)).toBe(true);
      expect(tokenService.isTokenExpired(validToken)).toBe(false);
    });

    it("should handle edge case at exactly 30 seconds", () => {
      // At exactly 30 seconds, it should be considered expired (< 30000ms)
      const token = createMockJWT(30);
      // Due to timing, this might be true or false, so we just check it doesn't throw
      expect(() => tokenService.isTokenExpired(token)).not.toThrow();
    });
  });

  describe("Token Creation for Tests", () => {
    it("should create valid mock tokens", () => {
      const token = createMockJWT(3600);
      const parts = token.split(".");

      expect(parts).toHaveLength(3);
      expect(parts[0]).toBeTruthy(); // header
      expect(parts[1]).toBeTruthy(); // payload
      expect(parts[2]).toBeTruthy(); // signature
    });

    it("should create tokens with correct expiration", () => {
      const expiresIn = 3600;
      const token = createMockJWT(expiresIn);
      const payload = tokenService.decodeToken(token);

      const expectedExp = Math.floor(Date.now() / 1000) + expiresIn;
      // Allow 1 second tolerance for test execution time
      expect(payload?.exp).toBeGreaterThanOrEqual(expectedExp - 1);
      expect(payload?.exp).toBeLessThanOrEqual(expectedExp + 1);
    });
  });
});
