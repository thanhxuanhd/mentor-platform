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

    async isHidden(locator: Locator) {
        await expect(locator).toBeHidden();
        return this;
    }

    async isLocatorVisible(locator: Locator): Promise<boolean> {
        try {
            await locator.waitFor({ state: "visible", timeout: 5000 });
            return true;
        } catch {
            return false;
        }
    }

    async waitUntilVisible(locator: Locator, timeout = 10000) {
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

    async expectMessage(message: string) {
        if (message) {
            const locator: Locator = this.page.getByText(message);
            await this.isVisible(locator);
        }
    }

    async uploadFile(selector: string | Locator, filePath: string | string[]) {
        const input = typeof selector === 'string' ? this.page.locator(selector) : selector;
        await input.setInputFiles(filePath);
    }
}
