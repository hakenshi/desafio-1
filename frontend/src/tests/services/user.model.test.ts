import { describe, it, expect } from "vitest";
import { UserModel } from "../../server/models/user.model";

describe("UserModel Schema Validation", () => {
  describe("UserSchema", () => {
    it("should validate valid user data", () => {
      const result = UserModel.UserSchema.safeParse({
        id: "user-1",
        username: "testuser",
        email: "test@example.com",
        firstName: "Test",
        lastName: "User",
        role: "user",
      });
      expect(result.success).toBe(true);
    });

    it("should validate user with admin role", () => {
      const result = UserModel.UserSchema.safeParse({
        id: "user-1",
        username: "admin",
        email: "admin@example.com",
        role: "admin",
      });
      expect(result.success).toBe(true);
    });

    it("should validate user with manager role", () => {
      const result = UserModel.UserSchema.safeParse({
        id: "user-1",
        username: "manager",
        email: "manager@example.com",
        role: "manager",
      });
      expect(result.success).toBe(true);
    });

    it("should accept null firstName and lastName", () => {
      const result = UserModel.UserSchema.safeParse({
        id: "user-1",
        username: "testuser",
        email: "test@example.com",
        firstName: null,
        lastName: null,
        role: "user",
      });
      expect(result.success).toBe(true);
    });

    it("should accept missing optional fields", () => {
      const result = UserModel.UserSchema.safeParse({
        id: "user-1",
        username: "testuser",
        email: "test@example.com",
        role: "user",
      });
      expect(result.success).toBe(true);
    });

    it("should reject invalid email format", () => {
      const result = UserModel.UserSchema.safeParse({
        id: "user-1",
        username: "testuser",
        email: "invalid-email",
        role: "user",
      });
      expect(result.success).toBe(false);
    });

    it("should reject invalid role", () => {
      const result = UserModel.UserSchema.safeParse({
        id: "user-1",
        username: "testuser",
        email: "test@example.com",
        role: "invalid-role",
      });
      expect(result.success).toBe(false);
    });

    it("should reject missing required fields", () => {
      const result = UserModel.UserSchema.safeParse({
        id: "user-1",
        username: "testuser",
      });
      expect(result.success).toBe(false);
    });
  });
});
