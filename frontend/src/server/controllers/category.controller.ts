"use server";

import { revalidatePath } from "next/cache";
import { actions, getValidAuthToken } from ".";
import { CategoryModel } from "../models/category.model";
import { CategoryService } from "../services/category.service";

const CACHE_TAGS = {
  categories: "categories",
  category: (id: string) => `category-${id}`,
};

const token = await getValidAuthToken();

const service = new CategoryService(token)

export async function getAllCategories(query?: CategoryModel.GetAllCategoriesQuery): Promise<CategoryModel.PaginatedCategories> {
  return await service.getAll(query);
}

export async function getCategoryById(id: string): Promise<CategoryModel.Category> {
  return await service.getById(id);
}

export async function createCategory(data: CategoryModel.CreateCategoryDto): Promise<CategoryModel.Category> {
  const category = await service.create(data);
  revalidatePath("/categories")
  return category;
}

export async function updateCategory(
  id: string,
  data: CategoryModel.UpdateCategoryDto
): Promise<CategoryModel.Category> {
  const category = await service.update(id, data);
  revalidatePath("/categories")
  return category;
}

export async function deleteCategory(id: string): Promise<void> {
  await service.delete(id);
  revalidatePath("/categories")
}
