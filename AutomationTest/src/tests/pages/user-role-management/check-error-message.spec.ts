import test, { expect } from "@playwright/test";
import { UserRoleManagementPage } from "../../../pages/user-role-management-page/user-role-management-page";
import userData from "./user-role-management.json";

test.describe("@UserRoleManagement All user role management testcase", async () => {
  let userRoleManagementPage: UserRoleManagementPage;
  const existedEmail = userData.existed_email;

  test.beforeEach(async ({ page }) => {
    userRoleManagementPage = new UserRoleManagementPage(page);
    await test.step("Navigate to User Role Management page", async () => {
      await userRoleManagementPage.navigateToUserRoleManagementPage();
      await userRoleManagementPage.navigateToUsers();
    });
  });

  //Check error message when enter user email is exists in edit user
  test("Verify that error message is shown when user email already exists", async () => {
    await test.step("Click edit user button", async () => {
      await userRoleManagementPage.clickOnEditUserBtn(0);
    });
    await test.step("Fill edit user form with existing email", async () => {
      await userRoleManagementPage.fillEditUserForm(
        existedEmail.fullname,
        existedEmail.email
      );
    });
    await test.step("Click save button", async () => {
      await userRoleManagementPage.clickOnSaveBtn();
    });
    await test.step("Verify error message is shown", async () => {
      const errorMessage = await userRoleManagementPage.getNotification();
      expect(errorMessage).toBe(`Email ${existedEmail.email} already exists.`);
    });
  });
});
