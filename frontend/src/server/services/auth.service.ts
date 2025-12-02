import { cookies } from "next/headers";
import { z } from "zod";
import { AuthModel } from "../models/auth.model";
import { BaseService } from "./base.service";
import { redirect } from "next/navigation";

export class AuthService extends BaseService {
  private static instance: AuthService | null = null;

  private constructor(token?: string) {
    super(token);
  }

  static initialize(token?: string): AuthService {
    if (!AuthService.instance) {
      AuthService.instance = new AuthService(token);
    } else {
      AuthService.instance.setToken(token);
    }
    return AuthService.instance;
  }

  static getInstance(): AuthService {
    if (!AuthService.instance) {
      AuthService.instance = new AuthService();
    }
    return AuthService.instance;
  }
  async login(data: AuthModel.LoginRequest): Promise<AuthModel.TokenResponse> {
    const validatedData = AuthModel.LoginRequestSchema.parse(data);
    const response = await this.client.post<AuthModel.TokenResponse>(
      "/auth/login",
      validatedData
    );

    const authData = AuthModel.TokenResponseSchema.parse({ ...response });
    console.log(authData)
    const cookie = await cookies()

    const expiresAt = new Date(Date.now() + authData.expiresIn * 1000)

    cookie.set({
      name: "authToken",
      value: authData.accessToken,
      httpOnly: true,
      secure: process.env.NODE_ENV === "production",
      expires: expiresAt
    })


    if (authData.refreshToken) {
      cookie.set({
        name: "refreshToken",
        value: authData.refreshToken,
        httpOnly: true,
        secure: process.env.NODE_ENV === "production",
        expires: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000)
      })
    }
    redirect("/dashboard")
  }

  async register(data: AuthModel.RegisterRequest): Promise<void> {
    const validatedData = AuthModel.RegisterRequestSchema.parse(data);

    await this.client.post("/auth/register", validatedData);
  }

  async refreshToken(): Promise<AuthModel.TokenResponse> {
    const refreshToken = (await cookies()).get("refreshToken")?.value

    const response = await this.client.post<AuthModel.TokenResponse>(
      "/auth/refresh",
      { refresh_token: refreshToken }
    );

    return AuthModel.TokenResponseSchema.parse(response);
  }

  async logout(refreshToken?: string): Promise<void> {
    await this.client.post("/auth/logout", { refresh_token: refreshToken }, undefined, this.token);
  }

  async getUserInfo(): Promise<AuthModel.UserInfo> {
    const response = await this.client.get<AuthModel.UserInfo>("/auth/me", undefined, this.token);
    return AuthModel.UserInfoSchema.parse(response);
  }

  async getUsers(): Promise<AuthModel.KeycloakUser[]> {
    const response = await this.client.get<AuthModel.KeycloakUser[]>("/auth/users", undefined, this.token);
    return z.array(AuthModel.KeycloakUserSchema).parse(response);
  }

  async updateUser(id: string, data: AuthModel.UpdateUserRequest): Promise<void> {
    const validatedData = AuthModel.UpdateUserRequestSchema.parse(data);
    await this.client.put(`/auth/users/${id}`, validatedData, undefined, this.token);
  }

  async deleteUser(id: string): Promise<void> {
    await this.client.delete(`/auth/users/${id}`, undefined, this.token);
  }
}
