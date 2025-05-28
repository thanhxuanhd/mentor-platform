import { Page, Locator } from "@playwright/test";
import { BasePage } from "../base-page";
import { PAGE_ENDPOINT_URL } from "../../core/constants/page-url";

export class CoursePage extends BasePage {
    private LNK_COURSE_URL: string;
    private TXT_TITLE_LOCATOR: Locator;
    private DRD_CATEGORY_LOCATOR: Locator;
    private DRD_DIFFICULTY_LOCATOR: (diffculty: string) => Locator;
    private DRD_DIFFICULTYFIRST_LOCATOR: Locator;
    private DTM_DUEDATE_LOCATOR: Locator;
    private TXT_TAGS_LOCATOR: Locator;
    private TXT_DESCRIPTION_LOCATOR: Locator;

    //add course locator
    private BTN_ADDTAG_LOCATOR: Locator;
    private BTN_ADDNEWCOURSE_LOCATOR: Locator;
    private BTN_CREATECOURSE_LOCATOR: Locator;

    //update course locator
    private BTN_UPDATECOURSEICON_LOCATOR: Locator;
    private BTN_SAVECHANGES_LOCATOR: Locator;

    //delete category locator
    private BTN_DELETECOURSEICON_LOCATOR: Locator;
    private BTN_CONFIRMDELETE_LOCATOR: Locator;
    private LBL_SUCCESSDELETE_LOCATOR: Locator;

    constructor(page: Page) {
        super(page);
        this.LNK_COURSE_URL = PAGE_ENDPOINT_URL.COURSE_MANAGEMENT;
        this.BTN_ADDNEWCOURSE_LOCATOR = page.getByText("Add New Course")
        this.TXT_TITLE_LOCATOR = page.getByPlaceholder('Enter new course title');
        this.DRD_CATEGORY_LOCATOR = page.locator('#categoryId');
        this.DRD_DIFFICULTYFIRST_LOCATOR = page.locator('label[for="difficulty"]').locator('xpath=ancestor::div[contains(@class, "ant-form-item")]//div[contains(@class,"ant-select-selector")]');
        this.DRD_DIFFICULTY_LOCATOR = (difficulty: string): Locator => {
            return this.page.locator(`.ant-select-dropdown [title="${difficulty}"]`);
        };
        this.DTM_DUEDATE_LOCATOR = page.getByPlaceholder('Select due date');
        this.TXT_TAGS_LOCATOR = page.getByPlaceholder("Add a tag");
        this.BTN_ADDTAG_LOCATOR = page.locator('//button[span[text()="Add"]]');
        this.TXT_DESCRIPTION_LOCATOR = page.getByPlaceholder('Enter course description');
        this.BTN_CREATECOURSE_LOCATOR = page.locator('//button[span[text()="Create Course"]]');
        this.BTN_UPDATECOURSEICON_LOCATOR = page.locator('[aria-label="edit"]').first().locator('xpath=ancestor::button');
        this.BTN_SAVECHANGES_LOCATOR = page.locator('//button[span[text()="Save Changes"]]');
        this.BTN_DELETECOURSEICON_LOCATOR = page.locator('[aria-label="delete"]').first().locator('xpath=ancestor::button');
        this.BTN_CONFIRMDELETE_LOCATOR = page.locator('//button[span[text()="Yes"]]');
        this.LBL_SUCCESSDELETE_LOCATOR = page.getByText("Delete successfully!");
    }

    async navigateToHomePage(url = "") {
        await this.page.goto(url);
    }

    async goToCoursePage() {
        await this.page.goto(this.LNK_COURSE_URL);
    }

    async clickAddNewCourseButton() {
        await this.click(this.BTN_ADDNEWCOURSE_LOCATOR);
    }

    async inputTitle(title: string) {
        await this.fill(this.TXT_TITLE_LOCATOR, title);
    }

    async selectCategory(category: string) {
        await this.fill(this.DRD_CATEGORY_LOCATOR, category);
        await this.DRD_CATEGORY_LOCATOR.press('Enter');
    }

    async selectDifficulty(difficulty: string) {
        await this.click(this.DRD_DIFFICULTYFIRST_LOCATOR);
        await this.click(this.DRD_DIFFICULTY_LOCATOR(difficulty));
    }

    async selectDueDate(dateStr: string) {
        if (!dateStr?.trim()) {
            return;
        }
        await this.DTM_DUEDATE_LOCATOR.click();

        const selectedDate = new Date(dateStr);
        const day = selectedDate.getDate();
        const month = selectedDate.getMonth();
        const year = selectedDate.getFullYear();

        const headerButton = this.page.locator('.ant-picker-header-view button');
        await this.click(headerButton.first());
        await this.click(headerButton.first());

        const yearOption = this.page.locator(`.ant-picker-year-panel .ant-picker-cell >> text="${year}"`);
        await this.waitUntilVisible(yearOption);
        await this.click(yearOption);

        const monthOption = this.page.locator('.ant-picker-month-panel .ant-picker-cell').nth(month);
        await this.waitUntilVisible(monthOption);
        await this.click(monthOption);

        const dayOption = this.page.locator(`.ant-picker-cell:not(.ant-picker-cell-disabled) >> text="${day}"`);
        await this.waitUntilVisible(dayOption);
        await this.click(dayOption);
    }

    async inputTagsName(tags: string[]) {
        for (const tag of tags) {
            await this.fill(this.TXT_TAGS_LOCATOR, tag);
            await this.TXT_TAGS_LOCATOR.press('Enter');
        }
    }

    async clickAddTagButton() {
        this.click(this.BTN_ADDTAG_LOCATOR);
    }

    async inputDescription(description: string) {
        await this.fill(this.TXT_DESCRIPTION_LOCATOR, description);
    }

    async clickCreateCourseButton() {
        await this.click(this.BTN_CREATECOURSE_LOCATOR);
    }

    async clickUpdateCourseIcon() {
        await this.click(this.BTN_UPDATECOURSEICON_LOCATOR);
    }

    async clickSaveChangesButton() {
        await this.click(this.BTN_SAVECHANGES_LOCATOR);
    }

    async clickDeleteCourseIcon() {
        await this.click(this.BTN_DELETECOURSEICON_LOCATOR);
    }

    async clickConfirmDeleteButton() {
        await this.click(this.BTN_CONFIRMDELETE_LOCATOR);
    }

    async expectSucessDeleteMessage() {
        await this.isVisible(this.LBL_SUCCESSDELETE_LOCATOR);
    }
}