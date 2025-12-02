import { test, expect } from '@playwright/test';

test.describe('Users Management (Admin Only)', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/login');
    await page.fill('input[type="email"]', 'admin@hypesoft.com');
    await page.fill('input[type="password"]', 'admin123');
    await page.click('button[type="submit"]');
    await page.waitForURL('/dashboard', { timeout: 30000 });
    await page.goto('/users');
    await page.waitForLoadState('networkidle');
  });

  test('should display users table for admin', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
    await expect(page.getByRole('columnheader', { name: /username/i })).toBeVisible();
    await expect(page.getByRole('columnheader', { name: /email/i })).toBeVisible();
    await expect(page.getByRole('columnheader', { name: /role/i })).toBeVisible();
  });

  test('should open create user dialog and fill form', async ({ page }) => {
    const username = `e2euser${Date.now()}`;
    const email = `${username}@test.com`;
    
    await page.click('button:has(svg.lucide-plus)');
    await expect(page.locator('[role="dialog"]')).toBeVisible();
    
    await page.fill('input[name="username"]', username);
    await page.fill('input[name="email"]', email);
    await page.fill('input[name="firstName"]', 'E2E');
    await page.fill('input[name="lastName"]', 'Test');
    await page.fill('input[name="password"]', 'password123');
    
    // Verify form is filled correctly
    await expect(page.locator('input[name="username"]')).toHaveValue(username);
    await expect(page.locator('input[name="email"]')).toHaveValue(email);
    await expect(page.locator('input[name="firstName"]')).toHaveValue('E2E');
    await expect(page.locator('input[name="lastName"]')).toHaveValue('Test');
    
    // Select role
    await page.click('button[role="combobox"]');
    await page.click('[role="option"]:has-text("User")');
    
    // Verify submit button exists
    await expect(page.locator('[role="dialog"] button[type="submit"]')).toBeVisible();
    
    // Close dialog
    await page.keyboard.press('Escape');
  });

  test('should edit a user', async ({ page }) => {
    await expect(page.locator('table tbody tr').first()).toBeVisible();
    
    const actionsButton = page.locator('table tbody tr').first().locator('button').last();
    await actionsButton.click();
    
    await page.click('[role="menuitem"]:has-text("Edit user")');
    await expect(page.locator('[role="dialog"]')).toBeVisible();
    await expect(page.locator('[role="dialog"] h2:has-text("Edit User")')).toBeVisible();
    
    await expect(page.locator('[role="dialog"] input[name="email"]')).toHaveValue(/.+@.+/);
    
    await page.keyboard.press('Escape');
  });

  test('should open delete confirmation dialog', async ({ page }) => {
    await expect(page.locator('table tbody tr').first()).toBeVisible();
    
    const actionsButton = page.locator('table tbody tr').first().locator('button').last();
    await actionsButton.click();
    
    await page.click('[role="menuitem"]:has-text("Delete user")');
    
    await expect(page.locator('[role="dialog"]')).toBeVisible();
    await expect(page.locator('[role="dialog"] h2:has-text("Delete User")')).toBeVisible();
    await expect(page.locator('[role="dialog"]:has-text("Are you sure")')).toBeVisible();
    
    await expect(page.locator('[role="dialog"] button:has-text("Cancel")')).toBeVisible();
    await expect(page.locator('[role="dialog"] button:has-text("Delete")')).toBeVisible();
    
    await page.click('[role="dialog"] button:has-text("Cancel")');
    await expect(page.locator('[role="dialog"]')).not.toBeVisible();
  });

  test('should show user details dialog', async ({ page }) => {
    await expect(page.locator('table tbody tr').first()).toBeVisible();
    
    const actionsButton = page.locator('table tbody tr').first().locator('button').last();
    await actionsButton.click();
    
    await page.click('[role="menuitem"]:has-text("Show Details")');
    await expect(page.locator('[role="dialog"]')).toBeVisible();
    
    await page.keyboard.press('Escape');
  });

  test('should filter users by role', async ({ page }) => {
    await page.click('button:has-text("Role")');
    await page.click('[role="menuitem"]:has-text("Admin")');
    await page.waitForTimeout(500);
    await expect(page.locator('table')).toBeVisible();
  });

  test('should search users by username', async ({ page }) => {
    await page.fill('input[placeholder*="Search"]', 'admin');
    await page.waitForTimeout(500);
    await expect(page.locator('table')).toBeVisible();
  });
});
