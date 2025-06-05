import { expect, Locator, Page } from "@playwright/test";
import { BasePage } from "../base-page";
import { PAGE_ENDPOINT_URL } from "../../core/constants/page-url";

export class MentorDashBoardPage extends BasePage {
  private readonly LBL_START_DATE: Locator;
  constructor(page: Page) {
    super(page);

    this.LBL_START_DATE = page.locator("span.text-sm");
  }

  async goToDashboardPage() {
    await this.page.goto(PAGE_ENDPOINT_URL.DASHBOARD);
  }

  async verifyStartDateIsAfterCurrentDate() {
    const startDateText = await this.LBL_START_DATE.textContent();
    const match = startDateText?.match(
      /(\d{2})\/(\d{2})\/(\d{4}) • (\d{2}:\d{2})/
    );
    if (!match) {
      throw new Error("Start date format is invalid or not found.");
    }
    const [_, datePart, startTime] = match;
    const sessionDateTime = new Date(`${datePart}T${startTime}:00`);
    const now = new Date();

    expect(sessionDateTime.getTime()).toBeGreaterThan(now.getTime());
  }
}
