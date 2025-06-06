import { Page, Locator, expect } from "@playwright/test";
import { BasePage } from "../base-page";
import { PAGE_ENDPOINT_URL } from "../../core/constants/page-url";

export class AvailabilityPage extends BasePage {
    private LNK_AVAILABILITY_URL: string;
    private BTN_TODAY_LOCATOR: Locator;
    public DRD_STARTTIME_LOCATOR: Locator;
    public DRD_ENDTIME_LOCATOR: Locator;
    private DRD_DURATION_LOCATOR: Locator;
    private DRD_BUFFERTIME_LOCATOR: Locator;
    private BTN_SELECTALL_LOCATOR: Locator;
    private BTN_SAVECHANGES_LOCATOR: Locator;
    private TBL_FIRSTTIMESLOT_LOCATOR: Locator;


    constructor(page: Page) {
        super(page);
        this.LNK_AVAILABILITY_URL = PAGE_ENDPOINT_URL.AVAILABILITY;
        this.BTN_TODAY_LOCATOR = page.locator('//button[span[text()="Today"]]');
        this.DRD_STARTTIME_LOCATOR = page.locator("label:has-text('Start time')")
            .locator("..").locator("..")
            .locator(".ant-select-selector");
        this.DRD_ENDTIME_LOCATOR = page.locator("label:has-text('End time')")
            .locator("..").locator("..")
            .locator(".ant-select-selector");
        this.DRD_DURATION_LOCATOR = page.locator("label:has-text('Session duration')")
            .locator("..").locator("..")
            .locator(".ant-select-selector");
        this.DRD_BUFFERTIME_LOCATOR = page.locator("label:has-text('Buffer time')")
            .locator("..").locator("..")
            .locator(".ant-select-selector");
        this.BTN_SELECTALL_LOCATOR = page.locator('button:has-text("Select all slots for")');
        this.BTN_SAVECHANGES_LOCATOR = page.locator('//button[span[text()="Save Changes"]]');
        this.TBL_FIRSTTIMESLOT_LOCATOR = page.locator("#time-blocks").first();
    }

    async navigateToHomePage(url = "") {
        await this.page.goto(url);
    }

    async goToAvailabilityManagementPage() {
        await this.page.goto(this.LNK_AVAILABILITY_URL);
    }

    async clickTodayButton() {
        await this.click(this.BTN_TODAY_LOCATOR);
    }

    async selectStartTime(targetTime: string) {
        await this.selectTimeByKeyboard(this.DRD_STARTTIME_LOCATOR, targetTime);
    }

    async selectEndTime(targetTime: string) {
        await this.selectTimeByKeyboard(this.DRD_ENDTIME_LOCATOR, targetTime);
    }

    async selectDuration(targetTime: string) {
        await this.selectAntDropdownByKeyboard(this.DRD_DURATION_LOCATOR, targetTime);
    }

    async selectBufferTime(targetTime: string) {
        await this.selectAntDropdownByKeyboard(this.DRD_BUFFERTIME_LOCATOR, targetTime);
    }

    async selectTimeByKeyboard(dropdownLocator: Locator, targetTime: string) {
        await this.click(dropdownLocator);
        const dropdown = this.page.locator('.ant-select-dropdown:not(.ant-select-dropdown-hidden)');
        const firstOption = dropdown.locator('.ant-select-item-option').first();
        await firstOption.hover();
        await this.page.waitForTimeout(100);
        for (let i = 0; i < 48; i++) {
            const highlighted = dropdown.locator('.ant-select-item-option-active');
            const value = await highlighted.first().getAttribute('title');
            if (value === targetTime) {
                await this.page.keyboard.press('Enter');
                await this.page.waitForSelector('.ant-select-dropdown:not(.ant-select-dropdown-hidden)', { state: 'detached' });
                return;
            }
            await this.page.keyboard.press('ArrowDown');
            await this.page.waitForTimeout(50);
        }
    }

    async selectAntDropdownByKeyboard(triggerLocator: Locator, targetText: string) {
        await this.click(triggerLocator);
        const dropdown = this.page.locator('.ant-select-dropdown:not(.ant-select-dropdown-hidden)');
        const firstOption = dropdown.locator('.ant-select-item-option').first();
        await firstOption.hover();
        await this.page.waitForTimeout(100);
        for (let i = 0; i < 10; i++) {
            const activeText = await dropdown.locator('.ant-select-item-option-active .ant-select-item-option-content').textContent();
            if (activeText?.trim() === targetText) {
                await this.page.keyboard.press('Enter');
                await this.page.waitForSelector('.ant-select-dropdown:not(.ant-select-dropdown-hidden)', { state: 'detached' });
                return;
            }
            await this.page.keyboard.press('ArrowDown');
            await this.page.waitForTimeout(50);
        }
    }

    async clickSelectAllButton() {
        await this.click(this.BTN_SELECTALL_LOCATOR);
    }

    async clickSaveChangesButton() {
        await this.click(this.BTN_SAVECHANGES_LOCATOR);
    }

    async expectFirstTimeSlot() {
        const timeRangeText = await this.TBL_FIRSTTIMESLOT_LOCATOR.locator('div').first().innerText();
        const startTimeString = timeRangeText.split(' - ')[0]; // "00:30"

        const today = new Date();
        const [startHour, startMinute] = startTimeString.split(':').map(Number);
        const startTime = new Date(today);
        startTime.setHours(startHour, startMinute, 0, 0);

        const now = new Date();

        console.log(`First available slot starts at: ${startTime.toLocaleTimeString()}`);
        console.log(`Current time is: ${now.toLocaleTimeString()}`);

        expect(startTime.getTime()).toBeGreaterThanOrEqual(now.getTime());
    }
}