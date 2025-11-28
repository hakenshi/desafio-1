import { z } from "zod";

export namespace CategoryModel {
  export const CategorySchema = z.object({
    id: z.string(),
    name: z.string(),
    description: z.string(),
    createdAt: z.string().datetime(),
    updatedAt: z.string().datetime(),
  });

  export const CreateCategorySchema = z.object({
    name: z.string().min(1, "Category name is required"),
    description: z.string().min(1, "Description is required"),
  });

  export const UpdateCategorySchema = CreateCategorySchema;

  export type Category = z.infer<typeof CategorySchema>;
  export type CreateCategoryDto = z.infer<typeof CreateCategorySchema>;
  export type UpdateCategoryDto = z.infer<typeof UpdateCategorySchema>;
}