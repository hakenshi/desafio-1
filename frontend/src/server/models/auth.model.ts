import { z } from "zod";

export namespace AuthModel {
    // Login
    export const LoginRequestSchema = z.object({
        email: z.string().email("Email inválido"),
        password: z.string().min(1, "Senha é obrigatória"),
    });

    export const TokenResponseSchema = z.object({
        accessToken: z.string(),
        refreshToken: z.string().optional(),
        expiresIn: z.number(),
        tokenType: z.string(),
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
        id: z.string(),
        username: z.string(),
        email: z.email(),
        firstName: z.string(),
        lastName: z.string(),
        roles: z.array(z.string())
    });

    // Types
    export type LoginRequest = z.infer<typeof LoginRequestSchema>;
    export type TokenResponse = z.infer<typeof TokenResponseSchema>;
    export type RegisterRequest = z.infer<typeof RegisterRequestSchema>;
    export type UserInfo = z.infer<typeof UserInfoSchema>;
}
