import { Locator, Page } from "@playwright/test";
import { BasePage } from "../base-page";

export class AdminDashboardPage extends BasePage {
  private readonly BTN_REVIEW_APPLICATIONS: Locator;
  private LBL_TOTAL_APPLICATIONS: Locator;
  constructor(page: Page) {
    super(page);
    this.LBL_TOTAL_APPLICATIONS = page.locator("#pending-approval");
  }

  async clickOnReviewApplicationsButton() {
    await this.click(this.BTN_REVIEW_APPLICATIONS);
  }

  async verifyTotalApplicationIsDecreaseBy1() {}
}
