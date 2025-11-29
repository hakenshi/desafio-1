import { describe, it, expect, beforeEach, vi } from "vitest";
import { AuthService } from "@/server/services/auth.service";
import { ApiMockHelper } from "../helpers/api-mock.helper";
import { apiClient } from "@/server/services/api-client.service";
import { AuthModel } from "@/server/models/auth.model";

vi.mock("@/server/services/api-client.service");

const createMockTokenResponse = (overrides?: Partial<AuthModel.TokenResponse>): AuthModel.TokenResponse => ({
  access_token: "mock-access-token",
  refresh_token: "mock-refresh-token",
  expires_in: 3600,
  token_type: "Bearer",
  ...overrides,
});

const createMockUserInfo = (overrides?: Partial<AuthModel.UserInfo>): AuthModel.UserInfo => ({
  sub: "user-123",
  username: "testuser",
  email: "test@example.com",
  email_verified: true,
  given_name: "Test",
  family_name: "User",
  roles: ["user"],
  ...overrides,
});

describe("AuthService", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe("Performance Tests", () => {
    it("should login within 200ms", async () => {
      vi.spyOn(apiClient, "post").mockResolvedValue(createMockTokenResponse());

      const responseTime = await ApiMockHelper.measureResponseTime(() =>
        AuthService.login({ email: "test@example.com", password: "password123" })
      );

      expect(responseTime).toBeLessThan(200);
    });

    it("should register within 300ms", async () => {
      vi.spyOn(apiClient, "post").mockResolvedValue(undefined);

      const responseTime = await ApiMockHelper.measureResponseTime(() =>
        AuthService.register({
          username: "newuser",
          email: "new@example.com",
          password: "password123",
        })
      );

      expect(responseTime).toBeLessThan(300);
    });

    it("should refresh token within 200ms", async () => {
      vi.spyOn(apiClient, "post").mockResolvedValue(createMockTokenResponse());

      const responseTime = await ApiMockHelper.measureResponseTime(() =>
        AuthService.refreshToken("mock-refresh-token")
      );

      expect(responseTime).toBeLessThan(200);
    });
  });

  describe("API Integration - login", () => {
    it("should login successfully with valid credentials", async () => {
      const mockResponse = createMockTokenResponse();
      vi.spyOn(apiClient, "post").mockResolvedValue(mockResponse);

      const result = await AuthService.login({
        email: "test@example.com",
        password: "password123",
      });

      expect(result).toEqual(mockResponse);
      expect(apiClient.post).toHaveBeenCalledWith("/auth/login", {
        email: "test@example.com",
        password: "password123",
      });
    });

    it("should validate email format", async () => {
      await expect(
        AuthService.login({ email: "invalid-email", password: "password123" })
      ).rejects.toThrow();
    });

    it("should validate password is not empty", async () => {
      await expect(
        AuthService.login({ email: "test@example.com", password: "" })
      ).rejects.toThrow();
    });

    it("should handle invalid credentials error", async () => {
      vi.spyOn(apiClient, "post").mockRejectedValue({
        statusCode: 401,
        message: "Invalid email or password",
      });

      await expect(
        AuthService.login({ email: "test@example.com", password: "wrong" })
      ).rejects.toMatchObject({ statusCode: 401 });
    });
  });

  describe("API Integration - register", () => {
    it("should register successfully with valid data", async () => {
      vi.spyOn(apiClient, "post").mockResolvedValue(undefined);

      await AuthService.register({
        username: "newuser",
        email: "new@example.com",
        password: "password123",
        firstName: "New",
        lastName: "User",
      });

      expect(apiClient.post).toHaveBeenCalledWith("/auth/register", {
        username: "newuser",
        email: "new@example.com",
        password: "password123",
        firstName: "New",
        lastName: "User",
      });
    });

    it("should validate username minimum length", async () => {
      await expect(
        AuthService.register({
          username: "ab",
          email: "test@example.com",
          password: "password123",
        })
      ).rejects.toThrow();
    });

    it("should validate email format", async () => {
      await expect(
        AuthService.register({
          username: "testuser",
          email: "invalid-email",
          password: "password123",
        })
      ).rejects.toThrow();
    });

    it("should validate password minimum length", async () => {
      await expect(
        AuthService.register({
          username: "testuser",
          email: "test@example.com",
          password: "12345",
        })
      ).rejects.toThrow();
    });

    it("should handle email already exists error", async () => {
      vi.spyOn(apiClient, "post").mockRejectedValue({
        statusCode: 400,
        message: "Email already exists",
      });

      await expect(
        AuthService.register({
          username: "newuser",
          email: "existing@example.com",
          password: "password123",
        })
      ).rejects.toMatchObject({ statusCode: 400 });
    });
  });

  describe("API Integration - refreshToken", () => {
    it("should refresh token successfully", async () => {
      const mockResponse = createMockTokenResponse();
      vi.spyOn(apiClient, "post").mockResolvedValue(mockResponse);

      const result = await AuthService.refreshToken("valid-refresh-token");

      expect(result).toEqual(mockResponse);
      expect(apiClient.post).toHaveBeenCalledWith("/auth/refresh", {
        refresh_token: "valid-refresh-token",
      });
    });

    it("should handle invalid refresh token error", async () => {
      vi.spyOn(apiClient, "post").mockRejectedValue({
        statusCode: 401,
        message: "Invalid or expired refresh token",
      });

      await expect(
        AuthService.refreshToken("invalid-token")
      ).rejects.toMatchObject({ statusCode: 401 });
    });
  });

  describe("API Integration - logout", () => {
    it("should logout successfully with refresh token", async () => {
      vi.spyOn(apiClient, "post").mockResolvedValue(undefined);

      await AuthService.logout("refresh-token");

      expect(apiClient.post).toHaveBeenCalledWith("/auth/logout", {
        refresh_token: "refresh-token",
      });
    });

    it("should logout successfully without refresh token", async () => {
      vi.spyOn(apiClient, "post").mockResolvedValue(undefined);

      await AuthService.logout();

      expect(apiClient.post).toHaveBeenCalledWith("/auth/logout", {
        refresh_token: undefined,
      });
    });
  });

  describe("API Integration - getUserInfo", () => {
    it("should get user info successfully", async () => {
      const mockUserInfo = createMockUserInfo();
      vi.spyOn(apiClient, "get").mockResolvedValue(mockUserInfo);

      const result = await AuthService.getUserInfo("valid-access-token");

      expect(result).toEqual(mockUserInfo);
      expect(apiClient.get).toHaveBeenCalledWith("/auth/userinfo", {
        headers: {
          Authorization: "Bearer valid-access-token",
        },
      });
    });

    it("should handle unauthorized error", async () => {
      vi.spyOn(apiClient, "get").mockRejectedValue({
        statusCode: 401,
        message: "Unauthorized",
      });

      await expect(
        AuthService.getUserInfo("invalid-token")
      ).rejects.toMatchObject({ statusCode: 401 });
    });
  });
});
