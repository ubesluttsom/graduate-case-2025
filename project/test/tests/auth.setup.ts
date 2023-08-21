import { test as setup, expect } from '@playwright/test';

const authFile = 'playwright/.auth/user.json';
const loginPage =
  process.env.LOGIN_URL ||
  'https://agreeable-smoke-07383f303.3.azurestaticapps.net';
const loginUsername = process.env.LOGIN_USERNAME || 'tester@testing.com';
const loginPassword = process.env.LOGIN_PASSWORD || 'testing123';

setup('authenticate', async ({ page }) => {
  // Start of authentication steps.
  await page.goto(loginPage);
  // Wait for the login page to load.
  await page.waitForSelector('input[type="email"]');
  await page.fill('input[type="email"]', loginUsername);
  // Press next button.
  await page.click('input[type="submit"]');
  // Wait for the password page to load.
  await page.waitForSelector('input[name="passwd"]');
  await page.fill('input[name="passwd"]', loginPassword);
  // Press login button.
  await page.click('input[type="submit"]');

  // End of authentication steps.
  await page.context().storageState({ path: authFile });
});
