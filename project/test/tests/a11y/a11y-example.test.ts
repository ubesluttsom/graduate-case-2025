import AxeBuilder from '@axe-core/playwright';
import { expect, test } from '@playwright/test';

const baseURL =
  process.env.BASE_URL ||
  'https://agreeable-smoke-07383f303.3.azurestaticapps.net';

test('Accessibility test', async ({ page }) => {
  await page.goto(baseURL);

  const accessibilityScanResults = await new AxeBuilder({ page })
    .withTags(['wcag2a'])
    .analyze();
  expect(accessibilityScanResults.violations).toEqual([]);
});
