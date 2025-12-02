import { test, expect } from '@playwright/test';

test.describe('Categories CRUD', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/login');
    await page.fill('input[type="email"]', 'admin@hypesoft.com');
    await page.fill('input[type="password"]', 'admin123');
    await page.click('button[type="submit"]');
    await page.waitForURL('/dashboard', { timeout: 30000 });
    await page.goto('/categories');
    await page.waitForLoadState('networkidle');
  });

  test('should display categories table', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
    await expect(page.getByRole('columnheader', { name: /name/i })).toBeVisible();
    await expect(page.getByRole('columnheader', { name: /description/i })).toBeVisible();
  });

  test('should open create category dialog and fill form', async ({ page }) => {
    const categoryName = `E2E Category ${Date.now()}`;
    
    await page.click('button:has(svg.lucide-plus)');
    await expect(page.locator('[role="dialog"]')).toBeVisible();
    
    await page.fill('input[name="name"]', categoryName);
    await page.fill('textarea[name="description"]', 'E2E test category description');
    
    // Verify form is filled correctly
    await expect(page.locator('input[name="name"]')).toHaveValue(categoryName);
    await expect(page.locator('textarea[name="description"]')).toHaveValue('E2E test category description');
    
    // Verify submit button exists
    await expect(page.locator('[role="dialog"] button[type="submit"]')).toBeVisible();
    
    // Close dialog
    await page.keyboard.press('Escape');
  });

  test('should edit a category', async ({ page }) => {
    await expect(page.locator('table tbody tr').first()).toBeVisible();
    
    const actionsButton = page.locator('table tbody tr').first().locator('button').last();
    await actionsButton.click();
    
    await page.click('[role="menuitem"]:has-text("Edit category")');
    await expect(page.locator('[role="dialog"]')).toBeVisible();
    await expect(page.locator('[role="dialog"] h2:has-text("Edit Category")')).toBeVisible();
    
    const updatedName = `Updated Category ${Date.now()}`;
    await page.fill('input[name="name"]', updatedName);
    
    await page.click('[role="dialog"] button[type="submit"]');
    await expect(page.locator('[role="dialog"]')).not.toBeVisible({ timeout: 10000 });
  });

  test('should open delete confirmation dialog', async ({ page }) => {
    await expect(page.locator('table tbody tr').first()).toBeVisible();
    
    const actionsButton = page.locator('table tbody tr').first().locator('button').last();
    await actionsButton.click();
    
    await page.click('[role="menuitem"]:has-text("Delete category")');
    
    await expect(page.locator('[role="dialog"]')).toBeVisible();
    await expect(page.locator('[role="dialog"] h2:has-text("Delete Category")')).toBeVisible();
    await expect(page.locator('[role="dialog"]:has-text("Are you sure")')).toBeVisible();
    
    await expect(page.locator('[role="dialog"] button:has-text("Cancel")')).toBeVisible();
    await expect(page.locator('[role="dialog"] button:has-text("Delete")')).toBeVisible();
    
    await page.click('[role="dialog"] button:has-text("Cancel")');
    await expect(page.locator('[role="dialog"]')).not.toBeVisible();
  });

  test('should search categories by name', async ({ page }) => {
    await page.fill('input[placeholder*="Search"]', 'test');
    await page.waitForTimeout(500);
    await expect(page.locator('table')).toBeVisible();
  });

  test('should paginate categories', async ({ page }) => {
    const pagination = page.locator('text=/Page \\d+ of \\d+/');
    await expect(pagination).toBeVisible();
  });
});
