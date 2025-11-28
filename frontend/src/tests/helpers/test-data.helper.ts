import { CategoryModel } from "@/server/models/category.model";
import { ProductModel } from "@/server/models/product.model";
import { DashboardModel } from "@/server/models/dashboard.model";

export class TestDataHelper {
  static createMockCategory(
    overrides?: Partial<CategoryModel.Category>
  ): CategoryModel.Category {
    return {
      id: "cat-123",
      name: "Electronics",
      description: "Electronic devices and accessories",
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
      ...overrides,
    };
  }

  static createMockProduct(
    overrides?: Partial<ProductModel.Product>
  ): ProductModel.Product {
    return {
      id: "prod-123",
      name: "Laptop",
      description: "High-performance laptop",
      price: 1299.99,
      categoryId: "cat-123",
      stockQuantity: 50,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
      ...overrides,
    };
  }

  static createMockDashboard(
    overrides?: Partial<DashboardModel.Dashboard>
  ): DashboardModel.Dashboard {
    return {
      totalProducts: 100,
      totalStockValue: 50000,
      lowStockProducts: [
        TestDataHelper.createMockProduct({ stockQuantity: 5 }),
      ],
      productsByCategory: {
        Electronics: 50,
        Clothing: 30,
        Food: 20,
      },
      ...overrides,
    };
  }
}
