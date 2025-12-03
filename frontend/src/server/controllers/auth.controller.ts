"use server";

import { revalidateTag, refresh } from "next/cache";
import { AuthModel } from "../models/auth.model";
import { AuthService } from "../services/auth.service";
import { cookies } from "next/headers";
import { getValidAuthToken } from "./token.controller";

// Cache profile for immediate revalidation
const REVALIDATE_NOW = { expire: 0 };

// Public actions - no token needed
export async function login(
  data: AuthModel.LoginRequest
): Promise<AuthModel.TokenResponse> {
  const service = new AuthService();
  return await service.login(data);
}

export async function register(data: AuthModel.RegisterRequest): Promise<void> {
  const service = new AuthService();
  await service.register(data);
  revalidateTag("users", REVALIDATE_NOW);
}

export async function refreshToken(): Promise<AuthModel.TokenResponse> {
  const service = new AuthService();
  return await service.refreshToken();
}

// For server components - token passed explicitly
export async function getUserInfo(token: string): Promise<AuthModel.UserInfo> {
  const service = new AuthService(token);
  return await service.getUserInfo();
}

export async function getUsers(
  token: string
): Promise<AuthModel.KeycloakUser[]> {
  const service = new AuthService(token);
  return await service.getUsers();
}

// For client components - token fetched internally
export async function logout(): Promise<void> {
  const token = await getValidAuthToken();
  const cookieStore = await cookies();
  const refreshTokenValue = cookieStore.get("refreshToken")?.value;

  try {
    const service = new AuthService(token);
    await service.logout(refreshTokenValue);
  } catch {
    // Ignore errors, we'll clear cookies anyway
  }

  cookieStore.delete("authToken");
  cookieStore.delete("refreshToken");
}

export async function updateUser(
  id: string,
  data: AuthModel.UpdateUserRequest
): Promise<void> {
  const token = await getValidAuthToken();
  const service = new AuthService(token);
  await service.updateUser(id, data);
  revalidateTag("users", REVALIDATE_NOW);
  revalidateTag("current-user", REVALIDATE_NOW);
  refresh(); // Force client cache refresh
}

export async function deleteUser(id: string): Promise<void> {
  const token = await getValidAuthToken();
  const service = new AuthService(token);
  await service.deleteUser(id);
  revalidateTag("users", REVALIDATE_NOW);
}
