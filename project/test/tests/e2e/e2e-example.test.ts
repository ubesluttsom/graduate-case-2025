import { test, expect } from '@playwright/test';

const baseURL = process.env.BASE_URL || 'https://playwright.dev';

test('has title', async ({ page }) => {
  await page.goto(baseURL);

  // Expect a title "to contain" a substring.
  await expect(page).toHaveTitle(/Graduate/);
});
