import { z } from "zod";

export namespace UserModel {
  export const UserSchema = z.object({
    id: z.string(),
    username: z.string(),
    email: z.email(),
    firstName: z.string().nullable().optional(),
    lastName: z.string().nullable().optional(),
    role: z.enum(["admin", "manager", "user"]),
  });

  export type User = z.infer<typeof UserSchema>;
}
