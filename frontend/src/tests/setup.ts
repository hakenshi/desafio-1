import { afterEach, vi } from "vitest";
import { cleanup } from "@testing-library/react";
import "@testing-library/jest-dom/vitest";

// Cleanup após cada teste
afterEach(() => {
  cleanup();
  vi.clearAllMocks();
});

// Mock do fetch global apenas para testes unitários
// Testes de integração vão sobrescrever isso
if (!global.fetch) {
  global.fetch = vi.fn();
}

// Mock das variáveis de ambiente
process.env.NEXT_PUBLIC_API_URL = "http://localhost:5000/api";
