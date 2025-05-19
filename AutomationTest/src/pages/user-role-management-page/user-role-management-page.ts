import { expect, Locator, Page } from "@playwright/test";
import { BasePage } from "../base-page";
import { get } from "http";

export class UserRoleManagementPage extends BasePage {
  //Side header
  private LNK_USER_ROLE_MANAGEMENT_LOC: Locator;

  //Edit function
  private LBL_ALL_USER_ROLE_LOC: Locator;
  private BTN_SAVE_LOC: Locator;
  private TXT_EDIT_USER_FULLNAME_LOC: Locator;
  private TXT_EDIT_USER_EMAIL_LOC: Locator;
  private DDL_EDIT_USER_ROLE_LOC: Locator;
  private DDL_USER_ROLE_OPTIONS_LOC: (role: string) => Locator;
  private readonly BTN_EDIT_USER_LOC: Locator;
  private readonly BTN_CANCEL_USER_LOC: Locator;

  //Search user
  private TXT_SEARCH_USER_LOC: Locator;
  private DDL_ROLE_FILTER_LOC: (role: string) => Locator;
  private BTN_SEARCH_LOC: Locator;

  //Pagination function
  private readonly BTN_PREVIOUS_LOC: Locator;
  private readonly BTN_NEXT_LOC: Locator;
  private readonly BTN_NAVIGATION_LOC: Locator;
  private DDL_ITEM_PER_PAGE_LOC: (item: string) => Locator;
  private readonly DDL_ITEM_PER_PAGE_SELECTED_VALUE_LOC: Locator;

  //Table elements locator
  private readonly DBL_ALL_USER_LOC: Locator;
  private readonly DGD_ROW_USER_LOC: Locator;
  private LBL_USER_FULLNAME_LOC: (index: number) => Locator;
  private LBL_USER_EMAIL_LOC: (index: number) => Locator;
  private LBL_USER_ROLE_LOC: (index: number) => Locator;

  //Activate/Deactivate user
  private readonly USER_STATUS_LOC: Locator;

  //Error message
  private readonly LBL_NOTIFICATION_LOC: Locator;
  private readonly LBL_NO_DATA_MESSAGE_LOC: Locator;

  constructor(page: Page) {
    super(page);
    this.LBL_NOTIFICATION_LOC = page.locator(
      ".ant-notification-notice-description"
    );
    this.USER_STATUS_LOC = page.locator(
      ".ant-table-cell:nth-child(4) .ant-tag"
    );
    this.DGD_ROW_USER_LOC = page.locator(".ant-table-content .ant-table-row");
    this.DDL_ITEM_PER_PAGE_SELECTED_VALUE_LOC = page.locator("div.ant-select");
    this.DBL_ALL_USER_LOC = page.locator(
      ".ant-table-row .truncate .font-medium"
    );
    this.BTN_NAVIGATION_LOC = page.locator(
      "ul.ant-pagination .ant-pagination-item"
    );
    this.BTN_NEXT_LOC = page.getByRole("img", { name: "right" });
    this.BTN_PREVIOUS_LOC = page.getByRole("img", { name: "left" });
    this.TXT_SEARCH_USER_LOC = page.getByPlaceholder("Search user...");
    this.DDL_ROLE_FILTER_LOC = (role: string) => {
      return this.page.locator("div.ant-segmented-item-label", {
        hasText: role,
      });
    };
    this.DDL_ITEM_PER_PAGE_LOC = (item: string) => {
      return this.page.locator(`.ant-select-item-option[title="${item}"]`);
    };
    this.LBL_NO_DATA_MESSAGE_LOC = page.locator(".ant-empty-description");
    this.BTN_SEARCH_LOC = page.getByRole("img", { name: "search" });
    this.LNK_USER_ROLE_MANAGEMENT_LOC = page.locator(
      'span.ant-menu-title-content:has-text("Users")'
    );
    this.LBL_USER_FULLNAME_LOC = (index: number) => {
      return page.locator(
        `.ant-table-row:nth-child(${index + 2}) .truncate .font-medium`
      );
    };
    this.LBL_USER_EMAIL_LOC = (index: number) => {
      return this.page.locator(
        `.ant-table-row:nth-child(${index + 2}) .truncate .text-gray-500`
      );
    };
    this.LBL_USER_ROLE_LOC = (index: number) => {
      return this.page.locator(
        `.ant-table-row .ant-table-cell:nth-child(2) span`
      );
    };
    this.BTN_EDIT_USER_LOC = page.getByRole("img", { name: "edit" });
    this.BTN_CANCEL_USER_LOC = page.getByRole("button", {
      name: "Cancel",
    });
    this.LBL_ALL_USER_ROLE_LOC = page.locator(
      ".ant-table-cell:nth-child(2) .ant-tag"
    );
    this.TXT_EDIT_USER_EMAIL_LOC = page.locator("#edit_user_form_email");
    this.TXT_EDIT_USER_FULLNAME_LOC = page.locator("#edit_user_form_fullName");
    this.BTN_SAVE_LOC = page.getByRole("button", { name: "Save" });
    this.DDL_EDIT_USER_ROLE_LOC = this.page.locator(
      ".ant-modal .ant-select-selector"
    );
    this.DDL_USER_ROLE_OPTIONS_LOC = (role: string) => {
      return this.page.locator(
        `.ant-select-item .ant-select-item-option-content:has-text("${role}")`
      );
    };
  }

