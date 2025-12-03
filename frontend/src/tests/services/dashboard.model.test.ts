import { describe, it, expect } from "vitest";
import { DashboardModel } from "../../server/models/dashboard.model";

describe("DashboardModel Schema Validation", () => {
  describe("DashboardSchema", () => {
    it("should validate valid dashboard data", () => {
      const result = DashboardModel.DashboardSchema.safeParse({
        totalProducts: 100,
        totalStockValue: 50000,
        lowStockCount: 5,
        productsByCategory: {
          "Electronics": 50,
          "Clothing": 30,
        },
      });
      expect(result.success).toBe(true);
    });

    it("should reject missing required fields", () => {
      const result = DashboardModel.DashboardSchema.safeParse({
        totalProducts: 100,
      });
      expect(result.success).toBe(false);
    });

    it("should reject invalid productsByCategory type", () => {
      const result = DashboardModel.DashboardSchema.safeParse({
        totalProducts: 100,
        totalStockValue: 50000,
        lowStockCount: 5,
        productsByCategory: "invalid",
      });
      expect(result.success).toBe(false);
    });

    it("should accept empty productsByCategory", () => {
      const result = DashboardModel.DashboardSchema.safeParse({
        totalProducts: 0,
        totalStockValue: 0,
        lowStockCount: 0,
        productsByCategory: {},
      });
      expect(result.success).toBe(true);
    });
  });

  describe("AuditLogSchema", () => {
    it("should validate valid audit log data", () => {
      const result = DashboardModel.AuditLogSchema.safeParse({
        id: "1",
        userId: "user-1",
        username: "admin",
        action: "Create",
        entityType: "Product",
        entityId: "prod-1",
        entityName: "Test Product",
        details: "Created product",
        createdAt: "2024-01-01T00:00:00Z",
      });
      expect(result.success).toBe(true);
    });

    it("should accept null entityName and details", () => {
      const result = DashboardModel.AuditLogSchema.safeParse({
        id: "1",
        userId: "user-1",
        username: "admin",
        action: "Delete",
        entityType: "Product",
        entityId: "prod-1",
        entityName: null,
        details: null,
        createdAt: "2024-01-01T00:00:00Z",
      });
      expect(result.success).toBe(true);
    });

    it("should accept missing optional fields", () => {
      const result = DashboardModel.AuditLogSchema.safeParse({
        id: "1",
        userId: "user-1",
        username: "admin",
        action: "Delete",
        entityType: "Product",
        entityId: "prod-1",
        createdAt: "2024-01-01T00:00:00Z",
      });
      expect(result.success).toBe(true);
    });

    it("should reject invalid datetime format", () => {
      const result = DashboardModel.AuditLogSchema.safeParse({
        id: "1",
        userId: "user-1",
        username: "admin",
        action: "Create",
        entityType: "Product",
        entityId: "prod-1",
        createdAt: "invalid-date",
      });
      expect(result.success).toBe(false);
    });
  });

  describe("RecentProductSchema", () => {
    it("should validate valid recent product data", () => {
      const result = DashboardModel.RecentProductSchema.safeParse({
        id: "1",
        name: "Test Product",
        price: 99.99,
        categoryName: "Electronics",
        stockQuantity: 100,
        createdAt: "2024-01-01T00:00:00Z",
      });
      expect(result.success).toBe(true);
    });

    it("should reject missing required fields", () => {
      const result = DashboardModel.RecentProductSchema.safeParse({
        id: "1",
        name: "Test Product",
      });
      expect(result.success).toBe(false);
    });

    it("should reject invalid price type", () => {
      const result = DashboardModel.RecentProductSchema.safeParse({
        id: "1",
        name: "Test Product",
        price: "invalid",
        categoryName: "Electronics",
        stockQuantity: 100,
        createdAt: "2024-01-01T00:00:00Z",
      });
      expect(result.success).toBe(false);
    });

    it("should reject invalid datetime format", () => {
      const result = DashboardModel.RecentProductSchema.safeParse({
        id: "1",
        name: "Test Product",
        price: 99.99,
        categoryName: "Electronics",
        stockQuantity: 100,
        createdAt: "not-a-date",
      });
      expect(result.success).toBe(false);
    });
  });
});
