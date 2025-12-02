import { CategoryModel } from "../models/category.model";
import { BaseService } from "./base.service";

export class CategoryService extends BaseService {
  private static instance: CategoryService | null = null;

  private constructor(token?: string) {
    super(token);
  }

  static initialize(token?: string): CategoryService {
    if (!CategoryService.instance) {
      CategoryService.instance = new CategoryService(token);
    } else {
      CategoryService.instance.setToken(token);
    }
    return CategoryService.instance;
  }

  static getInstance(): CategoryService {
    if (!CategoryService.instance) {
      CategoryService.instance = new CategoryService();
    }
    return CategoryService.instance;
  }
  async getAll(query?: CategoryModel.GetAllCategoriesQuery): Promise<CategoryModel.PaginatedCategories> {
    const validatedQuery = CategoryModel.GetAllCategoriesQuerySchema.parse(query || {});
    const params = new URLSearchParams({
      page: validatedQuery.page.toString(),
      pageSize: validatedQuery.pageSize.toString(),
    });

    const response = await this.client.get<CategoryModel.PaginatedCategories>(
      `/categories?${params.toString()}`, 
      undefined, 
      this.token
    );
    return CategoryModel.PaginatedCategoriesSchema.parse(response);
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
