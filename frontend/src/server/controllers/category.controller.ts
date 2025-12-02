"use server";

import { revalidatePath } from "next/cache";
import { CategoryModel } from "../models/category.model";
import { CategoryService } from "../services/category.service";
import { getValidAuthToken } from "./token.controller";

// For server components - token passed explicitly
export async function getAllCategories(
  token: string,
  query?: CategoryModel.GetAllCategoriesQuery
): Promise<CategoryModel.PaginatedCategories> {
  const service = new CategoryService(token);
  return await service.getAll(query);
}

export async function getCategoryById(
  token: string,
  id: string
): Promise<CategoryModel.Category> {
  const service = new CategoryService(token);
  return await service.getById(id);
}

// For client components - token fetched internally
export async function getCategories(
  query?: CategoryModel.GetAllCategoriesQuery
): Promise<CategoryModel.PaginatedCategories> {
  const token = await getValidAuthToken();
  const service = new CategoryService(token);
  return await service.getAll(query);
}

export async function createCategory(
  data: CategoryModel.CreateCategoryDto
): Promise<CategoryModel.Category> {
  const token = await getValidAuthToken();
  const service = new CategoryService(token);
  const category = await service.create(data);
  revalidatePath("/categories");
  return category;
}

export async function updateCategory(
  id: string,
  data: CategoryModel.UpdateCategoryDto
): Promise<CategoryModel.Category> {
  const token = await getValidAuthToken();
  const service = new CategoryService(token);
  const category = await service.update(id, data);
  revalidatePath("/categories");
  return category;
}

export async function deleteCategory(id: string): Promise<void> {
  const token = await getValidAuthToken();
  const service = new CategoryService(token);
  await service.delete(id);
  revalidatePath("/categories");
}
