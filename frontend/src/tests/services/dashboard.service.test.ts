import { describe, it, expect, beforeEach, vi } from "vitest";
import { DashboardService } from "@/server/services/dashboard.service";
import { ApiMockHelper } from "../helpers/api-mock.helper";
import { TestDataHelper } from "../helpers/test-data.helper";
import { apiClient } from "@/server/services/api-client.service";

vi.mock("@/server/services/api-client.service");
vi.mock("next/cache", () => ({
  cacheTag: vi.fn(),
  cacheLife: vi.fn(),
}));

describe("DashboardService", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe("Performance Tests", () => {
    it("should fetch dashboard data within 300ms", async () => {
      const mockDashboard = TestDataHelper.createMockDashboard();
      vi.spyOn(apiClient, "get").mockResolvedValue(mockDashboard);

      const responseTime = await ApiMockHelper.measureResponseTime(() =>
        DashboardService.getData()
      );

      expect(responseTime).toBeLessThan(300);
    });

    it("should handle large datasets within acceptable time", async () => {
      const largeDashboard = TestDataHelper.createMockDashboard({
        totalProducts: 10000,
        lowStockProducts: Array.from({ length: 100 }, (_, i) =>
          TestDataHelper.createMockProduct({
            id: `prod-${i}`,
            stockQuantity: i % 10,
          })
        ),
        productsByCategory: Object.fromEntries(
          Array.from({ length: 50 }, (_, i) => [`Category ${i}`, i * 10])
        ),
      });

      vi.spyOn(apiClient, "get").mockResolvedValue(largeDashboard);

      const responseTime = await ApiMockHelper.measureResponseTime(() =>
        DashboardService.getData()
      );

      expect(responseTime).toBeLessThan(500);
    });
  });

  describe("API Integration - getData", () => {
    it("should fetch dashboard data successfully", async () => {
      const mockDashboard = TestDataHelper.createMockDashboard();
      vi.spyOn(apiClient, "get").mockResolvedValue(mockDashboard);

      const result = await DashboardService.getData();

      expect(result).toEqual(mockDashboard);
      expect(apiClient.get).toHaveBeenCalledWith("/dashboard");
    });

    it("should validate dashboard schema", async () => {
      const invalidData = {
        totalProducts: "invalid", // Should be number
        totalStockValue: 50000,
      };
      vi.spyOn(apiClient, "get").mockResolvedValue(invalidData);

      await expect(DashboardService.getData()).rejects.toThrow();
    });

    it("should handle empty low stock products", async () => {
      const mockDashboard = TestDataHelper.createMockDashboard({
        lowStockProducts: [],
      });
      vi.spyOn(apiClient, "get").mockResolvedValue(mockDashboard);

      const result = await DashboardService.getData();

      expect(result.lowStockProducts).toEqual([]);
    });

    it("should handle empty products by category", async () => {
      const mockDashboard = TestDataHelper.createMockDashboard({
        productsByCategory: {},
      });
      vi.spyOn(apiClient, "get").mockResolvedValue(mockDashboard);

      const result = await DashboardService.getData();

      expect(result.productsByCategory).toEqual({});
    });

    it("should handle API errors", async () => {
      vi.spyOn(apiClient, "get").mockRejectedValue({
        statusCode: 500,
        message: "Internal server error",
      });

      await expect(DashboardService.getData()).rejects.toMatchObject({
        statusCode: 500,
      });
    });
  });

  describe("Data Integrity", () => {
    it("should return correct data types", async () => {
      const mockDashboard = TestDataHelper.createMockDashboard();
      vi.spyOn(apiClient, "get").mockResolvedValue(mockDashboard);

      const result = await DashboardService.getData();

      expect(typeof result.totalProducts).toBe("number");
      expect(typeof result.totalStockValue).toBe("number");
      expect(Array.isArray(result.lowStockProducts)).toBe(true);
      expect(typeof result.productsByCategory).toBe("object");
    });

    it("should validate low stock products structure", async () => {
      const mockDashboard = TestDataHelper.createMockDashboard();
      vi.spyOn(apiClient, "get").mockResolvedValue(mockDashboard);

      const result = await DashboardService.getData();

      result.lowStockProducts.forEach((product) => {
        expect(product).toHaveProperty("id");
        expect(product).toHaveProperty("name");
        expect(product).toHaveProperty("stockQuantity");
        expect(typeof product.stockQuantity).toBe("number");
      });
    });
  });
});
