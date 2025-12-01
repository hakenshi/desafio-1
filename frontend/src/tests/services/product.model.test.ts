import { describe, it, expect } from "vitest";
import { ProductModel } from "@/server/models/product.model";

describe("ProductModel Schema Validation", () => {
  describe("ProductSchema", () => {
    it("should validate valid product data", () => {
      const result = ProductModel.ProductSchema.safeParse({
        id: "prod-123",
        name: "Test Product",
        description: "A test product description",
        price: 99.99,
        categoryId: "cat-123",
        categoryName: "Electronics",
        stockQuantity: 50,
        isLowStock: false,
        createdAt: "2024-01-01T00:00:00.000Z",
        updatedAt: "2024-01-01T00:00:00.000Z",
      });
      expect(result.success).toBe(true);
    });

    it("should validate product with low stock", () => {
      const result = ProductModel.ProductSchema.safeParse({
        id: "prod-123",
        name: "Test Product",
        description: "A test product description",
        price: 99.99,
        categoryId: "cat-123",
        categoryName: "Electronics",
        stockQuantity: 5,
        isLowStock: true,
        createdAt: "2024-01-01T00:00:00.000Z",
        updatedAt: "2024-01-01T00:00:00.000Z",
      });
      expect(result.success).toBe(true);
      if (result.success) {
        expect(result.data.isLowStock).toBe(true);
      }
    });

    it("should reject invalid datetime format", () => {
      const result = ProductModel.ProductSchema.safeParse({
        id: "prod-123",
        name: "Test Product",
        description: "A test product description",
        price: 99.99,
        categoryId: "cat-123",
        categoryName: "Electronics",
        stockQuantity: 50,
        isLowStock: false,
        createdAt: "invalid-date",
        updatedAt: "2024-01-01T00:00:00.000Z",
      });
      expect(result.success).toBe(false);
    });
  });

  describe("CreateProductSchema", () => {
    it("should validate valid create product data", () => {
      const result = ProductModel.CreateProductSchema.safeParse({
        name: "New Product",
        description: "Product description",
        price: 49.99,
        categoryId: "cat-123",
        stockQuantity: 100,
      });
      expect(result.success).toBe(true);
    });

    it("should reject empty name", () => {
      const result = ProductModel.CreateProductSchema.safeParse({
        name: "",
        description: "Product description",
        price: 49.99,
        categoryId: "cat-123",
        stockQuantity: 100,
      });
      expect(result.success).toBe(false);
    });

    it("should reject negative price", () => {
      const result = ProductModel.CreateProductSchema.safeParse({
        name: "New Product",
        description: "Product description",
        price: -10,
        categoryId: "cat-123",
        stockQuantity: 100,
      });
      expect(result.success).toBe(false);
    });

    it("should reject negative stock quantity", () => {
      const result = ProductModel.CreateProductSchema.safeParse({
        name: "New Product",
        description: "Product description",
        price: 49.99,
        categoryId: "cat-123",
        stockQuantity: -5,
      });
      expect(result.success).toBe(false);
    });
  });

  describe("GetAllProductsQuerySchema", () => {
    it("should use default values when not provided", () => {
      const result = ProductModel.GetAllProductsQuerySchema.parse({});
      expect(result.page).toBe(1);
      expect(result.pageSize).toBe(10);
    });

    it("should accept custom page and pageSize", () => {
      const result = ProductModel.GetAllProductsQuerySchema.parse({
        page: 5,
        pageSize: 25,
      });
      expect(result.page).toBe(5);
      expect(result.pageSize).toBe(25);
    });

    it("should reject non-positive page number", () => {
      const result = ProductModel.GetAllProductsQuerySchema.safeParse({
        page: 0,
        pageSize: 10,
      });
      expect(result.success).toBe(false);
    });
  });

  describe("PaginatedProductsSchema", () => {
    it("should validate valid paginated response", () => {
      const result = ProductModel.PaginatedProductsSchema.safeParse({
        items: [
          {
            id: "prod-123",
            name: "Test Product",
            description: "Description",
            price: 99.99,
            categoryId: "cat-123",
            categoryName: "Electronics",
            stockQuantity: 50,
            isLowStock: false,
            createdAt: "2024-01-01T00:00:00.000Z",
            updatedAt: "2024-01-01T00:00:00.000Z",
          },
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
      const result = ProductModel.PaginatedProductsSchema.safeParse({
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
