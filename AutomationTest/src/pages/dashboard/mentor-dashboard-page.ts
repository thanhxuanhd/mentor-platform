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

  async verifyStartDateIsAfterOrEqualCurrentDate() {
    const startDateText = await this.LBL_START_DATE.textContent();
    if (!startDateText) {
      throw new Error("startDateText is undefined");
    }
    const [datePart] = startDateText.split(" • ");
    if (!datePart)
      throw new Error(`Could not extract date part from: ${startDateText}`);

    const sessionDate = new Date(datePart);
    const today = new Date();

    const sessionDay = new Date(sessionDate.toDateString());
    const todayDay = new Date(today.toDateString());

    expect(sessionDay.getTime()).toBeGreaterThanOrEqual(todayDay.getTime());
  }
}
