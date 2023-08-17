import { test, expect } from '@playwright/test';

const baseURL =
  process.env.API_BASE_URL || 'https://official-joke-api.appspot.com';

test('GET Joke is successfull', async ({ request }) => {
  const joke = await request.get(`${baseURL}/random_joke`);
  expect(joke.status()).toBe(200);
});
