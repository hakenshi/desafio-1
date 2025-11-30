"use server";

import { ProductModel } from "../models/product.model";
import { ProductService } from "../services/product.service";
import { cacheTag, cacheLife, revalidateTag } from "next/cache";

const CACHE_TAGS = {
  products: "products",
  product: (id: string) => `product-${id}`,
  lowStock: "products-low-stock",
  search: "products-search",
};

export async function getAllProducts(query?: ProductModel.GetAllProductsQuery): Promise<ProductModel.Product[]> {
  "use cache";
  cacheTag(CACHE_TAGS.products);
  cacheLife("minutes");
  
  return await ProductService.getAll(query);
}

export async function getProductById(id: string): Promise<ProductModel.Product> {
  "use cache";
  cacheTag(CACHE_TAGS.product(id));
  cacheLife("hours");
  
  return await ProductService.getById(id);
}

export async function searchProducts(query: ProductModel.SearchProductsQuery): Promise<ProductModel.Product[]> {
  "use cache";
  cacheTag(CACHE_TAGS.search);
  cacheLife("minutes");
  
  return await ProductService.search(query);
}

export async function getLowStockProducts(): Promise<ProductModel.Product[]> {
  "use cache";
  cacheTag(CACHE_TAGS.lowStock);
  cacheLife("seconds");
  
  return await ProductService.getLowStock();
}

export async function createProduct(data: ProductModel.CreateProductDto): Promise<ProductModel.Product> {
  const product = await ProductService.create(data);
  revalidateTag(CACHE_TAGS.products, "max");
  revalidateTag(CACHE_TAGS.lowStock, "max");
  revalidateTag(CACHE_TAGS.search, "max");
  return product;
}

export async function updateProduct(id: string, data: ProductModel.UpdateProductDto): Promise<ProductModel.Product> {
  const product = await ProductService.update(id, data);
  revalidateTag(CACHE_TAGS.product(id), "max");
  revalidateTag(CACHE_TAGS.products, "max");
  revalidateTag(CACHE_TAGS.lowStock, "max");
  revalidateTag(CACHE_TAGS.search, "max");
  return product;
}

export async function deleteProduct(id: string): Promise<void> {
  await ProductService.delete(id);
  revalidateTag(CACHE_TAGS.product(id), "max");
  revalidateTag(CACHE_TAGS.products, "max");
  revalidateTag(CACHE_TAGS.lowStock, "max");
  revalidateTag(CACHE_TAGS.search, "max");
}
