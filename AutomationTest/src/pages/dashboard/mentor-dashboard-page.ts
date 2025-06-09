import { expect, Locator, Page } from "@playwright/test";
import { BasePage } from "../base-page";
import { PAGE_ENDPOINT_URL } from "../../core/constants/page-url";

export class MentorDashboardPage extends BasePage {
  private LBL_TOTAL_COURSE: Locator;
  constructor(page: Page) {
    super(page);
    this.LBL_TOTAL_COURSE = page.locator(
      "#pending-approval .text-right .text-white"
    );
  }

  async goToDashboardPage() {
    await this.page.goto(PAGE_ENDPOINT_URL.DASHBOARD);
  }

  async getTotalCourseValue() {
    const value = await this.LBL_TOTAL_COURSE.textContent();
    return value;
  }
}
