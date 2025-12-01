import { describe, it, expect } from "vitest";
import { z } from "zod";
import { PaginationModel } from "@/server/models/pagination.model";

describe("PaginationModel Schema Validation", () => {
  describe("PaginatedResponseSchema", () => {
    const TestItemSchema = z.object({
      id: z.string(),
      name: z.string(),
    });

    const TestPaginatedSchema = PaginationModel.PaginatedResponseSchema(TestItemSchema);

    it("should validate valid paginated response", () => {
      const result = TestPaginatedSchema.safeParse({
        items: [
          { id: "1", name: "Item 1" },
          { id: "2", name: "Item 2" },
        ],
        page: 1,
        pageSize: 10,
        totalCount: 100,
        totalPages: 10,
        hasPreviousPage: false,
        hasNextPage: true,
      });
      expect(result.success).toBe(true);
    });

    it("should validate empty items array", () => {
      const result = TestPaginatedSchema.safeParse({
        items: [],
        page: 1,
        pageSize: 10,
        totalCount: 0,
        totalPages: 0,
        hasPreviousPage: false,
        hasNextPage: false,
      });
      expect(result.success).toBe(true);
    });

    it("should reject missing required fields", () => {
      const result = TestPaginatedSchema.safeParse({
        items: [],
        page: 1,
      });
      expect(result.success).toBe(false);
    });

    it("should reject invalid item schema", () => {
      const result = TestPaginatedSchema.safeParse({
        items: [{ invalid: "data" }],
        page: 1,
        pageSize: 10,
        totalCount: 1,
        totalPages: 1,
        hasPreviousPage: false,
        hasNextPage: false,
      });
      expect(result.success).toBe(false);
    });

    it("should validate pagination flags correctly", () => {
      const result = TestPaginatedSchema.safeParse({
        items: [],
        page: 5,
        pageSize: 10,
        totalCount: 100,
        totalPages: 10,
        hasPreviousPage: true,
        hasNextPage: true,
      });
      expect(result.success).toBe(true);
      if (result.success) {
        expect(result.data.hasPreviousPage).toBe(true);
        expect(result.data.hasNextPage).toBe(true);
      }
    });
  });

  describe("PaginatedResponse type", () => {
    it("should correctly type paginated response", () => {
      type TestItem = { id: string; name: string };
      const response: PaginationModel.PaginatedResponse<TestItem> = {
        items: [{ id: "1", name: "Test" }],
        page: 1,
        pageSize: 10,
        totalCount: 1,
        totalPages: 1,
        hasPreviousPage: false,
        hasNextPage: false,
      };
      
      expect(response.items).toHaveLength(1);
      expect(response.page).toBe(1);
      expect(response.totalCount).toBe(1);
    });
  });

  describe("PaginationParams type", () => {
    it("should allow optional page and pageSize", () => {
      const params1: PaginationModel.PaginationParams = {};
      const params2: PaginationModel.PaginationParams = { page: 1 };
      const params3: PaginationModel.PaginationParams = { pageSize: 20 };
      const params4: PaginationModel.PaginationParams = { page: 2, pageSize: 25 };
      
      expect(params1.page).toBeUndefined();
      expect(params2.page).toBe(1);
      expect(params3.pageSize).toBe(20);
      expect(params4.page).toBe(2);
      expect(params4.pageSize).toBe(25);
    });
  });
});
