import { test, expect } from '@playwright/test';

test.describe('Products CRUD', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/login');
    await page.fill('input[type="email"]', 'admin@hypesoft.com');
    await page.fill('input[type="password"]', 'admin123');
    await page.click('button[type="submit"]');
    await page.waitForURL('/dashboard', { timeout: 30000 });
    await page.goto('/products');
    await page.waitForLoadState('networkidle');
  });

  test('should display products table', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
    await expect(page.getByRole('columnheader', { name: /sku/i })).toBeVisible();
    await expect(page.getByRole('columnheader', { name: /name/i })).toBeVisible();
    await expect(page.getByRole('columnheader', { name: /price/i })).toBeVisible();
    await expect(page.getByRole('columnheader', { name: /category/i })).toBeVisible();
  });

  test('should open create product dialog and fill form', async ({ page }) => {
    const productName = `E2E Product ${Date.now()}`;
    
    await page.click('button:has(svg.lucide-plus)');
    await expect(page.locator('[role="dialog"]')).toBeVisible();
    
    await page.fill('input[name="name"]', productName);
    await page.fill('textarea[name="description"]', 'E2E test product description');
    await page.fill('input[name="price"]', '99.99');
    await page.fill('input[name="stockQuantity"]', '50');
    
    // Verify form is filled correctly
    await expect(page.locator('input[name="name"]')).toHaveValue(productName);
    await expect(page.locator('input[name="price"]')).toHaveValue('99.99');
    await expect(page.locator('input[name="stockQuantity"]')).toHaveValue('50');
    
    // Wait for categories to load and select one
    await page.waitForTimeout(1000);
    const combobox = page.locator('button[role="combobox"]');
    if (await combobox.isEnabled()) {
      await combobox.click();
      await page.click('[role="option"]:first-child');
    }
    
    // Verify submit button exists
    await expect(page.locator('[role="dialog"] button[type="submit"]')).toBeVisible();
    
    // Close dialog
    await page.keyboard.press('Escape');
  });

  test('should edit a product', async ({ page }) => {
    await expect(page.locator('table tbody tr').first()).toBeVisible();
    
    const actionsButton = page.locator('table tbody tr').first().locator('button').last();
    await actionsButton.click();
    
    await page.click('[role="menuitem"]:has-text("Edit product")');
    await expect(page.locator('[role="dialog"]')).toBeVisible();
    await expect(page.locator('[role="dialog"] h2:has-text("Edit Product")')).toBeVisible();
    
    const updatedName = `Updated Product ${Date.now()}`;
    await page.fill('input[name="name"]', updatedName);
    
    await page.click('[role="dialog"] button[type="submit"]');
    await expect(page.locator('[role="dialog"]')).not.toBeVisible({ timeout: 10000 });
  });

  test('should open delete confirmation dialog', async ({ page }) => {
    await expect(page.locator('table tbody tr').first()).toBeVisible();
    
    const actionsButton = page.locator('table tbody tr').first().locator('button').last();
    await actionsButton.click();
    
    await page.click('[role="menuitem"]:has-text("Delete product")');
    
    await expect(page.locator('[role="dialog"]')).toBeVisible();
    await expect(page.locator('[role="dialog"] h2:has-text("Delete Product")')).toBeVisible();
    await expect(page.locator('[role="dialog"]:has-text("Are you sure")')).toBeVisible();
    
    await expect(page.locator('[role="dialog"] button:has-text("Cancel")')).toBeVisible();
    await expect(page.locator('[role="dialog"] button:has-text("Delete")')).toBeVisible();
    
    await page.click('[role="dialog"] button:has-text("Cancel")');
    await expect(page.locator('[role="dialog"]')).not.toBeVisible();
  });

  test('should filter products by category', async ({ page }) => {
    await page.click('button:has-text("Category")');
    await page.click('[role="menuitem"]:not(:has-text("Clear")):first-child');
    await expect(page).toHaveURL(/categoryId=/);
    await page.waitForLoadState('networkidle');
    await expect(page.locator('table')).toBeVisible();
  });

  test('should search products by name', async ({ page }) => {
    await page.fill('input[placeholder*="Search"]', 'test');
    await page.waitForTimeout(500);
    await expect(page.locator('table')).toBeVisible();
  });

  test('should paginate products', async ({ page }) => {
    const pagination = page.locator('text=/Page \\d+ of \\d+/');
    await expect(pagination).toBeVisible();
    
    const nextButton = page.locator('button:has(svg.lucide-chevron-right)').first();
    if (await nextButton.isEnabled()) {
      await nextButton.click();
      await expect(page).toHaveURL(/page=2/);
    }
  });

  test('should display total value column', async ({ page }) => {
    await expect(page.getByRole('columnheader', { name: /total value/i })).toBeVisible();
  });
});
