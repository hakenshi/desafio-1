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

type FetchOptions = {
  method?: string;
  body?: string;
  headers?: Record<string, string>;
  cache?: RequestCache;
  next?: { tags?: string[]; revalidate?: number | false };
};

export class ApiClient {
  private async request<T>(
    endpoint: string,
    options?: FetchOptions,
    token?: string
  ): Promise<T> {
    const url = `${BASE_URL}${endpoint}`;

    const headers: Record<string, string> = {
      "Content-Type": "application/json",
      ...options?.headers,
    };

    if (token) {
      headers.Authorization = `Bearer ${token}`;
    }

    try {
      const response = await fetch(url, {
        method: options?.method,
        body: options?.body,
        headers,
        cache: options?.cache,
        next: options?.next,
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

  async get<T>(endpoint: string, options?: FetchOptions, token?: string): Promise<T> {
    return this.request<T>(endpoint, { ...options, method: "GET" }, token);
  }

  async post<T>(
    endpoint: string,
    data?: unknown,
    options?: FetchOptions,
    token?: string
  ): Promise<T> {
    return this.request<T>(
      endpoint,
      {
        ...options,
        method: "POST",
        body: data ? JSON.stringify(data) : undefined,
      },
      token
    );
  }

  async put<T>(
    endpoint: string,
    data?: unknown,
    options?: FetchOptions,
    token?: string
  ): Promise<T> {
    return this.request<T>(
      endpoint,
      {
        ...options,
        method: "PUT",
        body: data ? JSON.stringify(data) : undefined,
      },
      token
    );
  }

  async delete<T>(endpoint: string, options?: FetchOptions, token?: string): Promise<T> {
    return this.request<T>(endpoint, { ...options, method: "DELETE" }, token);
  }
}

export const apiClient = new ApiClient();
