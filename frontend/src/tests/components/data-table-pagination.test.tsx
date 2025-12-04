import { describe, it, expect, vi, beforeEach } from "vitest";

const mockPush = vi.fn();
const mockReplace = vi.fn();

vi.mock("next/navigation", () => ({
  useRouter: () => ({
    push: mockPush,
    replace: mockReplace,
  }),
  useSearchParams: () => new URLSearchParams("page=1"),
}));

describe("DataTable Pagination Logic", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe("getValidPage", () => {
    const getValidPage = (page: number, totalPages: number): number => {
      if (page < 1) return 1;
      if (totalPages > 0 && page > totalPages) return totalPages;
      return page;
    };

    it("should return 1 when page is 0", () => {
      expect(getValidPage(0, 5)).toBe(1);
    });

    it("should return 1 when page is negative", () => {
      expect(getValidPage(-1, 5)).toBe(1);
      expect(getValidPage(-10, 5)).toBe(1);
    });

    it("should return totalPages when page exceeds totalPages", () => {
      expect(getValidPage(10, 5)).toBe(5);
      expect(getValidPage(100, 3)).toBe(3);
    });

    it("should return the same page when within valid range", () => {
      expect(getValidPage(1, 5)).toBe(1);
      expect(getValidPage(3, 5)).toBe(3);
      expect(getValidPage(5, 5)).toBe(5);
    });

    it("should return 1 when totalPages is 0 and page is 0", () => {
      expect(getValidPage(0, 0)).toBe(1);
    });

    it("should return page when totalPages is 0 but page is positive", () => {
      expect(getValidPage(1, 0)).toBe(1);
    });
  });

  describe("shouldRedirectOnEmptyPage", () => {
    const shouldRedirectOnEmptyPage = (
      dataLength: number,
      currentPage: number,
      totalPages: number
    ): number | null => {
      if (currentPage < 1) return 1;
      if (totalPages > 0 && currentPage > totalPages) return totalPages;
      if (dataLength === 0 && currentPage > 1) return currentPage - 1;
      return null;
    };

    it("should redirect to previous page when data is empty and page > 1", () => {
      expect(shouldRedirectOnEmptyPage(0, 3, 5)).toBe(2);
      expect(shouldRedirectOnEmptyPage(0, 2, 5)).toBe(1);
    });

    it("should not redirect when data is empty but page is 1", () => {
      expect(shouldRedirectOnEmptyPage(0, 1, 5)).toBeNull();
    });

    it("should not redirect when data exists", () => {
      expect(shouldRedirectOnEmptyPage(5, 3, 5)).toBeNull();
      expect(shouldRedirectOnEmptyPage(10, 1, 5)).toBeNull();
    });

    it("should redirect to page 1 when current page is 0", () => {
      expect(shouldRedirectOnEmptyPage(5, 0, 5)).toBe(1);
    });

    it("should redirect to page 1 when current page is negative", () => {
      expect(shouldRedirectOnEmptyPage(5, -1, 5)).toBe(1);
    });

    it("should redirect to totalPages when current page exceeds it", () => {
      expect(shouldRedirectOnEmptyPage(5, 10, 5)).toBe(5);
    });
  });

  describe("clampPage", () => {
    const clampPage = (page: number, totalPages: number): number => {
      return Math.max(1, Math.min(page, totalPages || 1));
    };

    it("should clamp page to 1 when page is less than 1", () => {
      expect(clampPage(0, 5)).toBe(1);
      expect(clampPage(-5, 5)).toBe(1);
    });

    it("should clamp page to totalPages when page exceeds it", () => {
      expect(clampPage(10, 5)).toBe(5);
      expect(clampPage(100, 3)).toBe(3);
    });

    it("should return page when within valid range", () => {
      expect(clampPage(1, 5)).toBe(1);
      expect(clampPage(3, 5)).toBe(3);
      expect(clampPage(5, 5)).toBe(5);
    });

    it("should return 1 when totalPages is 0", () => {
      expect(clampPage(5, 0)).toBe(1);
      expect(clampPage(1, 0)).toBe(1);
    });
  });

  describe("Edge cases", () => {
    it("should handle single page scenario", () => {
      const clampPage = (page: number, totalPages: number): number => {
        return Math.max(1, Math.min(page, totalPages || 1));
      };

      expect(clampPage(1, 1)).toBe(1);
      expect(clampPage(2, 1)).toBe(1);
      expect(clampPage(0, 1)).toBe(1);
    });

    it("should handle deleting last item on last page", () => {
      const shouldRedirectOnEmptyPage = (
        dataLength: number,
        currentPage: number,
        totalPages: number
      ): number | null => {
        if (currentPage < 1) return 1;
        if (totalPages > 0 && currentPage > totalPages) return totalPages;
        if (dataLength === 0 && currentPage > 1) return currentPage - 1;
        return null;
      };

      expect(shouldRedirectOnEmptyPage(0, 5, 4)).toBe(4);
    });

    it("should handle deleting all items from page 1", () => {
      const shouldRedirectOnEmptyPage = (
        dataLength: number,
        currentPage: number,
        totalPages: number
      ): number | null => {
        if (currentPage < 1) return 1;
        if (totalPages > 0 && currentPage > totalPages) return totalPages;
        if (dataLength === 0 && currentPage > 1) return currentPage - 1;
        return null;
      };

      expect(shouldRedirectOnEmptyPage(0, 1, 0)).toBeNull();
    });
  });
});
