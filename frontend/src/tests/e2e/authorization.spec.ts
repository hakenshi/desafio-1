import { test, expect } from '@playwright/test';

// Test suite for authorization - uses a non-admin user
test.describe('Authorization - Access Control', () => {
  // Use a separate auth state for non-admin user
  test.use({ storageState: 'src/tests/e2e/.auth/regular-user.json' });

  test.beforeEach(async ({ page }) => {
    // Ensure we start from a known state
    await page.goto('/dashboard');
    await page.waitForLoadState('networkidle');
  });

  test('non-admin user should be redirected from /users to /dashboard', async ({ page }) => {
    // Try to access users page directly
    await page.goto('/users');
    
    // Should be redirected to dashboard
    await expect(page).toHaveURL('/dashboard');
    
    // Verify we're on the dashboard by checking the page title
    await expect(page.locator('h1, p.text-lg.font-semibold').first()).toContainText('Dashboard');
  });

  test('non-admin user should not see Users link in sidebar', async ({ page }) => {
    // Check that Users link is not visible in sidebar
    const usersLink = page.locator('nav a[href="/users"]');
    await expect(usersLink).not.toBeVisible();
  });

  test('non-admin user can access products page', async ({ page }) => {
    // Navigate to products
    await page.goto('/products');
    
    // Should stay on products page
    await expect(page).toHaveURL('/products');
    // Check page title in main content area
    await expect(page.locator('main p.text-lg.font-semibold, main h1').first()).toContainText('Products');
  });

  test('non-admin user can access categories page', async ({ page }) => {
    // Navigate to categories
    await page.goto('/categories');
    
    // Should stay on categories page
    await expect(page).toHaveURL('/categories');
    // Check page title in main content area
    await expect(page.locator('main p.text-lg.font-semibold, main h1').first()).toContainText('Categories');
  });

  test('non-admin user trying to access /users via URL manipulation should be blocked', async ({ page }) => {
    // Start from products page
    await page.goto('/products');
    await expect(page).toHaveURL('/products');
    
    // Try to navigate to users via URL
    await page.evaluate(() => {
      window.location.href = '/users';
    });
    
    // Wait for navigation and redirect
    await page.waitForLoadState('networkidle');
    
    // Should be redirected to dashboard
    await expect(page).toHaveURL('/dashboard');
  });
});

// Test suite for admin access - confirms admin CAN access restricted pages
test.describe('Authorization - Admin Access', () => {
  // Use admin auth state (default from auth.setup.ts)
  test.use({ storageState: 'src/tests/e2e/.auth/user.json' });

  test('admin user can access /users page', async ({ page }) => {
    await page.goto('/users');
    
    // Should stay on users page
    await expect(page).toHaveURL('/users');
    // Check page title in main content area
    await expect(page.locator('main p.text-lg.font-semibold, main h1').first()).toContainText('Users');
  });

  test('admin user should see Users link in sidebar', async ({ page }) => {
    await page.goto('/dashboard');
    
    // Check that Users link is visible in sidebar
    const usersLink = page.locator('nav a[href="/users"]');
    await expect(usersLink).toBeVisible();
  });
});
