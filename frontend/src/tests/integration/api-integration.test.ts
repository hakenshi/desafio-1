import { describe, it, expect } from "vitest";
import request from "supertest";

const API_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000/api";
const API_TIMEOUT = 15000; // 15 segundos para operações CRUD

describe("API Integration Tests - Real Calls", () => {
  describe("API Health Check", () => {
    it("should connect to API within 2 seconds", async () => {
      const start = performance.now();
      
      const response = await request(API_URL)
        .get("/categories")
        .expect(200);
      
      const duration = performance.now() - start;
      
      expect(duration).toBeLessThan(2000);
      expect(Array.isArray(response.body)).toBe(true);
    }, API_TIMEOUT);
  });

  describe("Categories Endpoint", () => {
    it("should fetch all categories", async () => {
      const response = await request(API_URL)
        .get("/categories")
        .expect(200);
      
      expect(Array.isArray(response.body)).toBe(true);
    }, API_TIMEOUT);

    it("should respond within 500ms", async () => {
      const start = performance.now();
      
      await request(API_URL)
        .get("/categories")
        .expect(200);
      
      const duration = performance.now() - start;
      expect(duration).toBeLessThan(500);
    }, API_TIMEOUT);
  });

  describe("Products Endpoint", () => {
    it("should fetch all products", async () => {
      const response = await request(API_URL)
        .get("/products?page=1&pageSize=10")
        .expect(200);
      
      expect(Array.isArray(response.body)).toBe(true);
    }, API_TIMEOUT);

    it("should respond within 500ms", async () => {
      const start = performance.now();
      
      await request(API_URL)
        .get("/products?page=1&pageSize=10")
        .expect(200);
      
      const duration = performance.now() - start;
      expect(duration).toBeLessThan(500);
    }, API_TIMEOUT);

    it("should search products by name", async () => {
      const response = await request(API_URL)
        .get("/products/search?name=Notebook")
        .expect(200);
      
      expect(Array.isArray(response.body)).toBe(true);
    }, API_TIMEOUT);

    it("should fetch low stock products", async () => {
      const response = await request(API_URL)
        .get("/products/low-stock")
        .expect(200);
      
      expect(Array.isArray(response.body)).toBe(true);
    }, API_TIMEOUT);
  });

  describe("Dashboard Endpoint", () => {
    it("should fetch dashboard data", async () => {
      const response = await request(API_URL)
        .get("/dashboard")
        .expect(200);
      
      expect(response.body).toBeDefined();
      expect(response.body).toHaveProperty("totalProducts");
      expect(response.body).toHaveProperty("totalStockValue");
      // API retorna lowStockCount em vez de lowStockProducts
      expect(response.body).toHaveProperty("lowStockCount");
      expect(response.body).toHaveProperty("productsByCategory");
    }, API_TIMEOUT);

    it("should respond within 1 second", async () => {
      const start = performance.now();
      
      await request(API_URL)
        .get("/dashboard")
        .expect(200);
      
      const duration = performance.now() - start;
      expect(duration).toBeLessThan(1000);
    }, API_TIMEOUT);
  });

  describe("CRUD Operations Performance", () => {
    let createdCategoryId: string;
    let createdProductId: string;

    it("should create category within 1 second", async () => {
      const start = performance.now();
      
      const response = await request(API_URL)
        .post("/categories")
        .send({
          name: `Test Category ${Date.now()}`,
          description: "Integration test category",
        })
        .expect(201);
      
      const duration = performance.now() - start;
      createdCategoryId = response.body.id;
      
      expect(duration).toBeLessThan(1000);
      expect(response.body).toHaveProperty("id");
    }, API_TIMEOUT);

    it("should create product within 1 second", async () => {
      const start = performance.now();
      
      const response = await request(API_URL)
        .post("/products")
        .send({
          name: `Test Product ${Date.now()}`,
          description: "Integration test product",
          price: 99.99,
          categoryId: createdCategoryId,
          stockQuantity: 100,
        })
        .expect(201);
      
      const duration = performance.now() - start;
      createdProductId = response.body.id;
      
      expect(duration).toBeLessThan(1000);
      expect(response.body).toHaveProperty("id");
    }, API_TIMEOUT);

    it("should update product within 1 second", async () => {
      const start = performance.now();
      
      const response = await request(API_URL)
        .put(`/products/${createdProductId}`)
        .send({
          name: "Updated Product",
          description: "Updated description",
          price: 149.99,
          categoryId: createdCategoryId,
          stockQuantity: 50,
        })
        .expect(200);
      
      const duration = performance.now() - start;
      
      expect(duration).toBeLessThan(1000);
      expect(response.body.name).toBe("Updated Product");
    }, API_TIMEOUT);

    it("should delete product within 10 seconds", async () => {
      const start = performance.now();
      
      await request(API_URL)
        .delete(`/products/${createdProductId}`)
        .expect(204);
      
      const duration = performance.now() - start;
      
      expect(duration).toBeLessThan(10000);
    }, API_TIMEOUT);

    it("should delete category within 10 seconds", async () => {
      const start = performance.now();
      
      await request(API_URL)
        .delete(`/categories/${createdCategoryId}`)
        .expect(204);
      
      const duration = performance.now() - start;
      
      expect(duration).toBeLessThan(10000);
    }, API_TIMEOUT);
  });
});
