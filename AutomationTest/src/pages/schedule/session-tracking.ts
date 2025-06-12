import { Page, Locator, expect } from "@playwright/test";
import { BasePage } from "../base-page";
import { PAGE_ENDPOINT_URL } from "../../core/constants/page-url";

export class SessionTrackingPage extends BasePage {
    private LNK_SESSIONTRACKING_URL: string;
    private BTN_ACCEPT_LOCATOR: Locator;

    constructor(page: Page) {
        super(page);
        this.LNK_SESSIONTRACKING_URL = PAGE_ENDPOINT_URL.SESSION_TRACKING;
        this.BTN_ACCEPT_LOCATOR = page.locator('button:has-text("Accept")').first();
    }

    async navigateToHomePage(url = "") {
        await this.page.goto(url);
    }

    async goToSessionTrackingPage() {
        await this.page.goto(this.LNK_SESSIONTRACKING_URL);
    }

    async clickAcceptButton(){
        await this.click(this.BTN_ACCEPT_LOCATOR);
    }
}