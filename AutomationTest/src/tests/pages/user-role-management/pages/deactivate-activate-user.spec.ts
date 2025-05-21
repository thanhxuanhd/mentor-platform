import { expect } from "@playwright/test";
import { UserRoleManagementPage } from "../../../../pages/user-role-management-page/user-role-management-page";
import userData from "../../../test-data/user-role-management.json";
import { test } from "../../../../core/fixture/authFixture";

test.describe("@UserRoleManagement All user role management testcase", async () => {
  let userRoleManagementPage: UserRoleManagementPage;
  const notification = userData.notification_message;

  test.beforeEach(async ({ loggedInPage, page }) => {
    userRoleManagementPage = new UserRoleManagementPage(page);
    await test.step("Navigate to User Role Management page", async () => {
      await userRoleManagementPage.navigateToUserRoleManagementPage();
      await userRoleManagementPage.navigateToUsers();
    });
  });

  //Activate/Deactivate user

  test("@SmokeTest Verify that Activate/ Deactivated user works correctly", async () => {
    const statusList = await userRoleManagementPage.getAllUserStatus();

    for (let i = 0; i < statusList.length; i++) {
      const status = statusList[i].toLowerCase();
      await userRoleManagementPage.clickOnActivateOrDeactivateUserBtn(
        status,
        i
      );
      const actualResult = await userRoleManagementPage.getNotification(i);
      expect(actualResult).toBe(notification[status]);
    }
  });
});
