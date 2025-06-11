import { Page, Locator, expect } from "@playwright/test";
import { BasePage } from "../base-page";
import { PAGE_ENDPOINT_URL } from "../../core/constants/page-url";

export class SessionBookingPage extends BasePage {
    private LNK_SESSIONBOOKING_URL: string;
    private BTN_SELECTMENTOR_LOCATOR: Locator;
    private BTN_AVAILABLEMENTOR_LOCATOR: (name: string) => Locator;
    private BTN_CURRENTDAY_LOCATOR: Locator;
    private BTN_SELECTTIMESLOT_LOCATOR: Locator;
    private BTN_SELECTSESSIONTYPE_LOCATOR: (type: string) => Locator;
    private BTN_CONFIRMBOOKING_LOCATOR: Locator;

    constructor(page: Page) {
        super(page);
        this.LNK_SESSIONBOOKING_URL = PAGE_ENDPOINT_URL.SESSION_BOOKING;
        this.BTN_SELECTMENTOR_LOCATOR = page.locator('button:has(svg[data-icon="plus"])');
        this.BTN_AVAILABLEMENTOR_LOCATOR = (name: string): Locator => {
            return this.page.getByText(name);
        };
        this.BTN_CURRENTDAY_LOCATOR = page.locator("div.grid.grid-cols-7.gap-1 button:not([disabled])").first();
        this.BTN_SELECTTIMESLOT_LOCATOR = page.locator("div.grid.grid-cols-5.gap-3 button:not([disabled])").first();
        this.BTN_SELECTSESSIONTYPE_LOCATOR = (type: string): Locator => {
            return this.page.getByText(type);
        }
        this.BTN_CONFIRMBOOKING_LOCATOR = page.locator('//button[span[text()="Confirm booking"]]');
    }

    async navigateToHomePage(url = "") {
        await this.page.goto(url);
    }

    async goToSessionBookingPage() {
        await this.page.goto(this.LNK_SESSIONBOOKING_URL);
    }

    async clickSelectMentorButton() {
        await this.click(this.BTN_SELECTMENTOR_LOCATOR);
    }

    async selectAvailableMentor(name: string = 'Bob Builder') {
        await this.click(this.BTN_AVAILABLEMENTOR_LOCATOR(name));
    }

    async selectCurrentDay() {
        await this.click(this.BTN_CURRENTDAY_LOCATOR);
    }

    async selectTimeSlot() {
        await this.click(this.BTN_SELECTTIMESLOT_LOCATOR);
    }

    async selectSessionType(type: string = "Virtual") {
        await this.click(this.BTN_SELECTSESSIONTYPE_LOCATOR(type));
    }

    async clickConfirmBookingButton() {
        await this.click(this.BTN_CONFIRMBOOKING_LOCATOR);
    }
}