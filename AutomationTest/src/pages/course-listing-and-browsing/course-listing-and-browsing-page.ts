import { Page, Locator, expect } from "@playwright/test";
import { BasePage } from "../base-page";
import { PAGE_ENDPOINT_URL } from "../../core/constants/page-url";

export class CourseListingAndBrowsingPage extends BasePage {
  //Navigate to course listing and browsing page
  private readonly LNK_COURSE_LISTING_AND_BROWSING_URL: string;

  //Search user
  private TXT_SEARCH_COURSE_LOC: Locator;
  private DDL_LEVEL_OPTIONS: Locator;
  private DDL_CATEGORY_OPTIONS: Locator;
  private DDL_MENTOR_OPTIONS: Locator;

  //Pagination function
  private readonly BTN_PREVIOUS_LOC: Locator;
  private readonly BTN_NEXT_LOC: Locator;
  private readonly BTN_NAVIGATION_LOC: Locator;


  constructor(page: Page) {
    super(page);
    this.LNK_COURSE_LISTING_AND_BROWSING_URL = PAGE_ENDPOINT_URL.COURSES;
    this.TXT_SEARCH_COURSE_LOC = page.getByPlaceholder("Filter by keyword");

    this.DDL_LEVEL_OPTIONS = page.getByLabel("Difficulty");

    this.DDL_CATEGORY_OPTIONS = page.getByLabel("Category");

    this.DDL_MENTOR_OPTIONS = page.getByLabel("Mentor");

    this.BTN_PREVIOUS_LOC = page.locator(".ant-pagination-prev  button.ant-pagination-item-link");

    this.BTN_NEXT_LOC = page.locator(".ant-pagination-next  button.ant-pagination-item-link");

    this.BTN_NAVIGATION_LOC = page.locator(
      "ul.ant-pagination .ant-pagination-item"
    );
  }

  //Navigate to course listing and browsing page
  async navigateToCourses() {
    await this.page.goto(this.LNK_COURSE_LISTING_AND_BROWSING_URL);
  }

  //Search course by input data
  async searchCourse(title = "") {
    await this.fill(this.TXT_SEARCH_COURSE_LOC, title);
  }

  //Search course by category filter
  async selectCategory(category: string) {
    await this.DDL_CATEGORY_OPTIONS.selectOption(category);
  }

  //Search course by mentor filter
  async selectMentor(mentor: string) {
    await this.DDL_MENTOR_OPTIONS.selectOption(mentor);
  }

  //Search course by level filter
  async selectLevel(level: string) {
    await this.DDL_LEVEL_OPTIONS.selectOption(level);
  }

  async expectAllCoursesContain(value: string) {
    const actualData = await this.getAllCourse();
    for (const u of actualData) {
      expect(u.toLowerCase()).toContain(value.toLowerCase());
    }
  }

  async getAllCourse() {
    const allCourse = await this.page
      .locator(".ant-table-tbody > tr > td > div .font-medium")
      .allInnerTexts();
    return allCourse;
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
}
