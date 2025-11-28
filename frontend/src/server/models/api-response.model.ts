import { z } from "zod";

export namespace ApiModel {
  export const ApiErrorSchema = z.object({
    message: z.string(),
    errors: z.record(z.string(), z.array(z.string())).optional(),
    statusCode: z.number().optional(),
  });

  export const ApiSuccessSchema = <T extends z.ZodTypeAny>(dataSchema: T) =>
    z.object({
      data: dataSchema,
      message: z.string().optional(),
    });

  export type ApiError = z.infer<typeof ApiErrorSchema>;
  export type ApiSuccess<T> = {
    data: T;
    message?: string;
  };
}