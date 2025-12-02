import { test, expect } from "@playwright/test";

test.describe("Settings Page", () => {
  test.beforeEach(async ({ page }) => {
    // Login as admin
    await page.goto("/login");
    await page.fill('input[type="email"]', "admin@hypesoft.com");
    await page.fill('input[type="password"]', "admin123");
    await page.click('button[type="submit"]');
    await page.waitForURL("/dashboard", { timeout: 30000 });
  });

  test("should navigate to settings and display cards", async ({ page }) => {
    await page.goto("/settings");

    // Wait for page to fully load
    await page.waitForTimeout(2000);

    // Check that we're on settings page
    await expect(page).toHaveURL("/settings");

    // Check for any card content (more flexible)
    const profileCard = page.getByText("Profile Information");
    const appearanceCard = page.getByText("Appearance");
    const accountCard = page.getByText("Account Information");

    // At least one should be visible
    const anyVisible = await Promise.race([
      profileCard.isVisible().catch(() => false),
      appearanceCard.isVisible().catch(() => false),
      accountCard.isVisible().catch(() => false),
    ]);

    expect(anyVisible).toBeTruthy();
  });

  test("should show email input with current user email", async ({ page }) => {
    await page.goto("/settings");
    await page.waitForTimeout(2000);

    // Look for email input
    const emailInput = page.locator('input[type="email"]');
    await expect(emailInput).toBeVisible({ timeout: 10000 });

    // Should have admin email
    await expect(emailInput).toHaveValue("admin@hypesoft.com");
  });

  test("should be accessible via sidebar", async ({ page }) => {
    // Start from dashboard
    await page.goto("/dashboard");
    await page.waitForTimeout(1000);

    // Click settings link
    const settingsLink = page.locator('a[href="/settings"]');
    await settingsLink.click();

    // Should navigate
    await expect(page).toHaveURL("/settings");
  });
});
