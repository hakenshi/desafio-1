import { ProductModel } from "../models/product.model";
import { apiClient } from "./api-client.service";
import { revalidateTag, cacheTag, cacheLife } from "next/cache";
import { z } from "zod";

export abstract class ProductService {
  static async getAll(query?: ProductModel.GetAllProductsQuery): Promise<ProductModel.Product[]> {
    const validatedQuery = ProductModel.GetAllProductsQuerySchema.parse(query || {});
    const params = new URLSearchParams({
      page: validatedQuery.page.toString(),
      pageSize: validatedQuery.pageSize.toString(),
    });

    const response = await apiClient.get<ProductModel.Product[]>(`/products?${params.toString()}`);
    return z.array(ProductModel.ProductSchema).parse(response);
  }

  static async getById(id: string): Promise<ProductModel.Product> {
    const response = await apiClient.get<ProductModel.Product>(`/products/${id}`);
    return ProductModel.ProductSchema.parse(response);
  }

  static async search(query: ProductModel.SearchProductsQuery): Promise<ProductModel.Product[]> {
    const validatedQuery = ProductModel.SearchProductsQuerySchema.parse(query);
    const params = new URLSearchParams();

    if (validatedQuery.searchTerm) {
      params.append("searchTerm", validatedQuery.searchTerm);
    }
    if (validatedQuery.categoryId) {
      params.append("categoryId", validatedQuery.categoryId);
    }
    params.append("page", validatedQuery.page.toString());
    params.append("pageSize", validatedQuery.pageSize.toString());

    const response = await apiClient.get<ProductModel.Product[]>(`/products/search?${params.toString()}`);
    return z.array(ProductModel.ProductSchema).parse(response);
  }

  static async getLowStock(): Promise<ProductModel.Product[]> {
    const response = await apiClient.get<ProductModel.Product[]>("/products/low-stock");
    return z.array(ProductModel.ProductSchema).parse(response);
  }

  static async create(data: ProductModel.CreateProductDto): Promise<ProductModel.Product> {
    const validatedData = ProductModel.CreateProductSchema.parse(data);
    const response = await apiClient.post<ProductModel.Product>("/products", validatedData);
    return ProductModel.ProductSchema.parse(response);
  }

  static async update(id: string, data: ProductModel.UpdateProductDto): Promise<ProductModel.Product> {
    const validatedData = ProductModel.UpdateProductSchema.parse(data);
    const response = await apiClient.put<ProductModel.Product>(`/products/${id}`, validatedData);
    return ProductModel.ProductSchema.parse(response);
  }

  static async delete(id: string): Promise<void> {
    await apiClient.delete<void>(`/products/${id}`);
  }
}
