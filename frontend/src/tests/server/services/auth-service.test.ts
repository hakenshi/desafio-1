import { describe, it, expect, vi, beforeEach } from "vitest";
import { AuthService } from "@/server/services/auth.service";
import { apiClient } from "@/server/services/api-client.service";

vi.mock("next/headers", () => ({
  cookies: vi.fn(() => Promise.resolve({
    get: vi.fn(),
    set: vi.fn(),
    delete: vi.fn(),
  })),
}));

vi.mock("next/navigation", () => ({
  redirect: vi.fn(),
}));

vi.mock("@/server/services/api-client.service", () => ({
  apiClient: {
    get: vi.fn(),
    post: vi.fn(),
    put: vi.fn(),
    delete: vi.fn(),
  },
}));

describe("AuthService", () => {
  let service: AuthService;
  const mockToken = "test-token";

  beforeEach(() => {
    service = new AuthService(mockToken);
    vi.resetAllMocks();
  });

  describe("register", () => {
    it("should register a new user", async () => {
      const registerData = {
        username: "testuser",
        email: "test@example.com",
        password: "password123",
        role: "user" as const,
      };

      vi.mocked(apiClient.post).mockResolvedValue(undefined);

      await service.register(registerData);

      expect(apiClient.post).toHaveBeenCalledWith(
        "/auth/register",
        registerData
      );
    });

    it("should register user with optional fields", async () => {
      const registerData = {
        username: "testuser",
        email: "test@example.com",
        password: "password123",
        firstName: "Test",
        lastName: "User",
        role: "admin" as const,
      };

      vi.mocked(apiClient.post).mockResolvedValue(undefined);

      await service.register(registerData);

      expect(apiClient.post).toHaveBeenCalledWith(
        "/auth/register",
        registerData
      );
    });
  });

  describe("logout", () => {
    it("should logout user", async () => {
      vi.mocked(apiClient.post).mockResolvedValue(undefined);

      await service.logout("refresh-token");

      expect(apiClient.post).toHaveBeenCalledWith(
        "/auth/logout",
        { refresh_token: "refresh-token" },
        undefined,
        mockToken
      );
    });

    it("should logout without refresh token", async () => {
      vi.mocked(apiClient.post).mockResolvedValue(undefined);

      await service.logout();

      expect(apiClient.post).toHaveBeenCalledWith(
        "/auth/logout",
        { refresh_token: undefined },
        undefined,
        mockToken
      );
    });
  });

  describe("getUserInfo", () => {
    it("should get current user info", async () => {
      const mockUser = {
        id: "user-1",
        username: "testuser",
        email: "test@example.com",
        firstName: "Test",
        lastName: "User",
        role: "user",
      };

      vi.mocked(apiClient.get).mockResolvedValue(mockUser);

      const result = await service.getUserInfo();

      expect(result.id).toBe("user-1");
      expect(result.username).toBe("testuser");
      expect(apiClient.get).toHaveBeenCalledWith(
        "/auth/me",
        { cache: "no-store" },
        mockToken
      );
    });
  });

  describe("getUsers", () => {
    it("should get all users", async () => {
      const mockUsers = [
        {
          id: "user-1",
          username: "admin",
          email: "admin@example.com",
          firstName: "Admin",
          lastName: "User",
          enabled: true,
          role: "admin",
        },
        {
          id: "user-2",
          username: "user",
          email: "user@example.com",
          firstName: "Regular",
          lastName: "User",
          enabled: true,
          role: "user",
        },
      ];

      vi.mocked(apiClient.get).mockResolvedValue(mockUsers);

      const result = await service.getUsers();

      expect(result).toHaveLength(2);
      expect(result[0].role).toBe("admin");
      expect(apiClient.get).toHaveBeenCalledWith(
        "/auth/users",
        undefined,
        mockToken
      );
    });
  });

  describe("updateUser", () => {
    it("should update user", async () => {
      const updateData = {
        email: "updated@example.com",
        firstName: "Updated",
        lastName: "User",
        role: "manager" as const,
      };

      vi.mocked(apiClient.put).mockResolvedValue(undefined);

      await service.updateUser("user-1", updateData);

      expect(apiClient.put).toHaveBeenCalledWith(
        "/auth/users/user-1",
        updateData,
        undefined,
        mockToken
      );
    });
  });

  describe("deleteUser", () => {
    it("should delete user", async () => {
      vi.mocked(apiClient.delete).mockResolvedValue(undefined);

      await service.deleteUser("user-1");

      expect(apiClient.delete).toHaveBeenCalledWith(
        "/auth/users/user-1",
        undefined,
        mockToken
      );
    });
  });
});
