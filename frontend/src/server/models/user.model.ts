import { z } from "zod";

export namespace UserModel {
  export const UserSchema = z.object({
    id: z.string(),
    username: z.string(),
    email: z.email(),
    firstName: z.string().nullable().optional(),
    lastName: z.string().nullable().optional(),
    roles: z.array(z.string()),
  });

  export type User = z.infer<typeof UserSchema>;
}
