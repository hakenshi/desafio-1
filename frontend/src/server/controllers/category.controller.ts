"use server";

import { CategoryModel } from "../models/category.model";
import { CategoryService } from "../services/category.service";
import { cacheTag, cacheLife, revalidateTag } from "next/cache";

const CACHE_TAGS = {
  categories: "categories",
  category: (id: string) => `category-${id}`,
};

export async function getAllCategories(): Promise<CategoryModel.Category[]> {
  "use cache";
  cacheTag(CACHE_TAGS.categories);
  cacheLife("hours");

  return await CategoryService.getAll();
}

export async function getCategoryById(id: string): Promise<CategoryModel.Category> {
  "use cache";
  cacheTag(CACHE_TAGS.category(id));
  cacheLife("hours");

  return await CategoryService.getById(id);
}

export async function createCategory(data: CategoryModel.CreateCategoryDto): Promise<CategoryModel.Category> {
  const category = await CategoryService.create(data);
  revalidateTag(CACHE_TAGS.categories, "max");
  return category;
}

export async function updateCategory(
  id: string,
  data: CategoryModel.UpdateCategoryDto
): Promise<CategoryModel.Category> {
  const category = await CategoryService.update(id, data);
  revalidateTag(CACHE_TAGS.category(id), "max");
  revalidateTag(CACHE_TAGS.categories, "max");
  return category;
}

export async function deleteCategory(id: string): Promise<void> {
  await CategoryService.delete(id);
  revalidateTag(CACHE_TAGS.category(id), "max");
  revalidateTag(CACHE_TAGS.categories, "max");
}
