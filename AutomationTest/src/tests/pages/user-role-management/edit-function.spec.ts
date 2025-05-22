import { expect } from "@playwright/test";
import { test } from "../../../core/fixture/authFixture";
import userData from "../../test-data/user-role-management.json";
import { UserRoleManagementPage } from "../../../pages/user-role-management-page/user-role-management-page";

test.describe("@UserRoleManagement All user role management testcase", async () => {
  let userRoleManagementPage: UserRoleManagementPage;
  const editUser = userData.validate_edit_user;
  const existedEmail = userData.existed_email;
  const validateEditField = userData.validate_edit_field;

  test.beforeEach(async ({ loggedInPage, page }) => {
    userRoleManagementPage = new UserRoleManagementPage(page);
    await test.step("Navigate to User Role Management page", async () => {
      await userRoleManagementPage.navigateToUserRoleManagementPage();
      await userRoleManagementPage.navigateToUsers();
    });
  });

  //Validate Fullname and Email are mandatory
  validateEditField.forEach((validate, index) => {
    test(`Validate Fullname and Email are mandatory ${index}`, async () => {
      await test.step("Click edit user button", async () => {
        await userRoleManagementPage.clickOnEditUserBtn(0);
      });
      await test.step("Fill edit user form with empty field", async () => {
        await userRoleManagementPage.fillEditUserForm("", "");
      });
      await test.step("Verify error message is shown", async () => {
        const errorMessage = await userRoleManagementPage.getEditErrorMessage();
        expect(validate.invalid_message).toContain(errorMessage[index]);
      });
    });
  });

  //Validate Email and Fullname default value
  test("Validate Email and Fullname default value", async () => {
    await test.step("Click edit user button", async () => {
      await userRoleManagementPage.clickOnEditUserBtn(0);
    });
    await test.step("Verify default value of Fullname and Email", async () => {
      const actualData =
        await userRoleManagementPage.verifyEditUserDefaultValue();
      expect(actualData).toBeTruthy();
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

      await test.step(`Click on save button`, async () => {
        await test.step("Verify save button is enable", async () => {
          const isEnable =
            await userRoleManagementPage.getSaveButtonIsEnabled();
          expect(isEnable).toBeTruthy();
        });
        await test.step("Verify cancel button is enabled", async () => {
          const isEnable =
            await userRoleManagementPage.getCancelButtonIsEnabled();
          expect(isEnable).toBeTruthy();
        });
        await userRoleManagementPage.clickOnSaveBtn();
      });

      await test.step(`Verify update user successfully`, async () => {
        const actualData = await userRoleManagementPage.getNotification();
        expect(actualData).toEqual("User updated successfully.");
      });
    });
  });

  //Check error message when enter user email is exists in edit user
  test("@Flaky Verify that error message is shown when user email already exists", async () => {
    await test.step("Click edit user button", async () => {
      await userRoleManagementPage.clickOnEditUserBtn(1); //TODO: need to make index dynamic after API is able to create fullname
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
    await test.step("Click edit user button", async () => {
      await userRoleManagementPage.clickOnEditUserBtn(2); //TODO: need to make index dynamic after API is able to create fullname
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
