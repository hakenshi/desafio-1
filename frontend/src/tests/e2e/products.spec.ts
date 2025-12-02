import { test, expect, Page } from "@playwright/test";

// Track created items for cleanup
const createdProducts: string[] = [];

async function deleteProduct(page: Page, productName: string): Promise<void> {
  await page.goto("/products");
  await page.waitForLoadState("networkidle");
  await page.fill('input[placeholder*="Search"]', productName);
  await page.waitForTimeout(500);

  const row = page.locator(`table tbody tr:has-text("${productName}")`);
  if (await row.isVisible()) {
    await row.locator("button").last().click();
    await page.click('[role="menuitem"]:has-text("Delete product")');
    await page.click(
      '[role="dialog"] button:has-text("Delete"):not(:has-text("Cancel"))'
    );
    await expect(page.locator('[role="dialog"]')).not.toBeVisible({
      timeout: 10000,
    });
  }
}

test.describe("Products CRUD", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto("/login");
    await page.fill('input[type="email"]', "admin@hypesoft.com");
    await page.fill('input[type="password"]', "admin123");
    await page.click('button[type="submit"]');
    await page.waitForURL("/dashboard", { timeout: 30000 });
    await page.goto("/products");
    await page.waitForLoadState("networkidle");
  });

  test.afterAll(async ({ browser }) => {
    const context = await browser.newContext();
    const page = await context.newPage();

    await page.goto("/login");
    await page.fill('input[type="email"]', "admin@hypesoft.com");
    await page.fill('input[type="password"]', "admin123");
    await page.click('button[type="submit"]');
    await page.waitForURL("/dashboard", { timeout: 30000 });

    for (const productName of createdProducts) {
      try {
        await deleteProduct(page, productName);
        console.log(`Cleaned up product: ${productName}`);
      } catch {
        console.log(`Failed to cleanup product: ${productName}`);
      }
    }

    await context.close();
  });

  test("should display products table", async ({ page }) => {
    await expect(page.locator("table")).toBeVisible();
    await expect(page.getByRole("columnheader", { name: /sku/i })).toBeVisible();
    await expect(page.getByRole("columnheader", { name: /name/i })).toBeVisible();
    await expect(
      page.getByRole("columnheader", { name: /price/i })
    ).toBeVisible();
    await expect(
      page.getByRole("columnheader", { name: /category/i })
    ).toBeVisible();
  });

  test("should open create product dialog and fill form", async ({ page }) => {
    const productName = `E2E Product ${Date.now()}`;

    await page.click("button:has(svg.lucide-plus)");
    await expect(page.locator('[role="dialog"]')).toBeVisible();

    await page.fill('input[name="name"]', productName);
    await page.fill(
      'textarea[name="description"]',
      "E2E test product description"
    );
    await page.fill('input[name="price"]', "99.99");
    await page.fill('input[name="stockQuantity"]', "50");

    // Wait for categories to load
    const combobox = page.locator('button[role="combobox"]');
    await expect(combobox).toBeEnabled({ timeout: 10000 });

    // Verify form is filled correctly
    await expect(page.locator('input[name="name"]')).toHaveValue(productName);
    await expect(page.locator('input[name="price"]')).toHaveValue("99.99");

    // Close dialog without submitting
    await page.keyboard.press("Escape");
    await expect(page.locator('[role="dialog"]')).not.toBeVisible({
      timeout: 5000,
    });
  });

  test("should edit a product and see the updated name in the table", async ({
    page,
  }) => {
    await expect(page.locator("table tbody tr").first()).toBeVisible();

    const actionsButton = page
      .locator("table tbody tr")
      .first()
      .locator("button")
      .last();
    await actionsButton.click();

    await page.click('[role="menuitem"]:has-text("Edit product")');
    await expect(page.locator('[role="dialog"]')).toBeVisible();

    const updatedName = `Updated Product ${Date.now()}`;
    createdProducts.push(updatedName);
    await page.fill('input[name="name"]', updatedName);
    await page.click('[role="dialog"] button[type="submit"]');

    await expect(page.locator('[role="dialog"]')).not.toBeVisible({
      timeout: 10000,
    });
    await expect(
      page.locator(`table tbody tr:has-text("${updatedName}")`)
    ).toBeVisible({ timeout: 10000 });
  });

  test("should open delete confirmation dialog", async ({ page }) => {
    const firstRow = page.locator("table tbody tr").first();
    await expect(firstRow).toBeVisible();

    await firstRow.locator("button").last().click();
    await page.click('[role="menuitem"]:has-text("Delete product")');

    await expect(page.locator('[role="dialog"]')).toBeVisible();
    await expect(
      page.locator('[role="dialog"] h2:has-text("Delete Product")')
    ).toBeVisible();

    // Cancel instead of actually deleting to preserve test data
    await page.click('[role="dialog"] button:has-text("Cancel")');
    await expect(page.locator('[role="dialog"]')).not.toBeVisible({
      timeout: 10000,
    });
  });

  test("should filter products by category", async ({ page }) => {
    await page.click('button:has-text("Category")');
    await page.click('[role="menuitem"]:not(:has-text("Clear")):first-child');
    await expect(page).toHaveURL(/categoryId=/);
    await page.waitForLoadState("networkidle");
    await expect(page.locator("table")).toBeVisible();
  });

  test("should search products by name", async ({ page }) => {
    await page.fill('input[placeholder*="Search"]', "test");
    await page.waitForTimeout(500);
    await expect(page.locator("table")).toBeVisible();
  });

  test("should paginate products", async ({ page }) => {
    // Pagination shows "X / Y" format
    const pagination = page.locator("text=/\\d+ \\/ \\d+/");
    await expect(pagination).toBeVisible();

    const nextButton = page
      .locator("button:has(svg.lucide-chevron-right)")
      .first();
    if (await nextButton.isEnabled()) {
      await nextButton.click();
      await expect(page).toHaveURL(/page=2/);
    }
  });

  test("should display total value column", async ({ page }) => {
    await expect(
      page.getByRole("columnheader", { name: /total value/i })
    ).toBeVisible();
  });
});
