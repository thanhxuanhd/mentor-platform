import { Page, Locator } from "@playwright/test";
import { BasePage } from "../base-page";
import { PAGE_ENDPOINT_URL } from "../../core/constants/page-url";

export class PreferentsSetupPage extends BasePage {
    private DRD_TOPIC_LOCATOR: Locator;
    private BTN_TEACHINGAPPROACH_LOCATOR: Locator;
    private CBX_PRIVATEPROFILE_LOCATOR: Locator;
    private CBX_ALLOWMESSAGES_LOCATOR: Locator;
    private CBX_RECEIVENOTIFICATIONS_LOCATOR: Locator;
    private BTN_BACK_LOCATOR: Locator;
    private BTN_COMPLETEREGISTRATION_LOCATOR: Locator;

    constructor(page: Page) {
        super(page);
        this.DRD_TOPIC_LOCATOR = page.locator('#user_profile_form_categoryIds');
        this.BTN_TEACHINGAPPROACH_LOCATOR = page.locator("#user_profile_form_teachingApproachIds");
        this.CBX_PRIVATEPROFILE_LOCATOR = page.getByText("Private Profile");
        this.CBX_ALLOWMESSAGES_LOCATOR = page.getByText("Allow Messages");
        this.CBX_RECEIVENOTIFICATIONS_LOCATOR = page.getByText("Receive Notifications");
        this.BTN_BACK_LOCATOR = page.locator('//button[span[text()="Back"]]');
        this.BTN_COMPLETEREGISTRATION_LOCATOR = page.locator('//button[span[text()="Complete Registration"]]');
    }

    async navigateToHomePage(url = "") {
        await this.page.goto(url);
    }

    async selectTopics(topics: string[]) {
        for (const topic of topics) {
            await this.click(this.DRD_TOPIC_LOCATOR);
            await this.fill(this.DRD_TOPIC_LOCATOR, topic);
            await this.DRD_TOPIC_LOCATOR.press('Enter');
        }
    }

    async selectTeachingApproachByText(text: string): Promise<void> {
        const item = this.BTN_TEACHINGAPPROACH_LOCATOR.locator('span', {
            has: this.page.locator('div', { hasText: text }),
        });
        await this.click(item);
    }

    async selectMultipleTeachingApproaches(approaches: string[]): Promise<void> {
        for (const text of approaches) {
            await this.selectTeachingApproachByText(text);
        }
    }

    async checkOnPrivateProfileCheckbox(isCheck: boolean) {
        if (isCheck) {
            await this.CBX_PRIVATEPROFILE_LOCATOR.check();
        }
    }

    async checkOnAllowMessagesCheckbox(isCheck: boolean) {
        if (isCheck) {
            await this.CBX_ALLOWMESSAGES_LOCATOR.check();
        }
    }

    async checkOnReceiveNotificationsCheckbox(isCheck: boolean) {
        if (isCheck) {
            await this.CBX_RECEIVENOTIFICATIONS_LOCATOR.check();
        }
    }

    async clickBackButton() {
        await this.click(this.BTN_BACK_LOCATOR);
    }

    async clickCompleteRegistrationButton() {
        await this.click(this.BTN_COMPLETEREGISTRATION_LOCATOR);
    }

    async expectMessage(message: string) {
        const locator: Locator = this.page.getByText(message);
        await this.isVisible(locator);
    }
}
