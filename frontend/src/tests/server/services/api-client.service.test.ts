import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { ApiClient, ApiClientError } from "@/server/services/api-client.service";

describe("ApiClient", () => {
  let apiClient: ApiClient;
  const originalFetch = global.fetch;

  beforeEach(() => {
    apiClient = new ApiClient();
    vi.resetAllMocks();
  });

  afterEach(() => {
    global.fetch = originalFetch;
  });

  describe("ApiClientError", () => {
    it("should create error with statusCode and message", () => {
      const error = new ApiClientError(404, "Not Found");
      expect(error.statusCode).toBe(404);
      expect(error.message).toBe("Not Found");
      expect(error.name).toBe("ApiClientError");
    });

    it("should create error with errors object", () => {
      const errors = { field: ["error1", "error2"] };
      const error = new ApiClientError(400, "Validation Error", errors);
      expect(error.errors).toEqual(errors);
    });
  });

  describe("get", () => {
    it("should make GET request and return data", async () => {
      const mockData = { id: 1, name: "Test" };
      global.fetch = vi.fn().mockResolvedValue({
        ok: true,
        status: 200,
        json: () => Promise.resolve(mockData),
      });

      const result = await apiClient.get("/test");

      expect(result).toEqual(mockData);
      expect(global.fetch).toHaveBeenCalledWith(
        expect.stringContaining("/test"),
        expect.objectContaining({ method: "GET" })
      );
    });

    it("should include authorization header when token provided", async () => {
      global.fetch = vi.fn().mockResolvedValue({
        ok: true,
        status: 200,
        json: () => Promise.resolve({}),
      });

      await apiClient.get("/test", undefined, "test-token");

      expect(global.fetch).toHaveBeenCalledWith(
        expect.any(String),
        expect.objectContaining({
          headers: expect.objectContaining({
            Authorization: "Bearer test-token",
          }),
        })
      );
    });

    it("should throw ApiClientError on non-ok response", async () => {
      global.fetch = vi.fn().mockResolvedValue({
        ok: false,
        status: 404,
        statusText: "Not Found",
        json: () => Promise.resolve({ message: "Resource not found" }),
      });

      await expect(apiClient.get("/test")).rejects.toThrow(ApiClientError);
    });

    it("should handle json parse error on error response", async () => {
      global.fetch = vi.fn().mockResolvedValue({
        ok: false,
        status: 500,
        statusText: "Internal Server Error",
        json: () => Promise.reject(new Error("Invalid JSON")),
      });

      await expect(apiClient.get("/test")).rejects.toThrow(ApiClientError);
    });
  });

  describe("post", () => {
    it("should make POST request with body", async () => {
      const mockData = { id: 1 };
      const postData = { name: "Test" };
      global.fetch = vi.fn().mockResolvedValue({
        ok: true,
        status: 201,
        json: () => Promise.resolve(mockData),
      });

      const result = await apiClient.post("/test", postData);

      expect(result).toEqual(mockData);
      expect(global.fetch).toHaveBeenCalledWith(
        expect.stringContaining("/test"),
        expect.objectContaining({
          method: "POST",
          body: JSON.stringify(postData),
        })
      );
    });

    it("should make POST request without body", async () => {
      global.fetch = vi.fn().mockResolvedValue({
        ok: true,
        status: 200,
        json: () => Promise.resolve({}),
      });

      await apiClient.post("/test");

      expect(global.fetch).toHaveBeenCalledWith(
        expect.any(String),
        expect.objectContaining({
          method: "POST",
          body: undefined,
        })
      );
    });
  });

  describe("put", () => {
    it("should make PUT request with body", async () => {
      const mockData = { id: 1, name: "Updated" };
      const putData = { name: "Updated" };
      global.fetch = vi.fn().mockResolvedValue({
        ok: true,
        status: 200,
        json: () => Promise.resolve(mockData),
      });

      const result = await apiClient.put("/test/1", putData);

      expect(result).toEqual(mockData);
      expect(global.fetch).toHaveBeenCalledWith(
        expect.stringContaining("/test/1"),
        expect.objectContaining({
          method: "PUT",
          body: JSON.stringify(putData),
        })
      );
    });
  });

  describe("delete", () => {
    it("should make DELETE request", async () => {
      global.fetch = vi.fn().mockResolvedValue({
        ok: true,
        status: 204,
        json: () => Promise.resolve(undefined),
      });

      const result = await apiClient.delete("/test/1");

      expect(result).toBeUndefined();
      expect(global.fetch).toHaveBeenCalledWith(
        expect.stringContaining("/test/1"),
        expect.objectContaining({ method: "DELETE" })
      );
    });
  });

  describe("error handling", () => {
    it("should wrap network errors in ApiClientError", async () => {
      global.fetch = vi.fn().mockRejectedValue(new Error("Network error"));

      try {
        await apiClient.get("/test");
        expect.fail("Should have thrown");
      } catch (error) {
        expect(error).toBeInstanceOf(ApiClientError);
        expect((error as ApiClientError).statusCode).toBe(500);
        expect((error as ApiClientError).message).toBe("Network error");
      }
    });

    it("should handle non-Error throws", async () => {
      global.fetch = vi.fn().mockRejectedValue("string error");

      try {
        await apiClient.get("/test");
        expect.fail("Should have thrown");
      } catch (error) {
        expect(error).toBeInstanceOf(ApiClientError);
        expect((error as ApiClientError).message).toBe("Network error");
      }
    });

    it("should return undefined for 204 status", async () => {
      global.fetch = vi.fn().mockResolvedValue({
        ok: true,
        status: 204,
      });

      const result = await apiClient.delete("/test/1");
      expect(result).toBeUndefined();
    });
  });
});
