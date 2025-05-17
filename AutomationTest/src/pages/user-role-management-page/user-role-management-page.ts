import { expect, Locator, Page } from "@playwright/test";
import { BasePage } from "../base-page";

export class UserRoleManagementPage extends BasePage {
  private LI_USERROLE_MANAGEMENT: Locator = this.page.locator(
    'span.ant-menu-title-content:has-text("Users")'
  );
  private USER_STATUS: Locator = this.page.locator(
    ".ant-table-cell:nth-child(4) .ant-tag"
  );
  private btnDeactivateUser = this.page.locator(
    `tr button:has([aria-label="stop"])`
  );

  private btnActivateUser = this.page.locator(
    'tr button:has([aria-label="check"])'
  );

  private BTN_EDIT_USER: Locator = this.page.getByRole("img", { name: "edit" });
  private BTN_CANCEL_USER: Locator = this.page.getByRole("button", {
    name: "Cancel",
  });
  private BTN_SAVE: Locator = this.page.getByRole("button", { name: "Save" });
  private TXT_EDIT_USER_FULLNAME: Locator = this.page.locator(
    "#edit_user_form_fullName"
  );
  private TXT_EDIT_USER_EMAIL: Locator = this.page.locator(
    "#edit_user_form_email"
  );
  private DDL_USER_ROLE: Locator = this.page.locator(
    ".ant-modal .ant-select-selector"
  );
  private userRoleOption(role: string) {
    return this.page.locator(
      `.ant-select-item .ant-select-item-option-content:has-text("${role}")`
    );
  }
  private TXT_SEARCH_USER: Locator =
    this.page.getByPlaceholder("Search user...");
  private BTN_SEARCH: Locator = this.page.getByRole("img", { name: "search" });
  private ERROR_MESSAGE: Locator = this.page.locator(".ant-empty-description");
  private ROW_USER: Locator = this.page.locator(
    ".ant-table-content .ant-table-row"
  );
  private roleFilter(role: string) {
    const roleFilterLoc = this.page.locator("div.ant-segmented-item-label", {
      hasText: role,
    });
    return roleFilterLoc;
  }
  private itemPerPage(item: string) {
    return this.page.locator(`.ant-select-item-option[title="${item}"]`);
  }
  private DDL_ITEM_PER_PAGE: Locator = this.page.locator("div.ant-select");
  private BTN_PREVIOUS: Locator = this.page.getByRole("img", { name: "left" });
  private BTN_NEXT: Locator = this.page.getByRole("img", { name: "right" });
  private BTN_NAVIGATION: Locator = this.page.locator(
    "ul.ant-pagination .ant-pagination-item"
  );

  constructor(page: Page) {
    super(page);
  }

  async navigateToUserRoleManagementPage(url = "") {
    await this.page.goto(url);
  }

  async navigateToUsers() {
    await this.LI_USERROLE_MANAGEMENT.click();
  }

  //View detailed user information
  async clickOnEditUserBtn(index = 0) {
    await this.BTN_EDIT_USER.nth(index).click();
  }

  //Save button
  async verifySaveButtonIsEnabled() {
    const isEnable = await this.BTN_SAVE.isEnabled();
    return isEnable;
  }

  //Edit function
  async fillEditUserForm(fullname = "", email = "", role = "Admin") {
    await this.fill(this.TXT_EDIT_USER_FULLNAME, fullname);
    await this.fill(this.TXT_EDIT_USER_EMAIL, email);
    await this.DDL_USER_ROLE.click();
    await this.userRoleOption(role).click();
  }

  async clickOnSaveBtn() {
    await this.BTN_SAVE.click();
  }

  async getEditUserData(index) {
    const USER_FULLNAME: Locator = this.page.locator(
      `.ant-table-row:nth-child(${index + 2}) .truncate .font-medium`
    );
    await this.waitUntilVisible(USER_FULLNAME);
    const USER_EMAIL: Locator = this.page.locator(
      `.ant-table-row:nth-child(${index + 2}) .truncate .text-gray-500`
    );
    await this.waitUntilVisible(USER_EMAIL);
    const USER_ROLE: Locator = this.page.locator(
      `.ant-table-row .ant-table-cell:nth-child(2) span`
    );

    const userEmail = await USER_EMAIL.textContent();
    const userFullname = await USER_FULLNAME.textContent();
    const userRole = await USER_ROLE.nth(index).textContent();
    return {
      userFullname,
      userEmail,
      userRole,
    };
  }

  //Cancel button
  async clickOnCancelBtn() {
    await this.BTN_CANCEL_USER.click();
  }

  async verifyCancelButtonIsEnabled() {
    const isEnable = await this.BTN_CANCEL_USER.isEnabled();
    return isEnable;
  }

  //Activate/Deactivate user
  async clickOnDeactivateUserBtn(index: number) {
    await this.waitUntilVisible(this.btnDeactivateUser.first(), 2000);
    return await this.ROW_USER.nth(index)
      .locator(this.btnDeactivateUser)
      .click();
  }

  async clickOnActivateUserBtn(index: number) {
    await this.waitUntilVisible(this.btnActivateUser.first(), 2000);
    return await this.ROW_USER.nth(index).locator(this.btnActivateUser).click();
  }

  async getUserStatus(index: number) {
    await expect(this.USER_STATUS.first()).toBeVisible();
    return await this.ROW_USER.nth(index)
      .locator(this.USER_STATUS)
      .first()
      .innerText();
  }

  //Filter user
  async filterByRole(role = "All") {
    await this.roleFilter(role).click();
  }

  //Search user
  async searchUser(userName = "") {
    await this.fill(this.TXT_SEARCH_USER, userName);
    await this.BTN_SEARCH.click();
  }

  async getErrorMessage() {
    if (this.ERROR_MESSAGE) {
      const errorMessage = await this.ERROR_MESSAGE.textContent();
      return errorMessage;
    }
  }

  async expectAllUsersContain(value: string) {
    const actualData = await this.getAllUser();
    for (const u of actualData) {
      expect(u).toContain(value);
    }
  }

  async getAllUser() {
    const allUser = await this.page
      .locator(".ant-table-row .truncate .font-medium")
      .allTextContents();
    return allUser;
  }

  async getAllTableRowCount() {
    const allRow = await this.ROW_USER.count();
    return allRow;
  }

  //Pagination function
  async getPaginationDefaultValue() {
    const defaultValue = await this.DDL_ITEM_PER_PAGE.textContent();
    return defaultValue;
  }

  async getPreviousButtonStatus() {
    const isEnable = await this.BTN_PREVIOUS.isEnabled();
    return isEnable;
  }

  async getNextButtonStatus() {
    const isEnable = await this.BTN_NEXT.isEnabled();
    return isEnable;
  }

  async clickOnNavigationButton(index: number) {
    await this.BTN_NAVIGATION.nth(index).click();
  }

  async getAllPagingCount() {
    const numberOfPage = await this.BTN_NAVIGATION.count();
    return numberOfPage;
  }

  async clickOnItemPerPage(number: string) {
    await this.DDL_ITEM_PER_PAGE.click();
    await this.itemPerPage(number).click();
  }
}
