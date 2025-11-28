import { describe, it, expect, beforeEach, vi } from "vitest";
import { ProductService } from "@/server/services/product.service";
import { ApiMockHelper } from "../helpers/api-mock.helper";
import { TestDataHelper } from "../helpers/test-data.helper";
import { apiClient } from "@/server/services/api-client.service";

vi.mock("@/server/services/api-client.service");
vi.mock("next/cache", () => ({
  revalidateTag: vi.fn(),
  cacheTag: vi.fn(),
  cacheLife: vi.fn(),
}));

describe("ProductService", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe("Performance Tests", () => {
    it("should fetch all products within 200ms", async () => {
      const mockProducts = [TestDataHelper.createMockProduct()];
      vi.spyOn(apiClient, "get").mockResolvedValue(mockProducts);

      const responseTime = await ApiMockHelper.measureResponseTime(() =>
        ProductService.getAll()
      );

      expect(responseTime).toBeLessThan(200);
    });

    it("should search products within 250ms", async () => {
      const mockProducts = [TestDataHelper.createMockProduct()];
      vi.spyOn(apiClient, "get").mockResolvedValue(mockProducts);

      const responseTime = await ApiMockHelper.measureResponseTime(() =>
        ProductService.search({ searchTerm: "laptop" })
      );

      expect(responseTime).toBeLessThan(250);
    });

    it("should fetch low stock products within 150ms", async () => {
      const mockProducts = [
        TestDataHelper.createMockProduct({ stockQuantity: 5 }),
      ];
      vi.spyOn(apiClient, "get").mockResolvedValue(mockProducts);

      const responseTime = await ApiMockHelper.measureResponseTime(() =>
        ProductService.getLowStock()
      );

      expect(responseTime).toBeLessThan(150);
    });
  });

  describe("API Integration - getAll", () => {
    it("should fetch all products with default pagination", async () => {
      const mockProducts = [TestDataHelper.createMockProduct()];
      vi.spyOn(apiClient, "get").mockResolvedValue(mockProducts);

      const result = await ProductService.getAll();

      expect(result).toEqual(mockProducts);
      expect(apiClient.get).toHaveBeenCalledWith(
        "/products?page=1&pageSize=10"
      );
    });

    it("should fetch products with custom pagination", async () => {
      const mockProducts = [TestDataHelper.createMockProduct()];
      vi.spyOn(apiClient, "get").mockResolvedValue(mockProducts);

      await ProductService.getAll({ page: 2, pageSize: 20 });

      expect(apiClient.get).toHaveBeenCalledWith(
        "/products?page=2&pageSize=20"
      );
    });

    it("should validate response schema", async () => {
      const invalidData = [{ id: "prod-123" }]; // Missing required fields
      vi.spyOn(apiClient, "get").mockResolvedValue(invalidData);

      await expect(ProductService.getAll()).rejects.toThrow();
    });
  });

  describe("API Integration - getById", () => {
    it("should fetch product by ID successfully", async () => {
      const mockProduct = TestDataHelper.createMockProduct();
      vi.spyOn(apiClient, "get").mockResolvedValue(mockProduct);

      const result = await ProductService.getById("prod-123");

      expect(result).toEqual(mockProduct);
      expect(apiClient.get).toHaveBeenCalledWith("/products/prod-123");
    });
  });

  describe("API Integration - search", () => {
    it("should search products by term", async () => {
      const mockProducts = [TestDataHelper.createMockProduct()];
      vi.spyOn(apiClient, "get").mockResolvedValue(mockProducts);

      const result = await ProductService.search({ searchTerm: "laptop" });

      expect(result).toEqual(mockProducts);
      expect(apiClient.get).toHaveBeenCalledWith(
        expect.stringContaining("searchTerm=laptop")
      );
    });

    it("should search products by category", async () => {
      const mockProducts = [TestDataHelper.createMockProduct()];
      vi.spyOn(apiClient, "get").mockResolvedValue(mockProducts);

      await ProductService.search({ categoryId: "cat-123" });

      expect(apiClient.get).toHaveBeenCalledWith(
        expect.stringContaining("categoryId=cat-123")
      );
    });

    it("should search with multiple filters", async () => {
      const mockProducts = [TestDataHelper.createMockProduct()];
      vi.spyOn(apiClient, "get").mockResolvedValue(mockProducts);

      await ProductService.search({
        searchTerm: "laptop",
        categoryId: "cat-123",
        page: 2,
        pageSize: 20,
      });

      const call = (apiClient.get as any).mock.calls[0][0];
      expect(call).toContain("searchTerm=laptop");
      expect(call).toContain("categoryId=cat-123");
      expect(call).toContain("page=2");
      expect(call).toContain("pageSize=20");
    });
  });

  describe("API Integration - getLowStock", () => {
    it("should fetch low stock products", async () => {
      const mockProducts = [
        TestDataHelper.createMockProduct({ stockQuantity: 5 }),
        TestDataHelper.createMockProduct({ stockQuantity: 2 }),
      ];
      vi.spyOn(apiClient, "get").mockResolvedValue(mockProducts);

      const result = await ProductService.getLowStock();

      expect(result).toEqual(mockProducts);
      expect(apiClient.get).toHaveBeenCalledWith("/products/low-stock");
    });
  });

  describe("API Integration - create", () => {
    it("should create product successfully", async () => {
      const createDto = {
        name: "New Laptop",
        description: "High-performance laptop",
        price: 1299.99,
        categoryId: "cat-123",
        stockQuantity: 50,
      };
      const mockProduct = TestDataHelper.createMockProduct(createDto);

      vi.spyOn(apiClient, "post").mockResolvedValue(mockProduct);

      const result = await ProductService.create(createDto);

      expect(result).toEqual(mockProduct);
      expect(apiClient.post).toHaveBeenCalledWith("/products", createDto);
    });

    it("should validate input data - negative price", async () => {
      const invalidDto = {
        name: "Test",
        description: "Test",
        price: -100,
        categoryId: "cat-123",
        stockQuantity: 10,
      };

      await expect(ProductService.create(invalidDto)).rejects.toThrow();
    });

    it("should validate input data - negative stock", async () => {
      const invalidDto = {
        name: "Test",
        description: "Test",
        price: 100,
        categoryId: "cat-123",
        stockQuantity: -5,
      };

      await expect(ProductService.create(invalidDto)).rejects.toThrow();
    });

    it("should validate input data - missing required fields", async () => {
      const invalidDto = {
        name: "",
        description: "",
        price: 0,
        categoryId: "",
        stockQuantity: 0,
      };

      await expect(ProductService.create(invalidDto)).rejects.toThrow();
    });
  });

  describe("API Integration - update", () => {
    it("should update product successfully", async () => {
      const updateDto = {
        name: "Updated Laptop",
        description: "Updated description",
        price: 1499.99,
        categoryId: "cat-123",
        stockQuantity: 30,
      };
      const mockProduct = TestDataHelper.createMockProduct(updateDto);

      vi.spyOn(apiClient, "put").mockResolvedValue(mockProduct);

      const result = await ProductService.update("prod-123", updateDto);

      expect(result).toEqual(mockProduct);
      expect(apiClient.put).toHaveBeenCalledWith("/products/prod-123", updateDto);
    });

    it("should validate update data", async () => {
      const invalidDto = {
        name: "",
        description: "",
        price: -100,
        categoryId: "",
        stockQuantity: -10,
      };

      await expect(ProductService.update("prod-123", invalidDto)).rejects.toThrow();
    });
  });

  describe("API Integration - delete", () => {
    it("should delete product successfully", async () => {
      vi.spyOn(apiClient, "delete").mockResolvedValue(undefined);

      await ProductService.delete("prod-123");

      expect(apiClient.delete).toHaveBeenCalledWith("/products/prod-123");
    });

    it("should handle delete errors", async () => {
      vi.spyOn(apiClient, "delete").mockRejectedValue({
        statusCode: 404,
        message: "Product not found",
      });

      await expect(ProductService.delete("invalid-id")).rejects.toMatchObject({
        statusCode: 404,
      });
    });
  });
});
