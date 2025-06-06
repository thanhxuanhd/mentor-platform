import { expect, Locator, Page } from "@playwright/test";
import { BasePage } from "../base-page";
import { PAGE_ENDPOINT_URL } from "../../core/constants/page-url";

export class AdminDashboardPage extends BasePage {
  private TXT_SEARCH_REPORT: Locator;
  private LBL_ACTIVITY_REPORT: Locator;
  private LBL_NO_DATA: Locator;
  constructor(page: Page) {
    super(page);
    this.TXT_SEARCH_REPORT = page.getByPlaceholder("Search by keyword");
    this.LBL_ACTIVITY_REPORT = page.locator(
      ".ant-table-row .ant-table-cell:nth-child(1)"
    );
    this.LBL_NO_DATA = page.locator(".ant-table-tbody .ant-table-cell");
  }

  async navigateToDashboard() {
    await this.page.goto(PAGE_ENDPOINT_URL.DASHBOARD);
  }

  async searchActivityReportKeyword(value = "") {
    return this.fill(this.TXT_SEARCH_REPORT, value);
  }

  async getAllReportText() {
    await this.waitUntilVisible(this.LBL_ACTIVITY_REPORT.first());
    const allUser = await this.LBL_ACTIVITY_REPORT.allTextContents();
    return allUser;
  }

  async verifyAllActivityReportContain(value: string) {
    const actualData = await this.getAllReportText();
    for (const u of actualData) {
      expect(u.toLowerCase()).toContain(value.toLowerCase());
    }
  }

  async verifyNoDataMessageExist(message = "") {
    await this.expectMessage(message);
  }
}
