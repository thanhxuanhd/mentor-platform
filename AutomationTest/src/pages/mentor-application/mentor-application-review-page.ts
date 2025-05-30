import { expect, Locator, Page } from "@playwright/test";
import { BasePage } from "../base-page";
import { PAGE_ENDPOINT_URL } from "../../core/constants/page-url";
import { LoginPage } from "../authentication/login-page";
import { API_ENDPOINTS } from "../../core/constants/api-endpoint-url";

export class MentorApplicationReview extends BasePage {
  //Admin view
  private readonly LBL_MENTOR_APPLICATION: (mentorName: string) => Locator;
  private readonly BTN_APPLICATION_STATUS: (status: string) => Locator;
  private readonly LBL_NOTIFICATION: Locator;
  private readonly TXT_ADMIN_NOTES: Locator;
  private readonly LBL_APPLICATION_FILTER_STATUS: (filter: string) => Locator;

  constructor(page: Page) {
    super(page);
    this.LBL_MENTOR_APPLICATION = (mentorName) => {
      return this.page.locator(
        `.ant-list-item-meta-title .items-start .text-white:has-text("${mentorName}")`
      );
    };

    this.BTN_APPLICATION_STATUS = (status) => {
      return this.page.locator(`button:has-text("${status}")`);
    };
    this.TXT_ADMIN_NOTES = page.getByPlaceholder(
      "Add notes about this application..."
    );
    this.LBL_NOTIFICATION = page.locator(".ant-notification-notice");
    this.LBL_APPLICATION_FILTER_STATUS = (filter) => {
      return this.page.locator(
        `div.ant-segmented-item-label:has-text("${filter}")`
      );
    };
  }

  //Admin view
  async navigateToApplicationsPage() {
    await this.page.goto(PAGE_ENDPOINT_URL.APPLICATIONS);
  }
  async clickOnMentorApplicationAdmin(mentorName: string) {
    await this.LBL_MENTOR_APPLICATION(mentorName).click();
  }
  async clickOnFilterStatus(mentorName: string) {
    await this.LBL_APPLICATION_FILTER_STATUS(mentorName).click();
  }
  async fillInAdminNotesField(adminNotes: string) {
    await this.fill(this.TXT_ADMIN_NOTES, adminNotes);
  }
  async clickOnStatusActionButton(status: string) {
    await this.BTN_APPLICATION_STATUS(status).click();
  }
  async verifyNotificationMessage() {
    await this.waitUntilVisible(this.LBL_NOTIFICATION);
    await this.isVisible(this.LBL_NOTIFICATION);
  }
  async requestInfoFromMentor(mentorName: string, status: string) {
    await this.clickOnMentorApplicationAdmin(mentorName);
    await this.clickOnStatusActionButton(status);
    await this.verifyNotificationMessage();
  }

  async rejectMentorApplication(
    mentorName: string,
    filter: string,
    status: string
  ) {
    await this.clickOnFilterStatus(filter);
    await this.clickOnMentorApplicationAdmin(mentorName);
    await this.fillInAdminNotesField("This is test notes");
    await this.clickOnStatusActionButton(status);
    await this.verifyNotificationMessage();
  }
}
