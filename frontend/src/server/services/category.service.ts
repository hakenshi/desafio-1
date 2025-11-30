import { CategoryModel } from "../models/category.model";
import { apiClient } from "./api-client.service";
import { revalidateTag, cacheTag, cacheLife } from "next/cache";
import { z } from "zod";

export abstract class CategoryService {
  static async getAll(): Promise<CategoryModel.Category[]> {
    const response = await apiClient.get<CategoryModel.Category[]>("/categories");
    return z.array(CategoryModel.CategorySchema).parse(response);
  }

  static async getById(id: string): Promise<CategoryModel.Category> {
    const response = await apiClient.get<CategoryModel.Category>(`/categories/${id}`);
    return CategoryModel.CategorySchema.parse(response);
  }

  static async create(data: CategoryModel.CreateCategoryDto): Promise<CategoryModel.Category> {
    const validatedData = CategoryModel.CreateCategorySchema.parse(data);
    const response = await apiClient.post<CategoryModel.Category>("/categories", validatedData);
    return CategoryModel.CategorySchema.parse(response);
  }

  static async update(
    id: string,
    data: CategoryModel.UpdateCategoryDto
  ): Promise<CategoryModel.Category> {
    const validatedData = CategoryModel.UpdateCategorySchema.parse(data);
    const response = await apiClient.put<CategoryModel.Category>(`/categories/${id}`, validatedData);
    return CategoryModel.CategorySchema.parse(response);
  }

  static async delete(id: string): Promise<void> {
    await apiClient.delete<void>(`/categories/${id}`);
  }
}  
