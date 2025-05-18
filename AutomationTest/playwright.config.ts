import { defineConfig, devices } from '@playwright/test';
import dotenv from 'dotenv';
dotenv.config();

export default defineConfig({
  testDir: './src/tests',
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : undefined,
  reporter: [['junit', { outputFile: 'test-results/e2e-junit-results.xml' }]],
  use: {
    baseURL: 'http://localhost:5173',
    trace: 'retain-on-failure',
    headless: false,
    testIdAttribute: '',
    actionTimeout: 10000,
    extraHTTPHeaders: {
      'Accept': 'application/json'
    }

  },

  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
  ],

});