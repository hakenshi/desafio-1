import { describe, it, expect } from "vitest";
import { AuthModel } from "../../../server/models/auth.model";

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
      });
      expect(result.success).toBe(true);
    });

    it("should reject username with less than 3 characters", () => {
      const result = AuthModel.RegisterRequestSchema.safeParse({
        username: "ab",
        email: "test@example.com",
        password: "password123",
      });
      expect(result.success).toBe(false);
    });

    it("should reject invalid email format", () => {
      const result = AuthModel.RegisterRequestSchema.safeParse({
        username: "testuser",
        email: "invalid-email",
        password: "password123",
      });
      expect(result.success).toBe(false);
    });

    it("should reject password with less than 6 characters", () => {
      const result = AuthModel.RegisterRequestSchema.safeParse({
        username: "testuser",
        email: "test@example.com",
        password: "12345",
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
      });
      expect(result.success).toBe(true);
    });
  });

  describe("TokenResponseSchema", () => {
    it("should validate valid token response", () => {
      const result = AuthModel.TokenResponseSchema.safeParse({
        access_token: "token123",
        expires_in: 3600,
        token_type: "Bearer",
      });
      expect(result.success).toBe(true);
    });

    it("should accept optional refresh_token", () => {
      const result = AuthModel.TokenResponseSchema.safeParse({
        access_token: "token123",
        refresh_token: "refresh123",
        expires_in: 3600,
        token_type: "Bearer",
      });
      expect(result.success).toBe(true);
    });
  });

  describe("UserInfoSchema", () => {
    it("should validate valid user info", () => {
      const result = AuthModel.UserInfoSchema.safeParse({
        sub: "user-123",
      });
      expect(result.success).toBe(true);
    });

    it("should accept all optional fields", () => {
      const result = AuthModel.UserInfoSchema.safeParse({
        sub: "user-123",
        username: "testuser",
        email: "test@example.com",
        email_verified: true,
        given_name: "Test",
        family_name: "User",
        roles: ["user", "admin"],
      });
      expect(result.success).toBe(true);
    });
  });
});
