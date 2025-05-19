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
          const errorMessage =
            await userRoleManagementPage.getNoDataErrorMessage();
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
});