  async navigateToUserRoleManagementPage(url = "") {
    await this.page.goto(url);
  }

  async navigateToUsers() {
    await this.LNK_USER_ROLE_MANAGEMENT_LOC.click();
  }

  //View detailed user information
  async clickOnEditUserBtn(index = 0) {
    await this.BTN_EDIT_USER_LOC.nth(index).click();
  }

  //Save button
  async verifySaveButtonIsEnabled() {
    const isEnable = await this.BTN_SAVE_LOC.isEnabled();
    return isEnable;
  }

  //Edit function
  async fillEditUserForm(fullname = "", email = "", role = "Admin") {
    await this.fill(this.TXT_EDIT_USER_FULLNAME_LOC, fullname);
    await this.fill(this.TXT_EDIT_USER_EMAIL_LOC, email);
    await this.DDL_EDIT_USER_ROLE_LOC.click();
    await this.DDL_USER_ROLE_OPTIONS_LOC(role).click();
  }

  async clickOnSaveBtn() {
    await this.BTN_SAVE_LOC.click();
  }

  async getEditUserData(index: number) {
    const userFullname = await this.LBL_USER_FULLNAME_LOC(index).textContent();
    const userEmail = await this.LBL_USER_EMAIL_LOC(index).textContent();
    const userRole = await this.LBL_USER_ROLE_LOC(index)
      .nth(index)
      .textContent();
    return {
      userFullname,
      userEmail,
      userRole,
    };
  }

  //Cancel button
  async clickOnCancelBtn() {
    await this.BTN_CANCEL_USER_LOC.click();
  }

  async verifyCancelButtonIsEnabled() {
    const isEnable = await this.BTN_CANCEL_USER_LOC.isEnabled();
    return isEnable;
  }

  //Activate/Deactivate user
  async clickOnDeactivateUserBtn(index: number) {
    const btnDeactivate = `button:has([aria-label="stop"])`;
    return await this.DGD_ROW_USER_LOC.nth(index)
      .locator(btnDeactivate)
      .click();
  }

  async clickOnActivateUserBtn(index: number) {
    const btnActivate = `button:has([aria-label="check"])`;
    return await this.DGD_ROW_USER_LOC.nth(index).locator(btnActivate).click();
  }

  async getUserStatus(index: number) {
    await expect(this.USER_STATUS_LOC.first()).toBeVisible();
    return await this.DGD_ROW_USER_LOC.nth(index)
      .locator(this.USER_STATUS_LOC)
      .first()
      .innerText();
  }

  //Filter user
  async filterUserByRole(role = "All") {
    await this.DDL_ROLE_FILTER_LOC(role).click();
  }

  async getUserRole(index: number) {
    return await this.DGD_ROW_USER_LOC.nth(index)
      .locator(this.LBL_ALL_USER_ROLE_LOC)
      .innerText();
  }

  //Search user
  async searchUser(userName = "") {
    await this.fill(this.TXT_SEARCH_USER_LOC, userName);
    await this.BTN_SEARCH_LOC.click();
  }

  async getNoDataErrorMessage() {
    if (this.LBL_NO_DATA_MESSAGE_LOC) {
      const errorMessage = await this.LBL_NO_DATA_MESSAGE_LOC.textContent();
      return errorMessage;
    }
  }

  async expectAllUsersContain(value: string) {
    if (await this.getAllUser()) {
      const actualData = await this.getAllUser();
      for (const u of actualData) {
        expect(u).toContain(value);
      }
    } else {
      return "No data";
    }
  }

  async getAllUser() {
    const allUser = await this.DBL_ALL_USER_LOC.allTextContents();
    return allUser;
  }

  async getAllTableRowCount() {
    await this.page.waitForTimeout(2000); // sleep for 2 seconds
    const allRow = await this.DGD_ROW_USER_LOC.count();
    return allRow;
  }

  //Pagination function
  async getPaginationDefaultValue() {
    const defaultValue =
      await this.DDL_ITEM_PER_PAGE_SELECTED_VALUE_LOC.textContent();
    return defaultValue;
  }

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

  async clickOnItemPerPage(number: string) {
    await this.DDL_ITEM_PER_PAGE_SELECTED_VALUE_LOC.click();
    await this.DDL_ITEM_PER_PAGE_LOC(number).click();
  }

  //Verify error message
  async getNotification() {
    const notification = await this.LBL_NOTIFICATION_LOC.first().textContent();
    if (notification) {
      return notification;
    }
  }
}
