import { test, expect } from '@playwright/test';

test.describe('Products CRUD', () => {
  test.beforeEach(async ({ page }) => {
    // Login first
    await page.goto('/login');
    await page.fill('input[type="email"]', 'admin@hypesoft.com');
    await page.fill('input[type="password"]', 'admin123');
    await page.click('button[type="submit"]');
    await page.waitForURL('/dashboard', { timeout: 30000 });
    
    // Navigate to products page
    await page.goto('/products');
    await page.waitForLoadState('networkidle');
  });

  test('should display products table', async ({ page }) => {
    // Check table is visible
    await expect(page.locator('table')).toBeVisible();
    
    // Check table headers
    await expect(page.getByRole('columnheader', { name: /name/i })).toBeVisible();
    await expect(page.getByRole('columnheader', { name: /price/i })).toBeVisible();
    await expect(page.getByRole('columnheader', { name: /category/i })).toBeVisible();
  });

  test('should open create product dialog and fill form', async ({ page }) => {
    const productName = `Test Product ${Date.now()}`;
    
    // Click add button
    await page.click('button:has(svg.lucide-plus)');
    
    // Wait for dialog
    await expect(page.locator('[role="dialog"]')).toBeVisible();
    
    // Fill form
    await page.fill('input[name="name"]', productName);
    await page.fill('textarea[name="description"]', 'Test product description for E2E testing');
    await page.fill('input[name="price"]', '99.99');
    await page.fill('input[name="stockQuantity"]', '50');
    
    // Select category (click the select trigger first)
    await page.click('button[role="combobox"]');
    await page.click('[role="option"]:first-child');
    
    // Verify form is filled
    await expect(page.locator('input[name="name"]')).toHaveValue(productName);
    await expect(page.locator('input[name="price"]')).toHaveValue('99.99');
    
    // Close dialog by clicking outside or pressing escape
    await page.keyboard.press('Escape');
  });

  test('should filter products by category', async ({ page }) => {
    // Click filter button
    await page.click('button:has-text("Category")');
    
    // Select first category option
    await page.click('[role="menuitem"]:not(:has-text("Clear"))');
    
    // Wait for URL to update with filter
    await expect(page).toHaveURL(/categoryId=/);
    
    // Verify table updates (should show filtered results)
    await page.waitForLoadState('networkidle');
    await expect(page.locator('table')).toBeVisible();
  });

  test('should search products by name', async ({ page }) => {
    // Type in search
    await page.fill('input[placeholder*="Search"]', 'test');
    
    // Wait for filter to apply
    await page.waitForTimeout(500);
    
    // Table should still be visible
    await expect(page.locator('table')).toBeVisible();
  });

  test('should paginate products', async ({ page }) => {
    // Check pagination is visible
    const pagination = page.locator('text=/Page \\d+ of \\d+/');
    await expect(pagination).toBeVisible();
    
    // Click next page if available
    const nextButton = page.locator('button:has(svg.lucide-chevron-right)').first();
    if (await nextButton.isEnabled()) {
      await nextButton.click();
      await expect(page).toHaveURL(/page=2/);
    }
  });
});
