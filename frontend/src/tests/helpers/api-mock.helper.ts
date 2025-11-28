import { vi } from "vitest";

export class ApiMockHelper {
  static mockSuccessResponse<T>(data: T, status = 200) {
    return vi.fn().mockResolvedValue({
      ok: true,
      status,
      json: async () => data,
    } as Response);
  }

  static mockErrorResponse(
    statusCode: number,
    message: string,
    errors?: Record<string, string[]>
  ) {
    return vi.fn().mockResolvedValue({
      ok: false,
      status: statusCode,
      statusText: message,
      json: async () => ({ message, errors }),
    } as Response);
  }

  static mockNoContentResponse() {
    return vi.fn().mockResolvedValue({
      ok: true,
      status: 204,
    } as Response);
  }

  static mockNetworkError() {
    return vi.fn().mockRejectedValue(new Error("Network error"));
  }

  static measureResponseTime(fn: () => Promise<unknown>): Promise<number> {
    const start = performance.now();
    return fn().then(() => performance.now() - start);
  }
}
