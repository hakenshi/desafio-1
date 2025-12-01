import { describe, it, expect } from "vitest";
import { CategoryModel } from "@/server/models/category.model";

describe("CategoryModel Schema Validation", () => {
  describe("CategorySchema", () => {
    it("should validate valid category data", () => {
      const result = CategoryModel.CategorySchema.safeParse({
        id: "cat-123",
        name: "Electronics",
        description: "Electronic devices and accessories",
        createdAt: "2024-01-01T00:00:00.000Z",
        updatedAt: "2024-01-01T00:00:00.000Z",
      });
      expect(result.success).toBe(true);
    });

    it("should reject invalid datetime format", () => {
      const result = CategoryModel.CategorySchema.safeParse({
        id: "cat-123",
        name: "Electronics",
        description: "Electronic devices",
        createdAt: "invalid-date",
        updatedAt: "2024-01-01T00:00:00.000Z",
      });
      expect(result.success).toBe(false);
    });
  });

  describe("CreateCategorySchema", () => {
    it("should validate valid create category data", () => {
      const result = CategoryModel.CreateCategorySchema.safeParse({
        name: "New Category",
        description: "Category description",
      });
      expect(result.success).toBe(true);
    });

    it("should reject empty name", () => {
      const result = CategoryModel.CreateCategorySchema.safeParse({
        name: "",
        description: "Category description",
      });
      expect(result.success).toBe(false);
    });

    it("should reject empty description", () => {
      const result = CategoryModel.CreateCategorySchema.safeParse({
        name: "New Category",
        description: "",
      });
      expect(result.success).toBe(false);
    });
  });

  describe("GetAllCategoriesQuerySchema", () => {
    it("should use default values when not provided", () => {
      const result = CategoryModel.GetAllCategoriesQuerySchema.parse({});
      expect(result.page).toBe(1);
      expect(result.pageSize).toBe(10);
    });

    it("should accept custom page and pageSize", () => {
      const result = CategoryModel.GetAllCategoriesQuerySchema.parse({
        page: 3,
        pageSize: 20,
      });
      expect(result.page).toBe(3);
      expect(result.pageSize).toBe(20);
    });

    it("should reject non-positive page number", () => {
      const result = CategoryModel.GetAllCategoriesQuerySchema.safeParse({
        page: -1,
        pageSize: 10,
      });
      expect(result.success).toBe(false);
    });
  });

  describe("PaginatedCategoriesSchema", () => {
    it("should validate valid paginated response", () => {
      const result = CategoryModel.PaginatedCategoriesSchema.safeParse({
        items: [
          {
            id: "cat-123",
            name: "Electronics",
            description: "Electronic devices",
            createdAt: "2024-01-01T00:00:00.000Z",
            updatedAt: "2024-01-01T00:00:00.000Z",
          },
        ],
        page: 1,
        pageSize: 10,
        totalCount: 50,
        totalPages: 5,
        hasPreviousPage: false,
        hasNextPage: true,
      });
      expect(result.success).toBe(true);
    });

    it("should validate empty items array", () => {
      const result = CategoryModel.PaginatedCategoriesSchema.safeParse({
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
  });
});
