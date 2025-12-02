import { test, expect } from '@playwright/test';

test.describe('Categories CRUD', () => {
  test.beforeEach(async ({ page }) => {
    // Login first
    await page.goto('/login');
    await page.fill('input[type="email"]', 'admin@hypesoft.com');
    await page.fill('input[type="password"]', 'admin123');
    await page.click('button[type="submit"]');
    await page.waitForURL('/dashboard', { timeout: 30000 });
    
    // Navigate to categories page
    await page.goto('/categories');
    await page.waitForLoadState('networkidle');
  });

  test('should display categories table', async ({ page }) => {
    // Check table is visible
    await expect(page.locator('table')).toBeVisible();
    
    // Check table headers
    await expect(page.getByRole('columnheader', { name: /name/i })).toBeVisible();
    await expect(page.getByRole('columnheader', { name: /description/i })).toBeVisible();
  });

  test('should open create category dialog and fill form', async ({ page }) => {
    const categoryName = `Test Category ${Date.now()}`;
    
    // Click add button
    await page.click('button:has(svg.lucide-plus)');
    
    // Wait for dialog
    await expect(page.locator('[role="dialog"]')).toBeVisible();
    
    // Fill form
    await page.fill('input[name="name"]', categoryName);
    await page.fill('textarea[name="description"]', 'Test category description for E2E testing');
    
    // Verify form is filled
    await expect(page.locator('input[name="name"]')).toHaveValue(categoryName);
    
    // Close dialog
    await page.keyboard.press('Escape');
  });

  test('should search categories by name', async ({ page }) => {
    // Type in search
    await page.fill('input[placeholder*="Search"]', 'test');
    
    // Wait for filter to apply
    await page.waitForTimeout(500);
    
    // Table should still be visible
    await expect(page.locator('table')).toBeVisible();
  });

  test('should paginate categories', async ({ page }) => {
    // Check pagination is visible
    const pagination = page.locator('text=/Page \\d+ of \\d+/');
    await expect(pagination).toBeVisible();
  });
});
