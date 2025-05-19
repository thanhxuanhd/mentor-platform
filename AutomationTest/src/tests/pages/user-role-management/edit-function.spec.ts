import test, { expect } from "@playwright/test";
import { UserRoleManagementPage } from "../../../pages/user-role-management-page/user-role-management-page";
import userData from "./user-role-management.json";

test.describe("@UserRoleManagement All user role management testcase", async () => {
  let userRoleManagementPage: UserRoleManagementPage;
  const editUser = userData.validate_edit_user;

  test.beforeEach(async ({ page }) => {
    userRoleManagementPage = new UserRoleManagementPage(page);
    await test.step("Navigate to User Role Management page", async () => {
      await userRoleManagementPage.navigateToUserRoleManagementPage();
      await userRoleManagementPage.navigateToUsers();
    });
  });

  //Edit function
  editUser.forEach((user, index) => {
    test(`@SmokeTest Validate edit user successfully ${index}`, async () => {
      await test.step("Click edit user button", async () => {
        await userRoleManagementPage.clickOnEditUserBtn(index);
      });

      await test.step(`Fill edit user form`, async () => {
        await userRoleManagementPage.fillEditUserForm(
          user.userFullname,
          user.userEmail,
          user.userRole
        );
      });

      await test.step(`Click save button`, async () => {
        await test.step("Verify save button is enable", async () => {
          const isEnable =
            await userRoleManagementPage.verifySaveButtonIsEnabled();
          expect(isEnable).toBeTruthy();
        });
        await test.step("Verify cancel button is enabled", async () => {
          const isEnable =
            await userRoleManagementPage.verifyCancelButtonIsEnabled();
          expect(isEnable).toBeTruthy();
        });
        await userRoleManagementPage.clickOnSaveBtn();
      });

      await test.step(`Verify update successfully`, async () => {
        const actualData = await userRoleManagementPage.getEditUserData(index);
        expect(actualData).toEqual(editUser[index]);
      });
    });
  });
});
