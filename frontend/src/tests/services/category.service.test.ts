import { describe, it, expect, beforeEach, vi } from "vitest";
import { CategoryService } from "@/server/services/category.service";
import { ApiMockHelper } from "../helpers/api-mock.helper";
import { TestDataHelper } from "../helpers/test-data.helper";
import { apiClient } from "@/server/services/api-client.service";

vi.mock("@/server/services/api-client.service");
vi.mock("next/cache", () => ({
  revalidateTag: vi.fn(),
  cacheTag: vi.fn(),
  cacheLife: vi.fn(),
}));

describe("CategoryService", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe("Performance Tests", () => {
    it("should fetch all categories within 200ms", async () => {
      const mockCategories = [
        TestDataHelper.createMockCategory(),
        TestDataHelper.createMockCategory({ id: "cat-456", name: "Clothing" }),
      ];

      vi.spyOn(apiClient, "get").mockResolvedValue(mockCategories);

      const responseTime = await ApiMockHelper.measureResponseTime(() =>
        CategoryService.getAll()
      );

      expect(responseTime).toBeLessThan(200);
    });

    it("should fetch category by ID within 150ms", async () => {
      const mockCategory = TestDataHelper.createMockCategory();
      vi.spyOn(apiClient, "get").mockResolvedValue(mockCategory);

      const responseTime = await ApiMockHelper.measureResponseTime(() =>
        CategoryService.getById("cat-123")
      );

      expect(responseTime).toBeLessThan(150);
    });

    it("should create category within 300ms", async () => {
      const mockCategory = TestDataHelper.createMockCategory();
      vi.spyOn(apiClient, "post").mockResolvedValue(mockCategory);

      const responseTime = await ApiMockHelper.measureResponseTime(() =>
        CategoryService.create({
          name: "Electronics",
          description: "Electronic devices",
        })
      );

      expect(responseTime).toBeLessThan(300);
    });
  });

  describe("API Integration - getAll", () => {
    it("should fetch all categories successfully", async () => {
      const mockCategories = [
        TestDataHelper.createMockCategory(),
        TestDataHelper.createMockCategory({ id: "cat-456", name: "Clothing" }),
      ];

      vi.spyOn(apiClient, "get").mockResolvedValue(mockCategories);

      const result = await CategoryService.getAll();

      expect(result).toEqual(mockCategories);
      expect(apiClient.get).toHaveBeenCalledWith("/categories");
    });

    it("should validate response schema", async () => {
      const invalidData = [{ id: "cat-123", name: "Test" }]; // Missing required fields
      vi.spyOn(apiClient, "get").mockResolvedValue(invalidData);

      await expect(CategoryService.getAll()).rejects.toThrow();
    });
  });

  describe("API Integration - getById", () => {
    it("should fetch category by ID successfully", async () => {
      const mockCategory = TestDataHelper.createMockCategory();
      vi.spyOn(apiClient, "get").mockResolvedValue(mockCategory);

      const result = await CategoryService.getById("cat-123");

      expect(result).toEqual(mockCategory);
      expect(apiClient.get).toHaveBeenCalledWith("/categories/cat-123");
    });

    it("should handle not found error", async () => {
      vi.spyOn(apiClient, "get").mockRejectedValue({
        statusCode: 404,
        message: "Category not found",
      });

      await expect(CategoryService.getById("invalid-id")).rejects.toMatchObject({
        statusCode: 404,
      });
    });
  });

  describe("API Integration - create", () => {
    it("should create category successfully", async () => {
      const createDto = {
        name: "Electronics",
        description: "Electronic devices",
      };
      const mockCategory = TestDataHelper.createMockCategory(createDto);

      vi.spyOn(apiClient, "post").mockResolvedValue(mockCategory);

      const result = await CategoryService.create(createDto);

      expect(result).toEqual(mockCategory);
      expect(apiClient.post).toHaveBeenCalledWith("/categories", createDto);
    });

    it("should validate input data", async () => {
      const invalidDto = {
        name: "",
        description: "",
      };

      await expect(CategoryService.create(invalidDto)).rejects.toThrow();
    });

    it("should handle validation errors from API", async () => {
      const createDto = {
        name: "Test",
        description: "Test description",
      };

      vi.spyOn(apiClient, "post").mockRejectedValue({
        statusCode: 400,
        message: "Validation failed",
        errors: { name: ["Name already exists"] },
      });

      await expect(CategoryService.create(createDto)).rejects.toMatchObject({
        statusCode: 400,
        errors: { name: ["Name already exists"] },
      });
    });
  });

  describe("API Integration - update", () => {
    it("should update category successfully", async () => {
      const updateDto = {
        name: "Updated Electronics",
        description: "Updated description",
      };
      const mockCategory = TestDataHelper.createMockCategory(updateDto);

      vi.spyOn(apiClient, "put").mockResolvedValue(mockCategory);

      const result = await CategoryService.update("cat-123", updateDto);

      expect(result).toEqual(mockCategory);
      expect(apiClient.put).toHaveBeenCalledWith("/categories/cat-123", updateDto);
    });

    it("should validate update data", async () => {
      const invalidDto = {
        name: "",
        description: "",
      };

      await expect(CategoryService.update("cat-123", invalidDto)).rejects.toThrow();
    });
  });

  describe("API Integration - delete", () => {
    it("should delete category successfully", async () => {
      vi.spyOn(apiClient, "delete").mockResolvedValue(undefined);

      await CategoryService.delete("cat-123");

      expect(apiClient.delete).toHaveBeenCalledWith("/categories/cat-123");
    });

    it("should handle delete errors", async () => {
      vi.spyOn(apiClient, "delete").mockRejectedValue({
        statusCode: 404,
        message: "Category not found",
      });

      await expect(CategoryService.delete("invalid-id")).rejects.toMatchObject({
        statusCode: 404,
      });
    });
  });
});
