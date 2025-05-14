import { expect, Locator, Page } from "@playwright/test";

export class BasePage {
    protected page: Page;

    constructor(page: Page) {
        this.page = page;
    }

    async click(locator: Locator) {
        await locator.click();
        return this;
    }

    async fill(locator: Locator, text: string) {
        await locator.fill(text);
        return this;
    }

    async isVisible(locator: Locator) {
        await expect(locator).toBeVisible();
        return this;
    }

    async waitUntilVisible(locator: Locator, timeout = 5000) {
        await locator.waitFor({ state: "visible", timeout });
        return this;
    }

    async assertText(locator: Locator, expected: string) {
        await expect(locator).toHaveText(expected);
        return this;
    }

    async assertURL(expected: string) {
        await expect(this.page).toHaveURL(expected);
        return this;
    }
}