import { Page, Locator, expect } from "@playwright/test";
import { BasePage } from "../base-page";

export class CourseListingAndBrowsingPage extends BasePage {
    //Navigate to course listing and browsing page
    private readonly LNK_COURSE_LISTING_AND_BROWSING_URL: string;

    //Search user
    private TXT_SEARCH_COURSE_LOC: Locator;
    private DDL_LEVEL_FILTER_LOC: Locator;
    private DDL_CATEGORY_FILTER_LOC: Locator;
    private DDL_MENTOR_FILTER_LOC: Locator;

    //View detailed course information
    private readonly BTN_EDIT_COURSE_LOC: Locator;
    private readonly DGD_ROW_COURSE_LOC: Locator;

    //Pagination function
    private readonly BTN_PREVIOUS_LOC: Locator;
    private readonly BTN_NEXT_LOC: Locator;
    private readonly BTN_NAVIGATION_LOC: Locator;

    //Error message
    private readonly LBL_NO_DATA_MESSAGE_LOC: Locator;

    constructor(page: Page) {
        super(page);
        this.LNK_COURSE_LISTING_AND_BROWSING_URL = "/courses";
        this.TXT_SEARCH_COURSE_LOC = page.getByPlaceholder("Filter by keyword");

        this.DDL_LEVEL_FILTER_LOC = page.getByLabel("Difficulty");

        this.DDL_CATEGORY_FILTER_LOC = page.getByLabel("Category");

        this.DDL_MENTOR_FILTER_LOC = page.getByLabel("Mentor");

        this.BTN_EDIT_COURSE_LOC = page.locator("span.anticon.anticon-eye");

        this.BTN_PREVIOUS_LOC = page.locator('.ant-pagination-prev');

        this.BTN_NEXT_LOC = page.locator('.ant-pagination-next');

        this.BTN_NAVIGATION_LOC = page.locator(
            "ul.ant-pagination .ant-pagination-item"
        );

        this.LBL_NO_DATA_MESSAGE_LOC = page.locator(".ant-empty-description");

        this.DGD_ROW_COURSE_LOC = page.locator(".ant-table-content .ant-table-row");
    }

    //Navigate to course listing and browsing page
    async navigateToCourseListingAndBrowsingPage(url = "") {
        await this.page.goto(url);
    }

    async navigateToLoginPage(url = "") {
        await this.page.goto(url);
    }

    async navigateToCourses() {
        await this.page.goto(this.LNK_COURSE_LISTING_AND_BROWSING_URL);
    }

    //View detailed course information
    async clickOnViewCourseBtn(index = 0) {
        await this.click(this.BTN_EDIT_COURSE_LOC.nth(index));
    }

    //Search course by input title
    async searchCourse(title = "") {
        await this.fill(this.TXT_SEARCH_COURSE_LOC, title);
    }

    //Search course by category filter
    async selectCategory() {
        await this.click(this.DDL_CATEGORY_FILTER_LOC);
        this.DDL_CATEGORY_FILTER_LOC.selectOption('Time Management');
    }

    //Search course by mentor filter
    async selectMentor() {
        await this.click(this.DDL_MENTOR_FILTER_LOC);
        this.DDL_MENTOR_FILTER_LOC.selectOption('DuongSenpai@at.local');
    }

    //Search course by level filter
    async selectLevel() {
        await this.click(this.DDL_LEVEL_FILTER_LOC);
        this.DDL_LEVEL_FILTER_LOC.selectOption('Beginner');
    }

    async expectAllCoursesContain(value: string) {
        const actualData = await this.getAllCourse();
        for (const u of actualData) {
            expect(u).toContainEqual(value);
        }
    }

    async getAllCourse() {
        const allCourse = await this.page.locator(".ant-table-tbody > tr > td > div .font-medium").allTextContents();
        return allCourse;
    }

    async getAllTableRowCount() {
        await this.page.waitForTimeout(2000); // sleep for 2 seconds
        const allRow = await this.DGD_ROW_COURSE_LOC.count();
        return allRow;
    }

    //Pagination function
    async getPreviousButtonStatus() {
        const isEnable = await this.BTN_PREVIOUS_LOC.isEnabled();
        return isEnable;
    }

    async getNextButtonStatus() {
        const isEnable = await this.BTN_NEXT_LOC.isEnabled();
        return isEnable;
    }

    async clickOnNavigationButton(index: number) {
        await this.BTN_NAVIGATION_LOC.nth(index).click();
    }

    async getAllPagingCount() {
        const numberOfPage = await this.BTN_NAVIGATION_LOC.count();
        return numberOfPage;
    }

    //Error message
    async getNoDataErrorMessage() {
        if (this.LBL_NO_DATA_MESSAGE_LOC) {
            const errorMessage = await this.LBL_NO_DATA_MESSAGE_LOC.textContent();
            return errorMessage;
        }
    }
}