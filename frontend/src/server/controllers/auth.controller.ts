"use server";

import { AuthModel } from "../models/auth.model";
import { AuthService } from "../services/auth.service";

export async function login(data: AuthModel.LoginRequest): Promise<AuthModel.TokenResponse> {
  return await AuthService.login(data);
}

export async function register(data: AuthModel.RegisterRequest): Promise<void> {
  return await AuthService.register(data);
}

export async function refreshToken(): Promise<AuthModel.TokenResponse> {
  return await AuthService.refreshToken();
}

export async function logout(refreshToken?: string): Promise<void> {
  return await AuthService.logout(refreshToken);
}

export async function getUserInfo(): Promise<AuthModel.UserInfo> {
  return await AuthService.getUserInfo();
}
