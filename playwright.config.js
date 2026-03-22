const { defineConfig } = require('@playwright/test');

const baseURL = process.env.PLAYWRIGHT_BASE_URL || 'https://127.0.0.1:5091';
const connectionString =
  process.env.PRORENTAL_CONNECTION_STRING ||
  'Host=localhost;Port=5432;Database=pro_rental;Username=postgres;Password=password';

module.exports = defineConfig({
  testDir: './tests/playwright',
  fullyParallel: false,
  retries: 0,
  timeout: 120000,
  use: {
    baseURL,
    browserName: 'chromium',
    headless: true,
    ignoreHTTPSErrors: true,
    trace: 'retain-on-failure',
  },
  webServer: {
    command: 'dotnet run --project ProRental.csproj --no-build',
    url: baseURL,
    ignoreHTTPSErrors: true,
    reuseExistingServer: true,
    timeout: 120000,
    env: {
      ASPNETCORE_URLS: baseURL,
      ConnectionStrings__Default: connectionString,
    },
  },
});
