import { z } from "zod";

export namespace AuthModel {
    // Login
    export const LoginRequestSchema = z.object({
        email: z.string().email("Email inválido"),
        password: z.string().min(1, "Senha é obrigatória"),
    });

    export const TokenResponseSchema = z.object({
        access_token: z.string(),
        refresh_token: z.string().optional(),
        expires_in: z.number(),
        token_type: z.string(),
    });

    // Register
    export const RegisterRequestSchema = z.object({
        username: z.string().min(3, "Username deve ter no mínimo 3 caracteres"),
        email: z.string().email("Email inválido"),
        password: z.string().min(6, "Senha deve ter no mínimo 6 caracteres"),
        firstName: z.string().optional(),
        lastName: z.string().optional(),
    });

    // User Info
    export const UserInfoSchema = z.object({
        sub: z.string(),
        username: z.string().optional(),
        email: z.string().optional(),
        email_verified: z.boolean().optional(),
        given_name: z.string().optional(),
        family_name: z.string().optional(),
        roles: z.array(z.string()).optional(),
    });

    // Types
    export type LoginRequest = z.infer<typeof LoginRequestSchema>;
    export type TokenResponse = z.infer<typeof TokenResponseSchema>;
    export type RegisterRequest = z.infer<typeof RegisterRequestSchema>;
    export type UserInfo = z.infer<typeof UserInfoSchema>;
}
