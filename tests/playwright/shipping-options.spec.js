const path = require('node:path');
const { execFileSync } = require('node:child_process');
const { test, expect } = require('@playwright/test');

const orderId = '12';
const dbHarnessDllPath = path.resolve(
  __dirname,
  '..',
  'ShippingOptionDbHarness',
  'bin',
  'Debug',
  'net9.0',
  'ShippingOptionDbHarness.dll'
);

function runDbProbe(commandName, selectedOrderId) {
  return execFileSync(
    'dotnet',
    [
      dbHarnessDllPath,
      commandName,
      selectedOrderId,
    ],
    {
    encoding: 'utf8',
    env: {
      ...process.env,
      PRORENTAL_CONNECTION_STRING:
        process.env.PRORENTAL_CONNECTION_STRING ||
        'Host=localhost;Port=5432;Database=pro_rental;Username=postgres;Password=password',
    },
    }
  ).trim();
}

test.beforeEach(() => {
  runDbProbe('reset-order', orderId);
});

test.afterEach(() => {
  runDbProbe('reset-order', orderId);
});

test('customer can compare and select shipping options for seeded order 12', async ({ page }) => {
  expect(runDbProbe('get-selected-option', orderId)).toBe('NULL');

  await page.goto('/');
  await page.getByLabel('Feature 1 Shipping Options').fill(orderId);
  await page.getByRole('button', { name: 'Open' }).click();

  await expect(page.getByRole('heading', { name: 'Choose a shipping option' })).toBeVisible();
  await expect(page.getByRole('heading', { name: 'Fastest' })).toBeVisible();
  await expect(page.getByRole('heading', { name: 'Cheapest' })).toBeVisible();
  await expect(page.getByRole('heading', { name: 'Greenest' })).toBeVisible();
  await expect(page.getByRole('button', { name: 'Select option' })).toHaveCount(3);

  await page.getByRole('link', { name: 'Compare options' }).click();

  await expect(page.getByRole('heading', { name: 'Compare shipping options' })).toBeVisible();
  await expect(page.getByRole('row', { name: /FAST Fastest/i })).toBeVisible();
  await expect(page.getByRole('row', { name: /CHEAP Cheapest/i })).toBeVisible();
  await expect(page.getByRole('row', { name: /GREEN Greenest/i })).toBeVisible();

  await page.goto(`/ShippingOptions/GetShippingOptions?orderId=${orderId}`);
  await page.getByRole('button', { name: 'Select option' }).first().click();

  await expect(page.getByRole('heading', { name: 'Shipping option confirmed' })).toBeVisible();

  const details = await page.locator('dd').allTextContents();
  const selectedOptionId = details[1].trim();

  expect(runDbProbe('get-selected-option', orderId)).toBe(selectedOptionId);
});
