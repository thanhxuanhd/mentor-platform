import test, { expect } from "@playwright/test";
import { UserRoleManagementPage } from "../../../pages/user-role-management-page/user-role-management-page";

test.describe("@UserRoleManagement All user role management testcase", async () => {
  let userRoleManagementPage: UserRoleManagementPage;

  test.beforeEach(async ({ page }) => {
    userRoleManagementPage = new UserRoleManagementPage(page);
    await test.step("Navigate to User Role Management page", async () => {
      await userRoleManagementPage.navigateToUserRoleManagementPage();
      await userRoleManagementPage.navigateToUsers();
    });
  });

  //Activate/Deactivate user
  test("@SmokeTest Verify that Activate/Deactivate user works correctly", async () => {
    const rowCount = await userRoleManagementPage.getAllTableRowCount();

    for (let i = 2; i < rowCount; i++) {
      const status = await userRoleManagementPage.getUserStatus(i);

      if (status === "Pending") {
        await test.step(`Activate user in row ${i}`, async () => {
          await userRoleManagementPage.clickOnActivateUserBtn(i);
          const errorMessage = await userRoleManagementPage.getNotification();
          expect(errorMessage).toBe(`User status updated to Active.`);
        });
      } else if (status === "Active") {
        await test.step(`Deactivate user in row ${i}`, async () => {
          await userRoleManagementPage.clickOnDeactivateUserBtn(i);
          const errorMessage = await userRoleManagementPage.getNotification();
          expect(errorMessage).toBe(`User status updated to Deactivated.`);
        });
      } else if (status === "Deactivated") {
        await test.step(`Deactivate user in row ${i}`, async () => {
          await userRoleManagementPage.clickOnActivateUserBtn(i);
          const errorMessage = await userRoleManagementPage.getNotification();
          expect(errorMessage).toBe(`User status updated to Active.`);
        });
      }
    }
  });
});
