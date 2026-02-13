// @ts-check
const { test, expect } = require('@playwright/test');

test.describe('Finance Tracker Health Check', () => {
  test('should display PostgreSQL version on index page', async ({ page }) => {
    // Navigate to the page
    await page.goto('/');

    // Wait for the page to load
    await page.waitForLoadState('networkidle');

    // Check that the page has the correct title
    await expect(page.locator('h1')).toContainText('Finance Tracker');

    // Click the health check button
    const checkButton = page.locator('button:has-text("Check Health")');
    await expect(checkButton).toBeVisible();
    
    // Wait a bit for initial health check (runs on mount)
    await page.waitForTimeout(2000);

    // Check if health result is visible
    const healthResult = page.locator('.health-result.success');
    await expect(healthResult).toBeVisible();

    // Verify the status is "Healthy"
    const statusText = healthResult.locator('p:has-text("Status:")');
    await expect(statusText).toContainText('Healthy');

    // Verify that database version is displayed
    const dbVersionText = healthResult.locator('p:has-text("Database Version:")');
    await expect(dbVersionText).toBeVisible();
    
    // Check that the database version contains "PostgreSQL" text
    const dbVersion = await dbVersionText.textContent();
    expect(dbVersion).toMatch(/PostgreSQL/i);
  });

  test('should display timestamp on health check', async ({ page }) => {
    // Navigate to the page
    await page.goto('/');

    // Wait for the page to load
    await page.waitForLoadState('networkidle');

    // Wait for initial health check
    await page.waitForTimeout(2000);

    // Check if health result is visible
    const healthResult = page.locator('.health-result.success');
    await expect(healthResult).toBeVisible();

    // Verify that timestamp is displayed
    const timestampText = healthResult.locator('p:has-text("Timestamp:")');
    await expect(timestampText).toBeVisible();
  });
});
