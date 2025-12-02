"use server";

import { AuthModel } from "../models/auth.model";
import { AuthService } from "../services/auth.service";
import { cookies } from "next/headers";
import { revalidatePath } from "next/cache";
import { getValidAuthToken } from "./token.controller";

async function getService(withToken: boolean = false): Promise<AuthService> {
  const token = withToken ? await getValidAuthToken() : undefined;
  return new AuthService(token);
}

export async function login(data: AuthModel.LoginRequest): Promise<AuthModel.TokenResponse> {
  const service = await getService();
  return await service.login(data);
}

export async function register(data: AuthModel.RegisterRequest): Promise<void> {
  const service = await getService();
  await service.register(data);
  revalidatePath("/users");
}

export async function refreshToken(): Promise<AuthModel.TokenResponse> {
  const service = await getService();
  return await service.refreshToken();
}

export async function logout(): Promise<void> {
  const cookieStore = await cookies();
  const refreshTokenValue = cookieStore.get("refreshToken")?.value;

  // Try to logout from Keycloak
  try {
    const service = await getService(true);
    await service.logout(refreshTokenValue);
  } catch {
    // Ignore errors, we'll clear cookies anyway
  }

  // Clear cookies
  cookieStore.delete("authToken");
  cookieStore.delete("refreshToken");
}

export async function getUserInfo(): Promise<AuthModel.UserInfo> {
  const service = await getService(true);
  return await service.getUserInfo();
}

export async function getUsers(): Promise<AuthModel.KeycloakUser[]> {
  const service = await getService(true);
  return await service.getUsers();
}

export async function updateUser(id: string, data: AuthModel.UpdateUserRequest): Promise<void> {
  const service = await getService(true);
  await service.updateUser(id, data);
  revalidatePath("/users");
}

export async function deleteUser(id: string): Promise<void> {
  const service = await getService(true);
  await service.deleteUser(id);
  revalidatePath("/users");
}
