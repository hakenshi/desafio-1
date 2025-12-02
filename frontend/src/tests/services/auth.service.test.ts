import { describe, it, expect } from "vitest";
import { AuthModel } from "@/server/models/auth.model";

describe("AuthModel Schema Validation", () => {
  describe("LoginRequestSchema", () => {
    it("should validate valid login data", () => {
      const result = AuthModel.LoginRequestSchema.safeParse({
        email: "test@example.com",
        password: "password123",
      });
      expect(result.success).toBe(true);
    });

    it("should reject invalid email format", () => {
      const result = AuthModel.LoginRequestSchema.safeParse({
        email: "invalid-email",
        password: "password123",
      });
      expect(result.success).toBe(false);
    });

    it("should reject empty password", () => {
      const result = AuthModel.LoginRequestSchema.safeParse({
        email: "test@example.com",
        password: "",
      });
      expect(result.success).toBe(false);
    });
  });

  describe("RegisterRequestSchema", () => {
    it("should validate valid register data", () => {
      const result = AuthModel.RegisterRequestSchema.safeParse({
        username: "testuser",
        email: "test@example.com",
        password: "password123",
        role: "user",
      });
      expect(result.success).toBe(true);
    });

    it("should reject username with less than 3 characters", () => {
      const result = AuthModel.RegisterRequestSchema.safeParse({
        username: "ab",
        email: "test@example.com",
        password: "password123",
        role: "user",
      });
      expect(result.success).toBe(false);
    });

    it("should reject invalid email format", () => {
      const result = AuthModel.RegisterRequestSchema.safeParse({
        username: "testuser",
        email: "invalid-email",
        password: "password123",
        role: "user",
      });
      expect(result.success).toBe(false);
    });

    it("should reject password with less than 6 characters", () => {
      const result = AuthModel.RegisterRequestSchema.safeParse({
        username: "testuser",
        email: "test@example.com",
        password: "12345",
        role: "user",
      });
      expect(result.success).toBe(false);
    });

    it("should accept optional firstName and lastName", () => {
      const result = AuthModel.RegisterRequestSchema.safeParse({
        username: "testuser",
        email: "test@example.com",
        password: "password123",
        firstName: "Test",
        lastName: "User",
        role: "admin",
      });
      expect(result.success).toBe(true);
    });

    it("should reject invalid role", () => {
      const result = AuthModel.RegisterRequestSchema.safeParse({
        username: "testuser",
        email: "test@example.com",
        password: "password123",
        role: "invalid-role",
      });
      expect(result.success).toBe(false);
    });

    it("should reject missing role", () => {
      const result = AuthModel.RegisterRequestSchema.safeParse({
        username: "testuser",
        email: "test@example.com",
        password: "password123",
      });
      expect(result.success).toBe(false);
    });
  });

  describe("TokenResponseSchema", () => {
    it("should validate valid token response", () => {
      const result = AuthModel.TokenResponseSchema.safeParse({
        accessToken: "token123",
        expiresIn: 3600,
        tokenType: "Bearer",
      });
      expect(result.success).toBe(true);
    });

    it("should accept optional refreshToken", () => {
      const result = AuthModel.TokenResponseSchema.safeParse({
        accessToken: "token123",
        refreshToken: "refresh123",
        expiresIn: 3600,
        tokenType: "Bearer",
      });
      expect(result.success).toBe(true);
    });
  });

  describe("UserInfoSchema", () => {
    it("should validate valid user info with required fields", () => {
      const result = AuthModel.UserInfoSchema.safeParse({
        id: "user-123",
        username: "testuser",
        email: "test@example.com",
        role: "user",
      });
      expect(result.success).toBe(true);
    });

    it("should accept optional firstName and lastName", () => {
      const result = AuthModel.UserInfoSchema.safeParse({
        id: "user-123",
        username: "testuser",
        email: "test@example.com",
        firstName: "Test",
        lastName: "User",
        role: "admin",
      });
      expect(result.success).toBe(true);
    });

    it("should accept null firstName and lastName", () => {
      const result = AuthModel.UserInfoSchema.safeParse({
        id: "user-123",
        username: "testuser",
        email: "test@example.com",
        firstName: null,
        lastName: null,
        role: "manager",
      });
      expect(result.success).toBe(true);
    });

    it("should reject invalid role", () => {
      const result = AuthModel.UserInfoSchema.safeParse({
        id: "user-123",
        username: "testuser",
        email: "test@example.com",
        role: "invalid-role",
      });
      expect(result.success).toBe(false);
    });

    it("should reject missing required fields", () => {
      const result = AuthModel.UserInfoSchema.safeParse({
        id: "user-123",
      });
      expect(result.success).toBe(false);
    });
  });

  describe("UpdateUserRequestSchema", () => {
    it("should validate valid update user data", () => {
      const result = AuthModel.UpdateUserRequestSchema.safeParse({
        email: "updated@example.com",
        role: "manager",
      });
      expect(result.success).toBe(true);
    });

    it("should accept optional firstName and lastName", () => {
      const result = AuthModel.UpdateUserRequestSchema.safeParse({
        email: "updated@example.com",
        firstName: "Updated",
        lastName: "User",
        role: "admin",
      });
      expect(result.success).toBe(true);
    });

    it("should reject invalid email format", () => {
      const result = AuthModel.UpdateUserRequestSchema.safeParse({
        email: "invalid-email",
        role: "user",
      });
      expect(result.success).toBe(false);
    });

    it("should reject invalid role", () => {
      const result = AuthModel.UpdateUserRequestSchema.safeParse({
        email: "test@example.com",
        role: "superadmin",
      });
      expect(result.success).toBe(false);
    });

    it("should reject missing email", () => {
      const result = AuthModel.UpdateUserRequestSchema.safeParse({
        role: "user",
      });
      expect(result.success).toBe(false);
    });

    it("should reject missing role", () => {
      const result = AuthModel.UpdateUserRequestSchema.safeParse({
        email: "test@example.com",
      });
      expect(result.success).toBe(false);
    });
  });

  describe("KeycloakUserSchema", () => {
    it("should validate valid keycloak user data", () => {
      const result = AuthModel.KeycloakUserSchema.safeParse({
        id: "kc-user-123",
        username: "kcuser",
        email: "kcuser@example.com",
        enabled: true,
        role: "user",
      });
      expect(result.success).toBe(true);
    });

    it("should accept optional firstName and lastName", () => {
      const result = AuthModel.KeycloakUserSchema.safeParse({
        id: "kc-user-123",
        username: "kcuser",
        email: "kcuser@example.com",
        firstName: "KC",
        lastName: "User",
        enabled: true,
        role: "admin",
      });
      expect(result.success).toBe(true);
    });

    it("should accept null firstName and lastName", () => {
      const result = AuthModel.KeycloakUserSchema.safeParse({
        id: "kc-user-123",
        username: "kcuser",
        email: "kcuser@example.com",
        firstName: null,
        lastName: null,
        enabled: false,
        role: "manager",
      });
      expect(result.success).toBe(true);
    });

    it("should reject missing enabled field", () => {
      const result = AuthModel.KeycloakUserSchema.safeParse({
        id: "kc-user-123",
        username: "kcuser",
        email: "kcuser@example.com",
        role: "user",
      });
      expect(result.success).toBe(false);
    });

    it("should reject invalid role", () => {
      const result = AuthModel.KeycloakUserSchema.safeParse({
        id: "kc-user-123",
        username: "kcuser",
        email: "kcuser@example.com",
        enabled: true,
        role: "invalid",
      });
      expect(result.success).toBe(false);
    });

    it("should reject invalid email format", () => {
      const result = AuthModel.KeycloakUserSchema.safeParse({
        id: "kc-user-123",
        username: "kcuser",
        email: "invalid-email",
        enabled: true,
        role: "user",
      });
      expect(result.success).toBe(false);
    });
  });
});
