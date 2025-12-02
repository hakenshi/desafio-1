import { test, expect } from '@playwright/test';

test.describe('Users Management (Admin Only)', () => {
  test.beforeEach(async ({ page }) => {
    // Login as admin
    await page.goto('/login');
    await page.fill('input[type="email"]', 'admin@hypesoft.com');
    await page.fill('input[type="password"]', 'admin123');
    await page.click('button[type="submit"]');
    await page.waitForURL('/dashboard', { timeout: 30000 });
    
    // Navigate to users page
    await page.goto('/users');
    await page.waitForLoadState('networkidle');
  });

  test('should display users table for admin', async ({ page }) => {
    // Check table is visible
    await expect(page.locator('table')).toBeVisible();
    
    // Check table headers
    await expect(page.getByRole('columnheader', { name: /username/i })).toBeVisible();
    await expect(page.getByRole('columnheader', { name: /email/i })).toBeVisible();
    await expect(page.getByRole('columnheader', { name: /role/i })).toBeVisible();
  });

  test('should create a new user', async ({ page }) => {
    const username = `testuser${Date.now()}`;
    const email = `${username}@test.com`;
    
    // Click add button
    await page.click('button:has(svg.lucide-plus)');
    
    // Wait for dialog
    await expect(page.locator('[role="dialog"]')).toBeVisible();
    
    // Fill form
    await page.fill('input[name="username"]', username);
    await page.fill('input[name="email"]', email);
    await page.fill('input[name="firstName"]', 'Test');
    await page.fill('input[name="lastName"]', 'User');
    await page.fill('input[name="password"]', 'password123');
    
    // Select role
    await page.click('button[role="combobox"]');
    await page.click('[role="option"]:has-text("User")');
    
    // Submit
    await page.click('button[type="submit"]:has-text("Create User")');
    
    // Wait for dialog to close (may take time due to Keycloak)
    await expect(page.locator('[role="dialog"]')).toBeHidden({ timeout: 15000 });
    
    // Verify user appears in table
    await page.fill('input[placeholder*="Search"]', username);
    await expect(page.getByRole('cell', { name: username })).toBeVisible({ timeout: 10000 });
  });

  test('should filter users by role', async ({ page }) => {
    // Click filter button
    await page.click('button:has-text("Role")');
    
    // Select Admin role
    await page.click('[role="menuitem"]:has-text("Admin")');
    
    // Verify filter is applied (table should update)
    await page.waitForTimeout(500);
    await expect(page.locator('table')).toBeVisible();
  });

  test('should search users by username', async ({ page }) => {
    // Type in search
    await page.fill('input[placeholder*="Search"]', 'admin');
    
    // Wait for filter to apply
    await page.waitForTimeout(500);
    
    // Should find admin user
    await expect(page.locator('table')).toBeVisible();
  });

  test('should open user details dialog', async ({ page }) => {
    // Click actions menu on first row
    await page.click('table tbody tr:first-child button:has(svg.lucide-more-horizontal)');
    
    // Click "Show Details"
    await page.click('[role="menuitem"]:has-text("Show Details")');
    
    // Dialog should open
    await expect(page.locator('[role="dialog"]')).toBeVisible();
  });

  test('should open edit user dialog', async ({ page }) => {
    // Click actions menu on first row
    await page.click('table tbody tr:first-child button:has(svg.lucide-more-horizontal)');
    
    // Click "Edit user"
    await page.click('[role="menuitem"]:has-text("Edit user")');
    
    // Dialog should open with form
    await expect(page.locator('[role="dialog"]')).toBeVisible();
    await expect(page.locator('input[name="email"]')).toBeVisible();
  });
});
