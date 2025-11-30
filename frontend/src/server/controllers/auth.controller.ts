"use server";

import { AuthModel } from "../models/auth.model";
import { AuthService } from "../services/auth.service";
import { cookies } from "next/headers";

async function getAuthToken(): Promise<string | undefined> {
  const cookieStore = await cookies();
  return cookieStore.get("authToken")?.value;
}

async function getService(withToken: boolean = false): Promise<AuthService> {
  const token = withToken ? await getAuthToken() : undefined;
  return new AuthService(token);
}

export async function login(data: AuthModel.LoginRequest): Promise<AuthModel.TokenResponse> {
  const service = await getService();
  return await service.login(data);
}

export async function register(data: AuthModel.RegisterRequest): Promise<void> {
  const service = await getService();
  return await service.register(data);
}

export async function refreshToken(): Promise<AuthModel.TokenResponse> {
  const service = await getService();
  return await service.refreshToken();
}

export async function logout(refreshToken?: string): Promise<void> {
  const service = await getService(true);
  return await service.logout(refreshToken);
}

export async function getUserInfo(): Promise<AuthModel.UserInfo> {
  const service = await getService(true);
  return await service.getUserInfo();
}
