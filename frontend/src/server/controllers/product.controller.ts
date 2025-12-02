"use server";

import { revalidatePath } from "next/cache";
import { ProductModel } from "../models/product.model";
import { ProductService } from "../services/product.service";
import { getValidAuthToken } from "./token.controller";

// For server components - token passed explicitly
export async function getAllProducts(
  token: string,
  query?: ProductModel.GetAllProductsQuery
): Promise<ProductModel.PaginatedProducts> {
  const service = new ProductService(token);
  return await service.getAll(query);
}

export async function getProductById(
  token: string,
  id: string
): Promise<ProductModel.Product> {
  const service = new ProductService(token);
  return await service.getById(id);
}

export async function searchProducts(
  token: string,
  query: ProductModel.SearchProductsQuery
): Promise<ProductModel.Product[]> {
  const service = new ProductService(token);
  return await service.search(query);
}

export async function getLowStockProducts(token: string): Promise<ProductModel.Product[]> {
  const service = new ProductService(token);
  return await service.getLowStock();
}

// For client components - token fetched internally
export async function createProduct(
  data: ProductModel.CreateProductDto
): Promise<ProductModel.Product> {
  const token = await getValidAuthToken();
  const service = new ProductService(token);
  const product = await service.create(data);
  revalidatePath("/products");
  return product;
}

export async function updateProduct(
  id: string,
  data: ProductModel.UpdateProductDto
): Promise<ProductModel.Product> {
  const token = await getValidAuthToken();
  const service = new ProductService(token);
  const product = await service.update(id, data);
  revalidatePath("/products");
  return product;
}

export async function deleteProduct(id: string): Promise<void> {
  const token = await getValidAuthToken();
  const service = new ProductService(token);
  await service.delete(id);
  revalidatePath("/products");
}
