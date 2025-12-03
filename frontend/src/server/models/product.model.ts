import { z } from "zod";
import { PaginationModel } from "./pagination.model";

export namespace ProductModel {
  // Product Schema
  export const ProductSchema = z.object({
    id: z.string(),
    sku: z.string(),
    name: z.string(),
    description: z.string(),
    price: z.number(),
    categoryId: z.string(),
    categoryName: z.string(),
    stockQuantity: z.number(),
    isLowStock: z.boolean(),
    createdAt: z.string().datetime(),
    updatedAt: z.string().datetime(),
  });

  // Paginated Response Schema
  export const PaginatedProductsSchema = PaginationModel.PaginatedResponseSchema(ProductSchema);

  // Create Product Schema
  export const CreateProductSchema = z.object({
    name: z.string()
      .min(3, "Name must be at least 3 characters")
      .max(200, "Name must be at most 200 characters"),
    description: z.string()
      .min(10, "Description must be at least 10 characters")
      .max(1000, "Description must be at most 1000 characters"),
    price: z.number()
      .positive("Price must be greater than zero")
      .max(1000000, "Price must be at most 1,000,000"),
    categoryId: z.string().min(1, "Category is required"),
    stockQuantity: z.number()
      .int()
      .min(0, "Stock quantity cannot be negative")
      .max(100000, "Stock quantity must be at most 100,000"),
  });

  // Update Product Schema
  export const UpdateProductSchema = CreateProductSchema;

  // Search Products Query Schema
  export const SearchProductsQuerySchema = z.object({
    searchTerm: z.string().optional(),
    categoryId: z.string().optional(),
    page: z.number().int().positive().optional().default(1),
    pageSize: z.number().int().positive().optional().default(10),
  });

  // Get All Products Query Schema
  export const GetAllProductsQuerySchema = z.object({
    page: z.number().int().positive().optional().default(1),
    pageSize: z.number().int().positive().optional().default(10),
    categoryId: z.string().optional(),
  });

  // Types
  export type Product = z.infer<typeof ProductSchema>;
  export type PaginatedProducts = PaginationModel.PaginatedResponse<Product>;
  export type CreateProductDto = z.infer<typeof CreateProductSchema>;
  export type UpdateProductDto = z.infer<typeof UpdateProductSchema>;
  export type SearchProductsQuery = z.infer<typeof SearchProductsQuerySchema>;
  export type GetAllProductsQuery = z.infer<typeof GetAllProductsQuerySchema>;
}