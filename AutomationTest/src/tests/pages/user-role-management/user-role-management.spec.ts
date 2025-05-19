import test, { expect } from "@playwright/test";
import { UserRoleManagementPage } from "../../../pages/user-role-management-page/user-role-management-page";
import userData from "./user-role-management.json";

test.describe("@UserRoleManagement All user role management testcase", async () => {
  let userRoleManagementPage: UserRoleManagementPage;
  const editUser = userData.validate_edit_user;
  const searchUser = userData.search_user;
  const userRoleFilter = userData.search_with_filter.user_roles;
  const searchUserWithFilter = userData.search_with_filter.search_filters;
  const itemsPerPage = userData.items_per_page;
  const existedEmail = userData.existed_email;

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
        await userRoleManagementPage.clickOnSaveBtn();
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
      });

      await test.step(`Verify update successfully`, async () => {
        const actualData = await userRoleManagementPage.getEditUserData(index);
        expect(actualData).toEqual(editUser[index]);
      });
    });
  });

  //Search user
  searchUser.forEach((user, index) => {
    test(`@SmokeTest Verify search user successfully ${index};`, async () => {
      await test.step("Perform search user", async () => {
        await userRoleManagementPage.searchUser();
      });

      await test.step("Search user", async () => {
        if (user.fullname) {
          await userRoleManagementPage.searchUser(user.fullname);
          await userRoleManagementPage.expectAllUsersContain(user.fullname);
        } else if (user.oneKeyword) {
          await userRoleManagementPage.searchUser(user.oneKeyword);
          await userRoleManagementPage.expectAllUsersContain(user.oneKeyword);
        } else if (user.multipleKeywords) {
          await userRoleManagementPage.searchUser(user.multipleKeywords);
          await userRoleManagementPage.expectAllUsersContain(
            user.multipleKeywords
          );
        } else if (user.invalidKeyword) {
          await userRoleManagementPage.searchUser(user.invalidKeyword);
          const errorMessage = await userRoleManagementPage.getErrorMessage();
          expect(errorMessage).toBe("No data");
        } else if (user.wildCard) {
          await userRoleManagementPage.searchUser(user.wildCard);
          await userRoleManagementPage.expectAllUsersContain(user.wildCard);
        } else if (user.empty) {
          const actualResult =
            await userRoleManagementPage.getAllTableRowCount();
          await userRoleManagementPage.searchUser(user.empty);
          const expectedResult =
            await userRoleManagementPage.expectAllUsersContain(user.empty);
          expect(actualResult).toEqual(expectedResult);
        }
      });
    });
  });

  userRoleFilter.forEach((role, index) => {
    test(`@SmokeTest Verify search user with filter successfully ${index}`, async () => {
      await test.step("Perform search user", async () => {
        await userRoleManagementPage.searchUser();
      });

      await test.step("Search user", async () => {
        await userRoleManagementPage.filterUserByRole(role);
        await test.step("Verify that the filter is applied successfully", async () => {
          const actualResult = await userRoleManagementPage.getUserRole(index);
          expect(actualResult).toEqual(role);
        });
        await userRoleManagementPage.searchUser(
          searchUserWithFilter.oneKeyword
        );
        await userRoleManagementPage.expectAllUsersContain(
          searchUserWithFilter.oneKeyword
        );
      });
    });
  });

  //Pagination function
  test("@SmokeTest Verify pagination default value", async () => {
    const isTrue = await userRoleManagementPage.getPaginationDefaultValue();
    expect(isTrue).toEqual("5");
  });

  test("Verify that the Previous button is disable when user is in the first page", async () => {
    await userRoleManagementPage.clickOnNavigationButton(0);
    const isTrue = await userRoleManagementPage.getPreviousButtonStatus();
    expect(isTrue).toBeFalsy();
  });

  test("Verify that the Next button is disable when user is in the last page", async () => {
    const totalPaging = await userRoleManagementPage.getAllPagingCount();
    await userRoleManagementPage.clickOnNavigationButton(totalPaging - 1);
    const isTrue = await userRoleManagementPage.getNextButtonStatus();
    expect(isTrue).toBeFalsy();
  });

  test("@SmokeTest Verify Next button is enable if user stays from Page 2 to the last page", async () => {
    await userRoleManagementPage.clickOnNavigationButton(1);
    const isTrue = await userRoleManagementPage.getNextButtonStatus();
    expect(isTrue).toBeTruthy();
  });

  test("@SmokeTest Verify Previous button is enable if user stays from the page before the last page", async () => {
    const totalPaging = await userRoleManagementPage.getAllPagingCount();
    await userRoleManagementPage.clickOnNavigationButton(totalPaging - 2);
    const isTrue = await userRoleManagementPage.getNextButtonStatus();
    expect(isTrue).toBeTruthy();
  });

  test("@SmokeTest Verify data changed among pages", async () => {
    const firstPageData = await userRoleManagementPage.getAllUser();
    await userRoleManagementPage.clickOnNavigationButton(1);
    const secondPageData = await userRoleManagementPage.getAllUser();
    expect(firstPageData).not.toEqual(secondPageData);
  });

  itemsPerPage.forEach((item) => {
    test(`@SmokeTest Verify that user table display ${item} users`, async () => {
      console.log("item: ", item);
      await test.step("Click on items per page dropdown", async () => {
        await userRoleManagementPage.clickOnItemPerPage(item);
      });
      const actualResult = await userRoleManagementPage.getAllTableRowCount();
      expect(actualResult.toString()).toEqual(item);
    });
  });

  //Activate/Deactivate user
  test("@SmokeTest Verify that Activate/Deactivate user works correctly", async () => {
    const rowCount = await userRoleManagementPage.getAllTableRowCount();

    for (let i = 0; i < rowCount; i++) {
      const status = await userRoleManagementPage.getUserStatus(i);

      if (status === "Pending" || status === "Deactivated") {
        await test.step(`Activate user in row ${i}`, async () => {
          await userRoleManagementPage.clickOnDeactivateUserBtn(i);
          await expect(userRoleManagementPage.getUserStatus(i)).resolves.toBe(
            "Active"
          );
        });
      } else if (status === "Active") {
        await test.step(`Deactivate user in row ${i}`, async () => {
          await userRoleManagementPage.clickOnActivateUserBtn(i);
          await expect(userRoleManagementPage.getUserStatus(i)).resolves.toBe(
            "Deactivated"
          );
        });
      }
    }
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
      const errorMessage =
        await userRoleManagementPage.getEmailAlreadyExistsErrorMessage();
      expect(errorMessage).toBe(`Email ${existedEmail.email} already exists.`);
    });
  });
});
