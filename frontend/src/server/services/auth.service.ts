"use server";

import { AuthModel } from "../models/auth.model";
import { apiClient } from "./api-client.service";

export abstract class AuthService {
  static async login(data: AuthModel.LoginRequest): Promise<AuthModel.TokenResponse> {
    const validatedData = AuthModel.LoginRequestSchema.parse(data);
    
    const response = await apiClient.post<AuthModel.TokenResponse>(
      "/auth/login",
      validatedData
    );
    
    return AuthModel.TokenResponseSchema.parse(response);
  }

  static async register(data: AuthModel.RegisterRequest): Promise<void> {
    const validatedData = AuthModel.RegisterRequestSchema.parse(data);
    
    await apiClient.post("/auth/register", validatedData);
  }

  static async refreshToken(refreshToken: string): Promise<AuthModel.TokenResponse> {
    const response = await apiClient.post<AuthModel.TokenResponse>(
      "/auth/refresh",
      { refresh_token: refreshToken }
    );
    
    return AuthModel.TokenResponseSchema.parse(response);
  }

  static async logout(refreshToken?: string): Promise<void> {
    await apiClient.post("/auth/logout", { refresh_token: refreshToken });
  }

  static async getUserInfo(accessToken: string): Promise<AuthModel.UserInfo> {
    const response = await apiClient.get<AuthModel.UserInfo>("/auth/userinfo", {
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    });
    
    return AuthModel.UserInfoSchema.parse(response);
  }
}
