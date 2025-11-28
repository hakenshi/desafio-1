"use server";

import { CategoryModel } from "../models/category.model";
import { apiClient } from "./api-client.service";
import { revalidateTag, cacheTag, cacheLife } from "next/cache";
import { z } from "zod";

export abstract class CategoryService {
  private static CACHE_TAGS = {
    categories: "categories",
    category: (id: string) => `category-${id}`,
  };

  static async getAll(): Promise<CategoryModel.Category[]> {
    "use cache";
    cacheTag(CategoryService.CACHE_TAGS.categories, "hours");
    cacheLife("hours");

    const response = await apiClient.get<CategoryModel.Category[]>("/categories");
    return z.array(CategoryModel.CategorySchema).parse(response);
  }

  static async getById(id: string): Promise<CategoryModel.Category> {
    "use cache";
    cacheTag(CategoryService.CACHE_TAGS.category(id));
    cacheLife("hours");

    const response = await apiClient.get<CategoryModel.Category>(`/categories/${id}`);
    return CategoryModel.CategorySchema.parse(response);
  }

  static async create(data: CategoryModel.CreateCategoryDto): Promise<CategoryModel.Category> {
    const validatedData = CategoryModel.CreateCategorySchema.parse(data);

    const response = await apiClient.post<CategoryModel.Category>("/categories", validatedData);
    const category = CategoryModel.CategorySchema.parse(response);

    revalidateTag(CategoryService.CACHE_TAGS.categories, "max");

    return category;
  }

  static async update(
    id: string,
    data: CategoryModel.UpdateCategoryDto
  ): Promise<CategoryModel.Category> {
    const validatedData = CategoryModel.UpdateCategorySchema.parse(data);

    const response = await apiClient.put<CategoryModel.Category>(`/categories/${id}`, validatedData);
    const category = CategoryModel.CategorySchema.parse(response);

    // Revalidate specific category and list
    revalidateTag(CategoryService.CACHE_TAGS.category(id), "max");
    revalidateTag(CategoryService.CACHE_TAGS.categories, "max");

    return category;
  }

  static async delete(id: string): Promise<void> {
    await apiClient.delete<void>(`/categories/${id}`);
    revalidateTag(CategoryService.CACHE_TAGS.category(id), "max");
    revalidateTag(CategoryService.CACHE_TAGS.categories, "max");
  }
}  
