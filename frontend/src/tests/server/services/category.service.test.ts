import { describe, it, expect, vi, beforeEach } from "vitest";
import { CategoryService } from "@/server/services/category.service";
import { apiClient } from "@/server/services/api-client.service";

vi.mock("@/server/services/api-client.service", () => ({
  apiClient: {
    get: vi.fn(),
    post: vi.fn(),
    put: vi.fn(),
    delete: vi.fn(),
  },
}));

describe("CategoryService", () => {
  let service: CategoryService;
  const mockToken = "test-token";

  beforeEach(() => {
    service = new CategoryService(mockToken);
    vi.resetAllMocks();
  });

  describe("getAll", () => {
    it("should fetch all categories with default pagination", async () => {
      const mockResponse = {
        items: [
          { id: "1", name: "Category 1", description: "Desc 1", createdAt: "2024-01-01T00:00:00Z", updatedAt: "2024-01-01T00:00:00Z" },
        ],
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
        expect.stringContaining("/categories"),
        undefined,
        mockToken
      );
    });

    it("should fetch categories with custom pagination", async () => {
      const mockResponse = {
        items: [],
        page: 2,
        pageSize: 5,
        totalCount: 10,
        totalPages: 2,
        hasPreviousPage: true,
        hasNextPage: false,
      };

      vi.mocked(apiClient.get).mockResolvedValue(mockResponse);

      const result = await service.getAll({ page: 2, pageSize: 5 });

      expect(result.page).toBe(2);
      expect(result.pageSize).toBe(5);
      expect(apiClient.get).toHaveBeenCalledWith(
        expect.stringContaining("page=2"),
        undefined,
        mockToken
      );
    });
  });

  describe("getById", () => {
    it("should fetch category by id", async () => {
      const mockCategory = {
        id: "1",
        name: "Test Category",
        description: "Test Description",
        createdAt: "2024-01-01T00:00:00Z",
        updatedAt: "2024-01-01T00:00:00Z",
      };

      vi.mocked(apiClient.get).mockResolvedValue(mockCategory);

      const result = await service.getById("1");

      expect(result.id).toBe("1");
      expect(result.name).toBe("Test Category");
      expect(apiClient.get).toHaveBeenCalledWith(
        "/categories/1",
        undefined,
        mockToken
      );
    });
  });

  describe("create", () => {
    it("should create a new category", async () => {
      const createData = { name: "New Category", description: "New Description" };
      const mockResponse = {
        id: "1",
        ...createData,
        createdAt: "2024-01-01T00:00:00Z",
        updatedAt: "2024-01-01T00:00:00Z",
      };

      vi.mocked(apiClient.post).mockResolvedValue(mockResponse);

      const result = await service.create(createData);

      expect(result.id).toBe("1");
      expect(result.name).toBe("New Category");
      expect(apiClient.post).toHaveBeenCalledWith(
        "/categories",
        createData,
        undefined,
        mockToken
      );
    });
  });

  describe("update", () => {
    it("should update an existing category", async () => {
      const updateData = { name: "Updated Category", description: "Updated Description" };
      const mockResponse = {
        id: "1",
        ...updateData,
        createdAt: "2024-01-01T00:00:00Z",
        updatedAt: "2024-01-02T00:00:00Z",
      };

      vi.mocked(apiClient.put).mockResolvedValue(mockResponse);

      const result = await service.update("1", updateData);

      expect(result.name).toBe("Updated Category");
      expect(apiClient.put).toHaveBeenCalledWith(
        "/categories/1",
        updateData,
        undefined,
        mockToken
      );
    });
  });

  describe("delete", () => {
    it("should delete a category", async () => {
      vi.mocked(apiClient.delete).mockResolvedValue(undefined);

      await service.delete("1");

      expect(apiClient.delete).toHaveBeenCalledWith(
        "/categories/1",
        undefined,
        mockToken
      );
    });
  });
});
