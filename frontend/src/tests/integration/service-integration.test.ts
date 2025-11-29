import { describe, it, expect, beforeAll, afterAll } from "vitest";
import request from "supertest";

const API_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000/api";
const API_TIMEOUT = 15000; // 15 segundos para operações CRUD

describe("Service Integration Tests - Real API Calls", () => {
  describe("CategoryService Integration", () => {
    let testCategoryId: string;

    it("should fetch all categories from real API", async () => {
      const response = await request(API_URL)
        .get("/categories")
        .expect(200);

      expect(Array.isArray(response.body)).toBe(true);
      response.body.forEach((cat: any) => {
        expect(cat).toHaveProperty("id");
        expect(cat).toHaveProperty("name");
        expect(cat).toHaveProperty("description");
      });
    }, API_TIMEOUT);

    it("should create category in real API", async () => {
      const response = await request(API_URL)
        .post("/categories")
        .send({
          name: `Integration Test ${Date.now()}`,
          description: "Created by integration test",
        })
        .expect(201);

      testCategoryId = response.body.id;

      expect(response.body).toHaveProperty("id");
      expect(response.body.name).toContain("Integration Test");
    }, API_TIMEOUT);

    it("should fetch category by ID from real API", async () => {
      const response = await request(API_URL)
        .get(`/categories/${testCategoryId}`)
        .expect(200);

      expect(response.body.id).toBe(testCategoryId);
    }, API_TIMEOUT);

    it("should update category in real API", async () => {
      const response = await request(API_URL)
        .put(`/categories/${testCategoryId}`)
        .send({
          name: "Updated Integration Test",
          description: "Updated by integration test",
        })
        .expect(200);

      expect(response.body.name).toBe("Updated Integration Test");
    }, API_TIMEOUT);

    it("should delete category from real API", async () => {
      const deleteResponse = await request(API_URL)
        .delete(`/categories/${testCategoryId}`);

      expect(deleteResponse.status).toBe(204);
    }, API_TIMEOUT);

    it("should return 404 for non-existent category ID", async () => {
      // Usar um ID que certamente não existe
      await request(API_URL)
        .get("/categories/000000000000000000000000")
        .expect(404);
    }, API_TIMEOUT);

    it("should return 404 when updating non-existent category", async () => {
      await request(API_URL)
        .put("/categories/non-existent-id")
        .send({
          name: "Test",
          description: "Test",
        })
        .expect(404);
    }, API_TIMEOUT);

    it("should return 400 for invalid category data", async () => {
      await request(API_URL)
        .post("/categories")
        .send({
          name: "",
          description: "",
        })
        .expect(400);
    }, API_TIMEOUT);
  })

  describe("ProductService Integration", () => {
    let testCategoryId: string;
    let testProductId: string;

    beforeAll(async () => {
      // Criar categoria para os testes de produto
      const response = await request(API_URL)
        .post("/categories")
        .send({
          name: `Product Test Category ${Date.now()}`,
          description: "For product integration tests",
        });

      testCategoryId = response.body.id;
    });

    it("should fetch all products from real API", async () => {
      const response = await request(API_URL)
        .get("/products?page=1&pageSize=10")
        .expect(200);

      expect(Array.isArray(response.body)).toBe(true);
    }, API_TIMEOUT);

    it("should create product in real API", async () => {
      const response = await request(API_URL)
        .post("/products")
        .send({
          name: `Test Product ${Date.now()}`,
          description: "Integration test product",
          price: 99.99,
          categoryId: testCategoryId,
          stockQuantity: 100,
        })
        .expect(201);

      testProductId = response.body.id;

      expect(response.body).toHaveProperty("id");
      expect(response.body.price).toBe(99.99);
    }, API_TIMEOUT);

    it("should search products in real API", async () => {
      const response = await request(API_URL)
        .get("/products/search?name=Test")
        .expect(200);

      expect(Array.isArray(response.body)).toBe(true);
    }, API_TIMEOUT);

    it("should fetch low stock products from real API", async () => {
      const response = await request(API_URL)
        .get("/products/low-stock")
        .expect(200);

      expect(Array.isArray(response.body)).toBe(true);
    }, API_TIMEOUT);

    it("should update product in real API", async () => {
      const response = await request(API_URL)
        .put(`/products/${testProductId}`)
        .send({
          name: "Updated Test Product",
          description: "Updated description",
          price: 149.99,
          categoryId: testCategoryId,
          stockQuantity: 50,
        })
        .expect(200);

      expect(response.body.price).toBe(149.99);
    }, API_TIMEOUT);

    it("should delete product from real API", async () => {
      await request(API_URL)
        .delete(`/products/${testProductId}`)
        .expect(204);

      await request(API_URL)
        .get(`/products/${testProductId}`)
        .expect(404);
    }, API_TIMEOUT);

    it("should return error when updating non-existent product", async () => {
      const response = await request(API_URL)
        .put("/products/non-existent-id")
        .send({
          name: "Test Product Name",
          description: "Test description for product",
          price: 10,
          categoryId: testCategoryId,
          stockQuantity: 5,
        });

      // API pode retornar 400 (validação) ou 404 (não encontrado)
      expect([400, 404]).toContain(response.status);
    }, API_TIMEOUT);

    it("should return 400 for invalid product data - empty name", async () => {
      await request(API_URL)
        .post("/products")
        .send({
          name: "",
          description: "Test",
          price: 10,
          categoryId: testCategoryId,
          stockQuantity: 5,
        })
        .expect(400);
    }, API_TIMEOUT);

    it("should return 400 for invalid product data - negative price", async () => {
      await request(API_URL)
        .post("/products")
        .send({
          name: "Test",
          description: "Test",
          price: -10,
          categoryId: testCategoryId,
          stockQuantity: 5,
        })
        .expect(400);
    }, API_TIMEOUT);

    it("should return 400 for invalid product data - negative stock", async () => {
      await request(API_URL)
        .post("/products")
        .send({
          name: "Test",
          description: "Test",
          price: 10,
          categoryId: testCategoryId,
          stockQuantity: -5,
        })
        .expect(400);
    }, API_TIMEOUT);

    afterAll(async () => {
      await request(API_URL)
        .delete(`/categories/${testCategoryId}`)
        .expect(204);
    });
  });

  describe("DashboardService Integration", () => {
    it("should fetch dashboard data from real API", async () => {
      const response = await request(API_URL)
        .get("/dashboard")
        .expect(200);

      expect(response.body).toHaveProperty("totalProducts");
      expect(response.body).toHaveProperty("totalStockValue");
      // API retorna lowStockCount em vez de lowStockProducts
      expect(response.body).toHaveProperty("lowStockCount");
      expect(response.body).toHaveProperty("productsByCategory");

      expect(typeof response.body.totalProducts).toBe("number");
      expect(typeof response.body.totalStockValue).toBe("number");
      expect(typeof response.body.lowStockCount).toBe("number");
    }, API_TIMEOUT);

    it("should respond within acceptable time", async () => {
      const start = performance.now();

      await request(API_URL)
        .get("/dashboard")
        .expect(200);

      const duration = performance.now() - start;

      expect(duration).toBeLessThan(2000);
    }, API_TIMEOUT);
  });

  describe("Performance Tests - Multiple Requests", () => {
    it("should handle 10 concurrent category requests within 3 seconds", async () => {
      const start = performance.now();

      const promises = Array.from({ length: 10 }, () =>
        request(API_URL).get("/categories").expect(200)
      );

      await Promise.all(promises);

      const duration = performance.now() - start;
      expect(duration).toBeLessThan(3000);
    }, API_TIMEOUT);

    it("should handle 10 concurrent product requests within 3 seconds", async () => {
      const start = performance.now();

      const promises = Array.from({ length: 10 }, () =>
        request(API_URL).get("/products?page=1&pageSize=10").expect(200)
      );

      await Promise.all(promises);

      const duration = performance.now() - start;
      expect(duration).toBeLessThan(3000);
    }, API_TIMEOUT);
  });
});
