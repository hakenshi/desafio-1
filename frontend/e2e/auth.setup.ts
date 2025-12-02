import { test as setup, expect } from '@playwright/test';

const authFile = 'e2e/.auth/user.json';

setup('authenticate', async ({ page }) => {
  // Go to login page
  await page.goto('/login');
  
  // Fill login form (using default credentials)
  await page.fill('input[type="email"]', 'admin@hypesoft.com');
  await page.fill('input[type="password"]', 'admin123');
  
  // Submit
  await page.click('button[type="submit"]');
  
  // Wait for redirect to dashboard
  await page.waitForURL('/dashboard', { timeout: 30000 });
  
  // Save auth state
  await page.context().storageState({ path: authFile });
});
