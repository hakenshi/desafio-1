"use server";

import { ProductModel } from "../models/product.model";
import { ProductService } from "../services/product.service";
import { cacheTag, cacheLife, revalidateTag } from "next/cache";
import { cookies } from "next/headers";

const CACHE_TAGS = {
  products: "products",
  product: (id: string) => `product-${id}`,
  lowStock: "products-low-stock",
  search: "products-search",
};

async function getAuthToken(): Promise<string | undefined> {
  const cookieStore = await cookies();
  return cookieStore.get("authToken")?.value;
}

async function getService(): Promise<ProductService> {
  const token = await getAuthToken();
  return new ProductService(token);
}

export async function getAllProducts(query?: ProductModel.GetAllProductsQuery): Promise<ProductModel.Product[]> {
  const service = await getService();
  return await service.getAll(query);
}

export async function getProductById(id: string): Promise<ProductModel.Product> {
  const service = await getService();
  return await service.getById(id);
}

export async function searchProducts(query: ProductModel.SearchProductsQuery): Promise<ProductModel.Product[]> {
  const service = await getService();
  return await service.search(query);
}

export async function getLowStockProducts(): Promise<ProductModel.Product[]> {
  const service = await getService();
  return await service.getLowStock();
}

export async function createProduct(data: ProductModel.CreateProductDto): Promise<ProductModel.Product> {
  const service = await getService();
  const product = await service.create(data);
  revalidateTag(CACHE_TAGS.products, "max");
  revalidateTag(CACHE_TAGS.lowStock, "max");
  revalidateTag(CACHE_TAGS.search, "max");
  return product;
}

export async function updateProduct(id: string, data: ProductModel.UpdateProductDto): Promise<ProductModel.Product> {
  const service = await getService();
  const product = await service.update(id, data);
  revalidateTag(CACHE_TAGS.product(id), "max");
  revalidateTag(CACHE_TAGS.products, "max");
  revalidateTag(CACHE_TAGS.lowStock, "max");
  revalidateTag(CACHE_TAGS.search, "max");
  return product;
}

export async function deleteProduct(id: string): Promise<void> {
  const service = await getService();
  await service.delete(id);
  revalidateTag(CACHE_TAGS.product(id), "max");
  revalidateTag(CACHE_TAGS.products, "max");
  revalidateTag(CACHE_TAGS.lowStock, "max");
  revalidateTag(CACHE_TAGS.search, "max");
}
