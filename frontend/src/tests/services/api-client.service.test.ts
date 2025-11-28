import { describe, it, expect, beforeEach, vi } from "vitest";
import { ApiClient, ApiClientError } from "@/server/services/api-client.service";
import { ApiMockHelper } from "../helpers/api-mock.helper";

describe("ApiClient Service", () => {
  let apiClient: ApiClient;

  beforeEach(() => {
    apiClient = new ApiClient();
    vi.clearAllMocks();
  });

  describe("Performance Tests", () => {
    it("should respond within 100ms for successful GET requests", async () => {
      const mockData = { id: "1", name: "Test" };
      global.fetch = ApiMockHelper.mockSuccessResponse(mockData);

      const responseTime = await ApiMockHelper.measureResponseTime(() =>
        apiClient.get("/test")
      );

      expect(responseTime).toBeLessThan(100);
    });

    it("should respond within 100ms for successful POST requests", async () => {
      const mockData = { id: "1", name: "Test" };
      global.fetch = ApiMockHelper.mockSuccessResponse(mockData);

      const responseTime = await ApiMockHelper.measureResponseTime(() =>
        apiClient.post("/test", { name: "Test" })
      );

      expect(responseTime).toBeLessThan(100);
    });

    it("should handle errors within 100ms", async () => {
      global.fetch = ApiMockHelper.mockErrorResponse(404, "Not found");

      const responseTime = await ApiMockHelper.measureResponseTime(() =>
        apiClient.get("/test").catch(() => {})
      );

      expect(responseTime).toBeLessThan(100);
    });
  });

  describe("GET Requests", () => {
    it("should make successful GET request", async () => {
      const mockData = { id: "1", name: "Test" };
      global.fetch = ApiMockHelper.mockSuccessResponse(mockData);

      const result = await apiClient.get("/test");

      expect(result).toEqual(mockData);
      expect(global.fetch).toHaveBeenCalledWith(
        "http://localhost:5000/api/test",
        expect.objectContaining({
          method: "GET",
          headers: expect.objectContaining({
            "Content-Type": "application/json",
          }),
        })
      );
    });

    it("should throw ApiClientError on failed GET request", async () => {
      global.fetch = ApiMockHelper.mockErrorResponse(404, "Not found");

      await expect(apiClient.get("/test")).rejects.toThrow(ApiClientError);
      await expect(apiClient.get("/test")).rejects.toMatchObject({
        statusCode: 404,
        message: "Not found",
      });
    });
  });

  describe("POST Requests", () => {
    it("should make successful POST request with data", async () => {
      const mockData = { id: "1", name: "Test" };
      const postData = { name: "Test" };
      global.fetch = ApiMockHelper.mockSuccessResponse(mockData);

      const result = await apiClient.post("/test", postData);

      expect(result).toEqual(mockData);
      expect(global.fetch).toHaveBeenCalledWith(
        "http://localhost:5000/api/test",
        expect.objectContaining({
          method: "POST",
          body: JSON.stringify(postData),
        })
      );
    });

    it("should throw ApiClientError with validation errors", async () => {
      const errors = { name: ["Name is required"] };
      global.fetch = ApiMockHelper.mockErrorResponse(400, "Validation failed", errors);

      await expect(apiClient.post("/test", {})).rejects.toMatchObject({
        statusCode: 400,
        message: "Validation failed",
        errors,
      });
    });
  });

  describe("PUT Requests", () => {
    it("should make successful PUT request", async () => {
      const mockData = { id: "1", name: "Updated" };
      const putData = { name: "Updated" };
      global.fetch = ApiMockHelper.mockSuccessResponse(mockData);

      const result = await apiClient.put("/test/1", putData);

      expect(result).toEqual(mockData);
      expect(global.fetch).toHaveBeenCalledWith(
        "http://localhost:5000/api/test/1",
        expect.objectContaining({
          method: "PUT",
          body: JSON.stringify(putData),
        })
      );
    });
  });

  describe("DELETE Requests", () => {
    it("should make successful DELETE request with 204 response", async () => {
      global.fetch = ApiMockHelper.mockNoContentResponse();

      const result = await apiClient.delete("/test/1");

      expect(result).toBeUndefined();
      expect(global.fetch).toHaveBeenCalledWith(
        "http://localhost:5000/api/test/1",
        expect.objectContaining({
          method: "DELETE",
        })
      );
    });
  });

  describe("Error Handling", () => {
    it("should handle network errors", async () => {
      global.fetch = ApiMockHelper.mockNetworkError();

      await expect(apiClient.get("/test")).rejects.toMatchObject({
        statusCode: 500,
        message: "Network error",
      });
    });

    it("should handle 500 server errors", async () => {
      global.fetch = ApiMockHelper.mockErrorResponse(500, "Internal server error");

      await expect(apiClient.get("/test")).rejects.toMatchObject({
        statusCode: 500,
        message: "Internal server error",
      });
    });
  });
});
