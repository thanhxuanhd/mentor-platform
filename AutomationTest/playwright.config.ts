import { defineConfig, devices } from "@playwright/test";
import dotenv from "dotenv";
dotenv.config();

export default defineConfig({
  testDir: "./src/tests",
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: 0,
  workers: process.env.CI ? 1 : undefined,
  reporter: [["junit", { outputFile: "test-results/e2e-junit-results.xml" }]],
  use: {
    baseURL: "http://localhost:5174/",
    trace: "retain-on-failure",
    headless: false,
    testIdAttribute: "",
    actionTimeout: 10000,
    extraHTTPHeaders: {
      Accept: "application/json",
      Authorization: `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI3M2U0Mzc4NC1kNmI0LTQ2ZTAtYWViMC00NjJjNDY2YzQ2ZWMiLCJqdGkiOiJjMmVlNzc0Ni0zODVmLTQzYzctOGRhMy1hOTBkYjdjZDU1YWYiLCJlbWFpbCI6ImJla2ltY3VvbmcxN0BnbWFpbC5jb20iLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQWRtaW4iLCJhdWQiOiIqIiwiaXNzIjoiKiIsImV4cCI6MTc0Nzc3NDg0MywiaWF0IjoxNzQ3NzY0MDQzLCJuYmYiOjE3NDc3NjQwNDN9.-VuggVYo9YaIVfkkYqWr6sUIBy7jM_wc4cMnjLCC_6o`,
    },
  },

  projects: [
    {
      name: "chromium",
      use: { ...devices["Desktop Chrome"] },
    },
  ],
});
