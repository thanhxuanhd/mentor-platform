import { expect } from "@playwright/test";
import { test } from "../../../core/fixture/auth-fixture";
import userData from "../../test-data/user-role-management.json";
import { UserRoleManagementPage } from "../../../pages/user-role-management-page/user-role-management-page";

test.describe("@UserRoleManagement All user role management testcase", async () => {
  let userRoleManagementPage: UserRoleManagementPage;
  const notification = userData.notification_message;

  test.beforeEach(async ({ loggedInPageByAdminRole, page }) => {
    userRoleManagementPage = new UserRoleManagementPage(page);
    await test.step("Navigate to User Role Management page", async () => {
      await userRoleManagementPage.navigateToUserRoleManagementPage();
      await userRoleManagementPage.navigateToUsers();
    });
  });

  //Activate/Deactivate user

  test("@Regression Verify that Activate/ Deactivated user works correctly", async () => {
    const statusValue = await userRoleManagementPage.getFirstUserStatus();
    if (statusValue) {
      const status = statusValue.toLowerCase();
      await userRoleManagementPage.clickOnActivateOrDeactivateUserBtn(status);
      const actualResult = await userRoleManagementPage.getNotification();
      const expectedResult = notification[status];
      expect(actualResult).toBe(expectedResult);
    }
  });
});
