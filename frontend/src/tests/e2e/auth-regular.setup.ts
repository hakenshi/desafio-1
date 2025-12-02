import { test as setup, expect } from '@playwright/test';

const authFile = 'src/tests/e2e/.auth/regular-user.json';

setup('authenticate as regular user', async ({ page }) => {
  // Go to login page
  await page.goto('/login');
  
  // Fill login form with regular user credentials
  await page.fill('input[type="email"]', 'user@hypesoft.com');
  await page.fill('input[type="password"]', 'user123');
  
  // Submit
  await page.click('button[type="submit"]');
  
  // Wait for redirect to dashboard
  await page.waitForURL('/dashboard', { timeout: 30000 });
  
  // Save auth state
  await page.context().storageState({ path: authFile });
});
