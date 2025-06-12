import { Locator, Page } from "@playwright/test";
import { BasePage } from "../base-page";
import { PAGE_ENDPOINT_URL } from "../../core/constants/page-url";

export class LearnerDashboardPage extends BasePage {
  private TAG_RESCHEDULE_BUTTON: Locator;
  private BTN_CANCEL_BUTTON: Locator;
  private BTN_ACCEPT_BUTTON: Locator;
  private BTN_CLOSE_BUTTON: Locator;
  private LBL_SUCCESS_CANCEL_LOCATOR: Locator;
  private LBL_SUCCESS_ACCEPT_LOCATOR: Locator;

  constructor(page: Page) {
    super(page);
    this.TAG_RESCHEDULE_BUTTON = page.getByText("Rescheduled").first();
    this.BTN_CANCEL_BUTTON = page.getByRole("button", { name: "Cancel" });
    this.BTN_ACCEPT_BUTTON = page.getByRole("button", { name: "Accept" });
    this.BTN_CLOSE_BUTTON = page.getByRole("button", { name: "Close" });
    this.LBL_SUCCESS_CANCEL_LOCATOR = page.getByText(
      "Canceled reschedule successfully"
    );
    this.LBL_SUCCESS_ACCEPT_LOCATOR = page.getByText(
      "Rescheduled successfully"
    );
  }

  async navigateToDashboardPage() {
    await this.page.goto(PAGE_ENDPOINT_URL.DASHBOARD);
  }

  async clickOnRescheduleSession() {
    await this.click(this.TAG_RESCHEDULE_BUTTON);
  }

  async clickOnCancelButton() {
    await this.click(this.BTN_CANCEL_BUTTON);
  }

  async clickOnAcceptButton() {
    await this.click(this.BTN_ACCEPT_BUTTON);
  }

  async clickOnCloseButton() {
    await this.click(this.BTN_CLOSE_BUTTON);
  }

  async expectSuccessCancelMessage() {
    await this.isVisible(this.LBL_SUCCESS_CANCEL_LOCATOR);
  }

  async expectSuccessAcceptMessage() {
    await this.isVisible(this.LBL_SUCCESS_ACCEPT_LOCATOR);
  }
}
