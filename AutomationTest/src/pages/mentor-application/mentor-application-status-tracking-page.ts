import { expect, Locator, Page } from "@playwright/test";
import { BasePage } from "../base-page";
import { PAGE_ENDPOINT_URL } from "../../core/constants/page-url";

export class MentorApplicationStatusTrackingPage extends BasePage {
  //Mentor view
  private readonly BTN_EDIT: Locator;
  private readonly LI_APPLICATION_LIST: Locator;
  private readonly BTN_CREATE_NEW_APPLICATION: Locator;

  constructor(page: Page) {
    super(page);
    this.LI_APPLICATION_LIST = page.locator("div.ant-list-item-meta");
    this.BTN_EDIT = page.getByRole("button", { name: "Edit" });
    this.BTN_CREATE_NEW_APPLICATION = page.getByRole("button", {
      name: "create new application",
    });
  }

  //history tracking
  async navigateToStatusTrackingPage() {
    await this.page.goto(PAGE_ENDPOINT_URL.STATUS_TRACKING);
  }

  async verifyEditMentorApplicationButtonIsEnable() {
    await this.waitUntilVisible(this.LI_APPLICATION_LIST.first());
    await this.LI_APPLICATION_LIST.first().click();
    const result = await this.isLocatorVisible(this.BTN_EDIT);
    expect(result).toBeTruthy();
  }

  async verifyCreateNewApplicaitonButtonIsEnable() {
    await this.waitUntilVisible(this.LI_APPLICATION_LIST.first());
    const result = await this.isLocatorVisible(this.BTN_CREATE_NEW_APPLICATION);
    expect(result).toBeTruthy();
  }
}
