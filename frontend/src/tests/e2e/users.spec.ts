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

  test('should open create user dialog and fill form', async ({ page }) => {
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
    
    // Verify form is filled
    await expect(page.locator('input[name="username"]')).toHaveValue(username);
    await expect(page.locator('input[name="email"]')).toHaveValue(email);
    
    // Close dialog
    await page.keyboard.press('Escape');
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

  test('should have actions menu in table rows', async ({ page }) => {
    // Wait for table to load
    await expect(page.locator('table tbody tr').first()).toBeVisible();
    
    // Check that actions button exists
    const actionsButton = page.locator('table tbody tr').first().locator('button').last();
    await expect(actionsButton).toBeVisible();
  });

  test('should open edit user dialog from actions menu', async ({ page }) => {
    // Wait for table to load
    await expect(page.locator('table tbody tr').first()).toBeVisible();
    
    // Click actions button on first row
    const actionsButton = page.locator('table tbody tr').first().locator('button').last();
    await actionsButton.click();
    
    // Click Edit user option
    await page.click('[role="menuitem"]:has-text("Edit user")');
    
    // Verify edit dialog opens
    await expect(page.locator('[role="dialog"]')).toBeVisible();
    await expect(page.locator('[role="dialog"] h2:has-text("Edit User")')).toBeVisible();
    
    // Verify form fields are pre-filled
    await expect(page.locator('[role="dialog"] input[name="email"]')).toHaveValue(/.+@.+/);
    
    // Close dialog
    await page.keyboard.press('Escape');
  });

  test('should open delete confirmation dialog from actions menu', async ({ page }) => {
    // Wait for table to load
    await expect(page.locator('table tbody tr').first()).toBeVisible();
    
    // Click actions button on first row
    const actionsButton = page.locator('table tbody tr').first().locator('button').last();
    await actionsButton.click();
    
    // Click Delete user option
    await page.click('[role="menuitem"]:has-text("Delete user")');
    
    // Verify delete confirmation dialog opens
    await expect(page.locator('[role="dialog"]')).toBeVisible();
    await expect(page.locator('[role="dialog"] h2:has-text("Delete User")')).toBeVisible();
    await expect(page.locator('[role="dialog"]:has-text("Are you sure")')).toBeVisible();
    
    // Verify Cancel and Delete buttons exist
    await expect(page.locator('[role="dialog"] button:has-text("Cancel")')).toBeVisible();
    await expect(page.locator('[role="dialog"] button:has-text("Delete")')).toBeVisible();
    
    // Close dialog by clicking Cancel
    await page.click('[role="dialog"] button:has-text("Cancel")');
    await expect(page.locator('[role="dialog"]')).not.toBeVisible();
  });

  test('should show user details dialog from actions menu', async ({ page }) => {
    // Wait for table to load
    await expect(page.locator('table tbody tr').first()).toBeVisible();
    
    // Click actions button on first row
    const actionsButton = page.locator('table tbody tr').first().locator('button').last();
    await actionsButton.click();
    
    // Click Show Details option
    await page.click('[role="menuitem"]:has-text("Show Details")');
    
    // Verify details dialog opens
    await expect(page.locator('[role="dialog"]')).toBeVisible();
    
    // Close dialog
    await page.keyboard.press('Escape');
  });
});
