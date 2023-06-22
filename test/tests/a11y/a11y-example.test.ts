import { expect, test } from '@playwright/test';
import AxeBuilder from '@axe-core/playwright';

const baseURL = process.env.BASE_URL || 'https://playwright.dev';

test('Accessibility test', async ({ page }) => {
  await page.goto(baseURL);

  const accessibilityScanResults = await new AxeBuilder({ page })
    .withTags(['wcag2a'])
    .analyze();
  expect(accessibilityScanResults.violations).toEqual([]);
});
