import { Page, Locator, expect } from "@playwright/test";
import { BasePage } from "../base-page";
import { PAGE_ENDPOINT_URL } from "../../core/constants/page-url";

export class CourseViewPage extends BasePage {
    private LNK_COURSE_URL: string;
    private DRD_DIFFICULTY_LOCATOR: (diffculty: string) => Locator;
    private DRD_DIFFICULTYFIRST_LOCATOR: Locator;
    private TXT_KEYWORD_LOCATOR: Locator;
    private DRD_CATEGORY_LOCATOR: Locator;
    private DRD_MENTOR_LOCATOR: Locator;
    private DRD_STATUSFIRST_LOCATOR: Locator;
    private DRD_STATUS_LOCATOR: (status: string) => Locator;
    private TBL_COURSELIST_LOCATOR: Locator;

    constructor(page: Page) {
        super(page);
        this.LNK_COURSE_URL = PAGE_ENDPOINT_URL.COURSE_MANAGEMENT;
        this.TXT_KEYWORD_LOCATOR = page.getByPlaceholder("Filter by keyword");
        this.DRD_CATEGORY_LOCATOR = page.locator("#category");
        this.DRD_DIFFICULTYFIRST_LOCATOR = page.getByLabel("Difficulty");
        this.DRD_DIFFICULTY_LOCATOR = (difficulty: string): Locator => {
            return this.page.locator(`.ant-select-dropdown [title="${difficulty}"]`);
        };
        this.DRD_MENTOR_LOCATOR = page.locator("#mentor ");
        this.DRD_STATUSFIRST_LOCATOR = page.getByLabel("Status");
        this.DRD_STATUS_LOCATOR = (status: string): Locator => {
            return this.page.locator(`.ant-select-dropdown [title="${status}"]`);
        };
        this.TBL_COURSELIST_LOCATOR = page.locator('tbody.ant-table-tbody tr:not([aria-hidden="true"])');
    }

    async navigateToHomePage(url = "") {
        await this.page.goto(url);
    }

    async goToCoursePage() {
        await this.page.goto(this.LNK_COURSE_URL);
    }

    async inputKeyword(keyword: string) {
        await this.fill(this.TXT_KEYWORD_LOCATOR, keyword);
    }

    async selectCategory(category: string) {
        await this.fill(this.DRD_CATEGORY_LOCATOR, category);
        await this.DRD_CATEGORY_LOCATOR.press('Enter');
    }

    async selectDifficulty(difficulty: string) {
        await this.click(this.DRD_DIFFICULTYFIRST_LOCATOR);
        await this.click(this.DRD_DIFFICULTY_LOCATOR(difficulty));
    }

    async selectMentor(mentor: string) {
        await this.fill(this.DRD_MENTOR_LOCATOR, mentor);
        await this.DRD_MENTOR_LOCATOR.press("Enter");
    }

    async selectStatus(status: string) {
        await this.click(this.DRD_STATUSFIRST_LOCATOR);
        await this.click(this.DRD_STATUS_LOCATOR(status));
    }

    async assertCourseTableIsNotEmpty() {
        const rowCount = await this.TBL_COURSELIST_LOCATOR.count();
        await expect(rowCount).toBeGreaterThan(0);
    }

    async logCourseNames() {
        for (let i = 0; i < await this.TBL_COURSELIST_LOCATOR.count(); i++) {
            const courseNameLocator = this.TBL_COURSELIST_LOCATOR.nth(i).locator('td').first();
            const courseName = await courseNameLocator.innerText();
            console.log(`Course ${i + 1}: ${courseName}`);
        }
    }
}