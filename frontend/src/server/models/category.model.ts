import { z } from "zod";
import { PaginationModel } from "./pagination.model";

export namespace CategoryModel {
  export const CategorySchema = z.object({
    id: z.string(),
    name: z.string(),
    description: z.string(),
    createdAt: z.string().datetime(),
    updatedAt: z.string().datetime(),
  });

  export const PaginatedCategoriesSchema = PaginationModel.PaginatedResponseSchema(CategorySchema);

  export const CreateCategorySchema = z.object({
    name: z.string().min(1, "Category name is required"),
    description: z.string().min(1, "Description is required"),
  });

  export const UpdateCategorySchema = CreateCategorySchema;

  export const GetAllCategoriesQuerySchema = z.object({
    page: z.number().int().positive().optional().default(1),
    pageSize: z.number().int().positive().optional().default(10),
  });

  export type Category = z.infer<typeof CategorySchema>;
  export type PaginatedCategories = PaginationModel.PaginatedResponse<Category>;
  export type CreateCategoryDto = z.infer<typeof CreateCategorySchema>;
  export type UpdateCategoryDto = z.infer<typeof UpdateCategorySchema>;
  export type GetAllCategoriesQuery = z.infer<typeof GetAllCategoriesQuerySchema>;
}