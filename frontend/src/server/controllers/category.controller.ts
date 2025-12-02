"use server";

import { revalidatePath } from "next/cache";
import { getValidAuthToken } from ".";
import { CategoryModel } from "../models/category.model";
import { CategoryService } from "../services/category.service";

async function getService(): Promise<CategoryService> {
  const token = await getValidAuthToken();
  return CategoryService.initialize(token);
}

export async function getAllCategories(query?: CategoryModel.GetAllCategoriesQuery): Promise<CategoryModel.PaginatedCategories> {
  const service = await getService();
  return await service.getAll(query);
}

export async function getCategoryById(id: string): Promise<CategoryModel.Category> {
  const service = await getService();
  return await service.getById(id);
}

export async function createCategory(data: CategoryModel.CreateCategoryDto): Promise<CategoryModel.Category> {
  const service = await getService();
  const category = await service.create(data);
  revalidatePath("/categories");
  return category;
}

export async function updateCategory(
  id: string,
  data: CategoryModel.UpdateCategoryDto
): Promise<CategoryModel.Category> {
  const service = await getService();
  const category = await service.update(id, data);
  revalidatePath("/categories");
  return category;
}

export async function deleteCategory(id: string): Promise<void> {
  const service = await getService();
  await service.delete(id);
  revalidatePath("/categories");
}
