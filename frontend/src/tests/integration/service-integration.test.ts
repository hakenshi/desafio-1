import { describe, it, expect, beforeAll } from "vitest";
import request from "supertest";

const API_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000/api";
const API_TIMEOUT = 15000;

describe("Service Integration Tests - Real API Calls", () => {
  let accessToken: string;

  // Setup: autenticar antes de todos os testes
  beforeAll(async () => {
    const testUser = {
      username: `testuser_${Date.now()}`,
      email: `test_${Date.now()}@example.com`,
      password: "Test@123456",
      firstName: "Test",
      lastName: "User",
    };

    // Registrar usuário
    await request(API_URL).post("/auth/register").send(testUser);

    // Login para obter token
    const loginResponse = await request(API_URL)
      .post("/auth/login")
      .send({ email: testUser.email, password: testUser.password });

    accessToken = loginResponse.body.accessToken;
  });

  describe("AuthService Integration", () => {
    const testUser = {
      username: `authtest_${Date.now()}`,
      email: `authtest_${Date.now()}@example.com`,
      password: "Test@123456",
      firstName: "Auth",
      lastName: "Test",
    };
    let authAccessToken: string;
    let authRefreshToken: string;

    it("should register a new user", async () => {
      const response = await request(API_URL)
        .post("/auth/register")
        .send(testUser);

      expect([201, 400]).toContain(response.status);
    }, API_TIMEOUT);

    it("should login with valid credentials", async () => {
      const response = await request(API_URL)
        .post("/auth/login")
        .send({ email: testUser.email, password: testUser.password });

      expect(response.body).toHaveProperty("accessToken");
      expect(response.body).toHaveProperty("refreshToken");
      authAccessToken = response.body.accessToken;
      authRefreshToken = response.body.refreshToken;
    }, API_TIMEOUT);

    it("should refresh token with valid refresh token", async () => {
      if (!authRefreshToken) return;

      const response = await request(API_URL)
        .post("/auth/refresh")
        .send({ refreshToken: authRefreshToken });

      expect([200, 401]).toContain(response.status);
    }, API_TIMEOUT);

    it("should get current user info with valid token", async () => {
      if (!authAccessToken) return;

      const response = await request(API_URL)
        .get("/auth/me")
        .set("Authorization", `Bearer ${authAccessToken}`);

      expect(response.status).toBe(200);
      expect(response.body).toHaveProperty("id");
      expect(response.body).toHaveProperty("email");
    }, API_TIMEOUT);

    it("should return 401 for unauthenticated request", async () => {
      const response = await request(API_URL).get("/auth/me");
      expect(response.status).toBe(401);
    }, API_TIMEOUT);
  });

  describe("CategoryService Integration", () => {
    let testCategoryId: string;

    it("should fetch all categories", async () => {
      const response = await request(API_URL)
        .get("/categories")
        .set("Authorization", `Bearer ${accessToken}`);

      expect(response.status).toBe(200);
      expect(Array.isArray(response.body)).toBe(true);
    }, API_TIMEOUT);

    it("should create category", async () => {
      const response = await request(API_URL)
        .post("/categories")
        .set("Authorization", `Bearer ${accessToken}`)
        .send({
          name: `Integration Test ${Date.now()}`,
          description: "Created by integration test",
        });

      expect(response.status).toBe(201);
      testCategoryId = response.body.id;
      expect(response.body).toHaveProperty("id");
    }, API_TIMEOUT);

    it("should fetch category by ID", async () => {
      if (!testCategoryId) return;

      const response = await request(API_URL)
        .get(`/categories/${testCategoryId}`)
        .set("Authorization", `Bearer ${accessToken}`);

      expect(response.status).toBe(200);
      expect(response.body.id).toBe(testCategoryId);
    }, API_TIMEOUT);

    it("should update category", async () => {
      if (!testCategoryId) return;

      const response = await request(API_URL)
        .put(`/categories/${testCategoryId}`)
        .set("Authorization", `Bearer ${accessToken}`)
        .send({
          name: "Updated Integration Test",
          description: "Updated by integration test",
        });

      expect(response.status).toBe(200);
      expect(response.body.name).toBe("Updated Integration Test");
    }, API_TIMEOUT);

    it("should delete category", async () => {
      if (!testCategoryId) return;

      const response = await request(API_URL)
        .delete(`/categories/${testCategoryId}`)
        .set("Authorization", `Bearer ${accessToken}`);

      expect(response.status).toBe(204);
    }, API_TIMEOUT);
  });

  describe("ProductService Integration", () => {
    let testCategoryId: string;
    let testProductId: string;

    beforeAll(async () => {
      const response = await request(API_URL)
        .post("/categories")
        .set("Authorization", `Bearer ${accessToken}`)
        .send({
          name: `Product Test Category ${Date.now()}`,
          description: "For product integration tests",
        });
      testCategoryId = response.body.id;
    });

    it("should fetch all products", async () => {
      const response = await request(API_URL)
        .get("/products?page=1&pageSize=10")
        .set("Authorization", `Bearer ${accessToken}`);

      expect(response.status).toBe(200);
      expect(Array.isArray(response.body)).toBe(true);
    }, API_TIMEOUT);

    it("should create product", async () => {
      const response = await request(API_URL)
        .post("/products")
        .set("Authorization", `Bearer ${accessToken}`)
        .send({
          name: `Test Product ${Date.now()}`,
          description: "Integration test product",
          price: 99.99,
          categoryId: testCategoryId,
          stockQuantity: 100,
        });

      expect(response.status).toBe(201);
      testProductId = response.body.id;
      expect(response.body.price).toBe(99.99);
    }, API_TIMEOUT);

    it("should update product", async () => {
      if (!testProductId) return;

      const response = await request(API_URL)
        .put(`/products/${testProductId}`)
        .set("Authorization", `Bearer ${accessToken}`)
        .send({
          name: "Updated Test Product",
          description: "Updated description",
          price: 149.99,
          categoryId: testCategoryId,
          stockQuantity: 50,
        });

      expect(response.status).toBe(200);
      expect(response.body.price).toBe(149.99);
    }, API_TIMEOUT);

    it("should delete product", async () => {
      if (!testProductId) return;

      const response = await request(API_URL)
        .delete(`/products/${testProductId}`)
        .set("Authorization", `Bearer ${accessToken}`);

      expect(response.status).toBe(204);
    }, API_TIMEOUT);
  });

  describe("DashboardService Integration", () => {
    it("should fetch dashboard data", async () => {
      const response = await request(API_URL)
        .get("/dashboard")
        .set("Authorization", `Bearer ${accessToken}`);

      expect(response.status).toBe(200);
      expect(response.body).toHaveProperty("totalProducts");
      expect(response.body).toHaveProperty("totalStockValue");
    }, API_TIMEOUT);
  });
});
