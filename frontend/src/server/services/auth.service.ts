import { cookies } from "next/headers";
import { AuthModel } from "../models/auth.model";
import { apiClient } from "./api-client.service";
import { redirect } from "next/navigation";

export abstract class AuthService {
  static async login(data: AuthModel.LoginRequest): Promise<AuthModel.TokenResponse> {
    const validatedData = AuthModel.LoginRequestSchema.parse(data);
    console.log(validatedData)
    const response = await apiClient.post<AuthModel.TokenResponse>(
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

  static async register(data: AuthModel.RegisterRequest): Promise<void> {
    const validatedData = AuthModel.RegisterRequestSchema.parse(data);

    await apiClient.post("/auth/register", validatedData);
  }

  static async refreshToken(): Promise<AuthModel.TokenResponse> {

    const refreshToken = (await cookies()).get("refreshToken")?.value

    const response = await apiClient.post<AuthModel.TokenResponse>(
      "/auth/refresh",
      { refresh_token: refreshToken }
    );

    return AuthModel.TokenResponseSchema.parse(response);
  }

  static async logout(refreshToken?: string): Promise<void> {
    await apiClient.post("/auth/logout", { refresh_token: refreshToken });
  }

  static async getUserInfo(): Promise<AuthModel.UserInfo> {

    const accessToken = (await cookies()).get("authToken")?.value


    const response = await apiClient.get<AuthModel.UserInfo>("/auth/me", {
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    });

    console.log(response)

    return AuthModel.UserInfoSchema.parse(response);
  }
}
