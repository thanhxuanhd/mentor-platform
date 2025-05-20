import test, { expect } from "@playwright/test";
import { UserRoleManagementPage } from "../../../../pages/user-role-management-page/user-role-management-page";
import userData from "../test-data/user-role-management.json";

test.describe("@UserRoleManagement All user role management testcase", async () => {
  let userRoleManagementPage: UserRoleManagementPage;
  const editUser = userData.validate_edit_user;
  const existedEmail = userData.existed_email;

  test.beforeEach(async ({ page }) => {
    userRoleManagementPage = new UserRoleManagementPage(page);
    await test.step("Navigate to User Role Management page", async () => {
      await userRoleManagementPage.navigateToUserRoleManagementPage();
      await userRoleManagementPage.navigateToUsers();
    });
  });

  //Validate Fullname
  test("Validate Fullname", async () => {});

  //Validate Email
  test("Validate Email", async () => {});

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
        const actualData = await userRoleManagementPage.getNotification(0);
        expect(actualData).toEqual("User updated successfully.");
      });
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
      const errorMessage = await userRoleManagementPage.getNotification(0);
      expect(errorMessage).toBe(`Email ${existedEmail.email} already exists.`);
    });
  });
});
