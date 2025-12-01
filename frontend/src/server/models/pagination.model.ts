import { z } from "zod";

export namespace PaginationModel {
  export const PaginatedResponseSchema = <T extends z.ZodTypeAny>(itemSchema: T) =>
    z.object({
      items: z.array(itemSchema),
      page: z.number(),
      pageSize: z.number(),
      totalCount: z.number(),
      totalPages: z.number(),
      hasPreviousPage: z.boolean(),
      hasNextPage: z.boolean(),
    });

  export type PaginatedResponse<T> = {
    items: T[];
    page: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
    hasPreviousPage: boolean;
    hasNextPage: boolean;
  };

  export type PaginationParams = {
    page?: number;
    pageSize?: number;
  };
}
