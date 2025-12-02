"use server";

import { CategoryModel } from "../models/category.model";
import { CategoryService } from "../services/category.service";
import { revalidatePath, revalidateTag } from "next/cache";
import { cookies } from "next/headers";

const CACHE_TAGS = {
  categories: "categories",
  category: (id: string) => `category-${id}`,
};

async function getAuthToken(): Promise<string | undefined> {
  const cookieStore = await cookies();
  return cookieStore.get("authToken")?.value;
}

async function getService(): Promise<CategoryService> {
  const token = await getAuthToken();
  return new CategoryService(token);
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
  revalidatePath("/categories")
  return category;
}

export async function updateCategory(
  id: string,
  data: CategoryModel.UpdateCategoryDto
): Promise<CategoryModel.Category> {
  const service = await getService();
  const category = await service.update(id, data);
  revalidatePath("/categories")
  return category;
}

export async function deleteCategory(id: string): Promise<void> {
  const service = await getService();
  await service.delete(id);
  revalidatePath("/categories")
}
