export class ApiClientError extends Error {
  constructor(
    public statusCode: number,
    public message: string,
    public errors?: Record<string, string[]>
  ) {
    super(message);
    this.name = "ApiClientError";
  }
}

const BASE_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000/api";

export class ApiClient {
  private async request<T>(
    endpoint: string,
    options?: RequestInit,
    token?: string
  ): Promise<T> {
    const url = `${BASE_URL}${endpoint}`;

    const headers: Record<string, string> = {
      "Content-Type": "application/json",
      ...(options?.headers as Record<string, string>),
    };

    // Adicionar Authorization header se o token for fornecido
    if (token) {
      headers.Authorization = `Bearer ${token}`;
    }

    try {
      const response = await fetch(url, {
        ...options,
        headers,
      });

      if (!response.ok) {
        const errorData = await response.json().catch(() => ({
          message: response.statusText,
        }));

        throw new ApiClientError(
          response.status,
          errorData.message || "An error occurred",
          errorData.errors
        );
      }

      // Handle 204 No Content
      if (response.status === 204) {
        return undefined as T;
      }

      return await response.json();
    } catch (error) {
      if (error instanceof ApiClientError) {
        throw error;
      }

      throw new ApiClientError(
        500,
        error instanceof Error ? error.message : "Network error"
      );
    }
  }

  async get<T>(endpoint: string, options?: RequestInit, token?: string): Promise<T> {
    return this.request<T>(endpoint, { ...options, method: "GET" }, token);
  }

  async post<T>(
    endpoint: string,
    data?: unknown,
    options?: RequestInit,
    token?: string
  ): Promise<T> {
    return this.request<T>(endpoint, {
      ...options,
      method: "POST",
      body: data ? JSON.stringify(data) : undefined,
    }, token);
  }

  async put<T>(
    endpoint: string,
    data?: unknown,
    options?: RequestInit,
    token?: string
  ): Promise<T> {
    return this.request<T>(endpoint, {
      ...options,
      method: "PUT",
      body: data ? JSON.stringify(data) : undefined,
    }, token);
  }

  async delete<T>(endpoint: string, options?: RequestInit, token?: string): Promise<T> {
    return this.request<T>(endpoint, { ...options, method: "DELETE" }, token);
  }
}

// Singleton instance
export const apiClient = new ApiClient();
