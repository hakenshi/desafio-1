import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  testDir: './src/tests/e2e',
  fullyParallel: false,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: 1,
  reporter: 'html',
  use: {
    baseURL: 'http://localhost:3000',
    trace: 'on-first-retry',
    screenshot: 'only-on-failure',
  },
  projects: [
    // Setup projects for authentication
    {
      name: 'setup-admin',
      testMatch: /auth\.setup\.ts/,
    },
    {
      name: 'setup-regular-user',
      testMatch: /auth-regular\.setup\.ts/,
    },
    // Main test project using admin auth
    {
      name: 'chromium',
      use: { 
        ...devices['Desktop Chrome'],
        storageState: 'src/tests/e2e/.auth/user.json',
      },
      dependencies: ['setup-admin'],
      testIgnore: /authorization\.spec\.ts/,
    },
    // Authorization tests (handles its own auth states)
    {
      name: 'authorization-tests',
      use: { ...devices['Desktop Chrome'] },
      dependencies: ['setup-admin', 'setup-regular-user'],
      testMatch: /authorization\.spec\.ts/,
    },
  ],
  webServer: {
    command: 'bun run dev',
    url: 'http://localhost:3000',
    reuseExistingServer: !process.env.CI,
    timeout: 120000,
  },
});
