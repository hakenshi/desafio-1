import { describe, it, expect } from "vitest";
import { z } from "zod";
import { ApiModel } from "@/server/models/api-response.model";

describe("ApiModel Schema Validation", () => {
  describe("ApiErrorSchema", () => {
    it("should validate valid error response", () => {
      const result = ApiModel.ApiErrorSchema.safeParse({
        message: "An error occurred",
        statusCode: 400,
      });
      expect(result.success).toBe(true);
    });

    it("should validate error with field errors", () => {
      const result = ApiModel.ApiErrorSchema.safeParse({
        message: "Validation failed",
        errors: {
          name: ["Name is required", "Name must be at least 3 characters"],
          email: ["Invalid email format"],
        },
        statusCode: 422,
      });
      expect(result.success).toBe(true);
    });

    it("should accept missing optional fields", () => {
      const result = ApiModel.ApiErrorSchema.safeParse({
        message: "Error",
      });
      expect(result.success).toBe(true);
    });

    it("should reject missing message", () => {
      const result = ApiModel.ApiErrorSchema.safeParse({
        statusCode: 500,
      });
      expect(result.success).toBe(false);
    });
  });

  describe("ApiSuccessSchema", () => {
    it("should validate success response with string data", () => {
      const schema = ApiModel.ApiSuccessSchema(z.string());
      const result = schema.safeParse({
        data: "Success",
        message: "Operation completed",
      });
      expect(result.success).toBe(true);
    });

    it("should validate success response with object data", () => {
      const dataSchema = z.object({
        id: z.string(),
        name: z.string(),
      });
      const schema = ApiModel.ApiSuccessSchema(dataSchema);
      const result = schema.safeParse({
        data: { id: "1", name: "Test" },
      });
      expect(result.success).toBe(true);
    });

    it("should validate success response with array data", () => {
      const schema = ApiModel.ApiSuccessSchema(z.array(z.number()));
      const result = schema.safeParse({
        data: [1, 2, 3],
        message: "Items retrieved",
      });
      expect(result.success).toBe(true);
    });

    it("should accept missing optional message", () => {
      const schema = ApiModel.ApiSuccessSchema(z.string());
      const result = schema.safeParse({
        data: "Success",
      });
      expect(result.success).toBe(true);
    });

    it("should reject invalid data type", () => {
      const schema = ApiModel.ApiSuccessSchema(z.number());
      const result = schema.safeParse({
        data: "not a number",
      });
      expect(result.success).toBe(false);
    });
  });
});
