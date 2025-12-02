import { test, expect } from '@playwright/test';

test.describe('Logout', () => {
  test.beforeEach(async ({ page }) => {
    // Login first
    await page.goto('/login');
    await page.fill('input[type="email"]', 'admin@hypesoft.com');
    await page.fill('input[type="password"]', 'admin123');
    await page.click('button[type="submit"]');
    await page.waitForURL('/dashboard', { timeout: 30000 });
  });

  test('should logout and redirect to login page', async ({ page }) => {
    // Click on the dropdown menu (ellipsis icon)
    await page.click('button:has(svg.lucide-ellipsis)');
    
    // Wait for dropdown to appear
    await expect(page.locator('[role="menuitem"]:has-text("Logout")')).toBeVisible();
    
    // Click logout
    await page.click('[role="menuitem"]:has-text("Logout")');
    
    // Should redirect to login page
    await page.waitForURL('/login', { timeout: 10000 });
    await expect(page).toHaveURL('/login');
  });

  test('should not be able to access dashboard after logout', async ({ page }) => {
    // Logout
    await page.click('button:has(svg.lucide-ellipsis)');
    await page.click('[role="menuitem"]:has-text("Logout")');
    await page.waitForURL('/login', { timeout: 10000 });
    
    // Try to access dashboard directly
    await page.goto('/dashboard');
    
    // Should be redirected to login
    await expect(page).toHaveURL('/login');
  });

  test('should show user details dialog from dropdown', async ({ page }) => {
    // Click on the dropdown menu
    await page.click('button:has(svg.lucide-ellipsis)');
    
    // Click User Details
    await page.click('[role="menuitem"]:has-text("User Details")');
    
    // Dialog should appear
    await expect(page.locator('[role="dialog"]')).toBeVisible();
    
    // Close dialog
    await page.keyboard.press('Escape');
  });
});
