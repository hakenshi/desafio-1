import { describe, it, expect, vi, beforeEach } from "vitest";
import { DashboardService } from "@/server/services/dashboard.service";
import { apiClient } from "@/server/services/api-client.service";

vi.mock("@/server/services/api-client.service", () => ({
  apiClient: {
    get: vi.fn(),
    post: vi.fn(),
    put: vi.fn(),
    delete: vi.fn(),
  },
}));

describe("DashboardService", () => {
  let service: DashboardService;
  const mockToken = "test-token";

  beforeEach(() => {
    service = new DashboardService(mockToken);
    vi.resetAllMocks();
  });

  describe("getData", () => {
    it("should fetch dashboard data", async () => {
      const mockDashboard = {
        totalProducts: 100,
        totalStockValue: 50000,
        lowStockCount: 5,
        productsByCategory: {
          "Category 1": 50,
          "Category 2": 50,
        },
      };

      vi.mocked(apiClient.get).mockResolvedValue(mockDashboard);

      const result = await service.getData();

      expect(result.totalProducts).toBe(100);
      expect(result.totalStockValue).toBe(50000);
      expect(result.lowStockCount).toBe(5);
      expect(apiClient.get).toHaveBeenCalledWith(
        "/dashboard",
        undefined,
        mockToken
      );
    });
  });

  describe("getAuditLogs", () => {
    it("should fetch audit logs with default count", async () => {
      const mockLogs = [
        {
          id: "1",
          userId: "user-1",
          username: "admin",
          action: "Create",
          entityType: "Product",
          entityId: "prod-1",
          entityName: "Test Product",
          details: "Created product",
          createdAt: "2024-01-01T00:00:00Z",
        },
      ];

      vi.mocked(apiClient.get).mockResolvedValue(mockLogs);

      const result = await service.getAuditLogs();

      expect(result).toHaveLength(1);
      expect(result[0].action).toBe("Create");
      expect(apiClient.get).toHaveBeenCalledWith(
        "/dashboard/audit-logs?count=10",
        undefined,
        mockToken
      );
    });

    it("should fetch audit logs with custom count", async () => {
      vi.mocked(apiClient.get).mockResolvedValue([]);

      await service.getAuditLogs(5);

      expect(apiClient.get).toHaveBeenCalledWith(
        "/dashboard/audit-logs?count=5",
        undefined,
        mockToken
      );
    });
  });

  describe("getRecentProducts", () => {
    it("should fetch recent products with default count", async () => {
      const mockProducts = [
        {
          id: "1",
          name: "Test Product",
          price: 99.99,
          categoryName: "Test Category",
          stockQuantity: 100,
          createdAt: "2024-01-01T00:00:00Z",
        },
      ];

      vi.mocked(apiClient.get).mockResolvedValue(mockProducts);

      const result = await service.getRecentProducts();

      expect(result).toHaveLength(1);
      expect(result[0].name).toBe("Test Product");
      expect(apiClient.get).toHaveBeenCalledWith(
        "/dashboard/recent-products?count=10",
        undefined,
        mockToken
      );
    });

    it("should fetch recent products with custom count", async () => {
      vi.mocked(apiClient.get).mockResolvedValue([]);

      await service.getRecentProducts(20);

      expect(apiClient.get).toHaveBeenCalledWith(
        "/dashboard/recent-products?count=20",
        undefined,
        mockToken
      );
    });
  });
});
