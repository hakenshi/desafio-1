import { describe, it, expect, vi, beforeEach } from "vitest";
import { ProductService } from "../../server/services/product.service";
import { apiClient } from "../../server/services/api-client.service";

vi.mock("../../server/services/api-client.service", () => ({
  apiClient: {
    get: vi.fn(),
    post: vi.fn(),
    put: vi.fn(),
    delete: vi.fn(),
  },
}));

describe("ProductService", () => {
  let service: ProductService;
  const mockToken = "test-token";

  const mockProduct = {
    id: "1",
    sku: "PRD000001",
    name: "Test Product",
    description: "Test Description",
    price: 99.99,
    categoryId: "cat-1",
    categoryName: "Test Category",
    stockQuantity: 100,
    isLowStock: false,
    createdAt: "2024-01-01T00:00:00Z",
    updatedAt: "2024-01-01T00:00:00Z",
  };

  beforeEach(() => {
    service = new ProductService(mockToken);
    vi.resetAllMocks();
  });

  describe("getAll", () => {
    it("should fetch all products with default pagination", async () => {
      const mockResponse = {
        items: [mockProduct],
        page: 1,
        pageSize: 10,
        totalCount: 1,
        totalPages: 1,
        hasPreviousPage: false,
        hasNextPage: false,
      };

      vi.mocked(apiClient.get).mockResolvedValue(mockResponse);

      const result = await service.getAll();

      expect(result.items).toHaveLength(1);
      expect(result.page).toBe(1);
      expect(apiClient.get).toHaveBeenCalledWith(
        expect.stringContaining("/products"),
        undefined,
        mockToken
      );
    });

    it("should fetch products with category filter", async () => {
      const mockResponse = {
        items: [mockProduct],
        page: 1,
        pageSize: 10,
        totalCount: 1,
        totalPages: 1,
        hasPreviousPage: false,
        hasNextPage: false,
      };

      vi.mocked(apiClient.get).mockResolvedValue(mockResponse);

      const result = await service.getAll({ page: 1, pageSize: 10, categoryId: "cat-1" });

      expect(result.items).toHaveLength(1);
      expect(apiClient.get).toHaveBeenCalledWith(
        expect.stringContaining("categoryId=cat-1"),
        undefined,
        mockToken
      );
    });
  });

  describe("getById", () => {
    it("should fetch product by id", async () => {
      vi.mocked(apiClient.get).mockResolvedValue(mockProduct);

      const result = await service.getById("1");

      expect(result.id).toBe("1");
      expect(result.name).toBe("Test Product");
      expect(apiClient.get).toHaveBeenCalledWith(
        "/products/1",
        undefined,
        mockToken
      );
    });
  });

  describe("search", () => {
    it("should search products by term", async () => {
      vi.mocked(apiClient.get).mockResolvedValue([mockProduct]);

      const result = await service.search({ searchTerm: "Test", page: 1, pageSize: 10 });

      expect(result).toHaveLength(1);
      expect(apiClient.get).toHaveBeenCalledWith(
        expect.stringContaining("searchTerm=Test"),
        undefined,
        mockToken
      );
    });

    it("should search products with category filter", async () => {
      vi.mocked(apiClient.get).mockResolvedValue([mockProduct]);

      const result = await service.search({ searchTerm: "Test", categoryId: "cat-1", page: 1, pageSize: 10 });

      expect(result).toHaveLength(1);
      expect(apiClient.get).toHaveBeenCalledWith(
        expect.stringContaining("categoryId=cat-1"),
        undefined,
        mockToken
      );
    });
  });

  describe("getLowStock", () => {
    it("should fetch low stock products", async () => {
      const lowStockProduct = { ...mockProduct, stockQuantity: 5, isLowStock: true };
      vi.mocked(apiClient.get).mockResolvedValue([lowStockProduct]);

      const result = await service.getLowStock();

      expect(result).toHaveLength(1);
      expect(result[0].isLowStock).toBe(true);
      expect(apiClient.get).toHaveBeenCalledWith(
        "/products/low-stock",
        undefined,
        mockToken
      );
    });
  });

  describe("create", () => {
    it("should create a new product", async () => {
      const createData = {
        name: "New Product",
        description: "New Description",
        price: 49.99,
        categoryId: "cat-1",
        stockQuantity: 50,
      };

      vi.mocked(apiClient.post).mockResolvedValue({ ...mockProduct, ...createData });

      const result = await service.create(createData);

      expect(result.name).toBe("New Product");
      expect(apiClient.post).toHaveBeenCalledWith(
        "/products",
        createData,
        undefined,
        mockToken
      );
    });
  });

  describe("update", () => {
    it("should update an existing product", async () => {
      const updateData = {
        name: "Updated Product",
        description: "Updated Description",
        price: 149.99,
        categoryId: "cat-1",
        stockQuantity: 75,
      };

      vi.mocked(apiClient.put).mockResolvedValue({ ...mockProduct, ...updateData });

      const result = await service.update("1", updateData);

      expect(result.name).toBe("Updated Product");
      expect(apiClient.put).toHaveBeenCalledWith(
        "/products/1",
        updateData,
        undefined,
        mockToken
      );
    });
  });

  describe("delete", () => {
    it("should delete a product", async () => {
      vi.mocked(apiClient.delete).mockResolvedValue(undefined);

      await service.delete("1");

      expect(apiClient.delete).toHaveBeenCalledWith(
        "/products/1",
        undefined,
        mockToken
      );
    });
  });
});
