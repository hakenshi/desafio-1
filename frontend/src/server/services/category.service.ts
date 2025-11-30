import { CategoryModel } from "../models/category.model";
import { BaseService } from "./base.service";
import { z } from "zod";

export class CategoryService extends BaseService {
  async getAll(): Promise<CategoryModel.Category[]> {
    const response = await this.client.get<CategoryModel.Category[]>(
      "/categories", 
      undefined, 
      this.token
    );
    return z.array(CategoryModel.CategorySchema).parse(response);
  }

  async getById(id: string): Promise<CategoryModel.Category> {
    const response = await this.client.get<CategoryModel.Category>(
      `/categories/${id}`, 
      undefined, 
      this.token
    );
    return CategoryModel.CategorySchema.parse(response);
  }

  async create(data: CategoryModel.CreateCategoryDto): Promise<CategoryModel.Category> {
    const validatedData = CategoryModel.CreateCategorySchema.parse(data);
    const response = await this.client.post<CategoryModel.Category>(
      "/categories", 
      validatedData, 
      undefined, 
      this.token
    );
    return CategoryModel.CategorySchema.parse(response);
  }

  async update(
    id: string,
    data: CategoryModel.UpdateCategoryDto
  ): Promise<CategoryModel.Category> {
    const validatedData = CategoryModel.UpdateCategorySchema.parse(data);
    const response = await this.client.put<CategoryModel.Category>(
      `/categories/${id}`, 
      validatedData, 
      undefined, 
      this.token
    );
    return CategoryModel.CategorySchema.parse(response);
  }

  async delete(id: string): Promise<void> {
    await this.client.delete<void>(`/categories/${id}`, undefined, this.token);
  }
}  
