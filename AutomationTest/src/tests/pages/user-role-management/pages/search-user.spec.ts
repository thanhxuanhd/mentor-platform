import { expect } from "@playwright/test";
import { UserRoleManagementPage } from "../../../../pages/user-role-management-page/user-role-management-page";
import userData from "../../../test-data/user-role-management.json";
import { test } from "../../../../core/fixture/authFixture";

test.skip("@UserRoleManagement All user role management testcase", async () => {
  let userRoleManagementPage: UserRoleManagementPage;
  const searchUserValid = userData.search_user.valid_keywords;
  const searchUserInvalid = userData.search_user.invalid_keywords;
  const searchUserEmpty = userData.search_user.empty_keyword;
  const searchUserWithFilter = userData.search_with_filter;

  test.beforeEach(async ({ loggedInPage, page }) => {
    userRoleManagementPage = new UserRoleManagementPage(page);
    await test.step("Navigate to User Role Management page", async () => {
      await userRoleManagementPage.navigateToUserRoleManagementPage();
      await userRoleManagementPage.navigateToUsers();
    });
  });

  //Search user

  test(`Verify search user default value is empty`, async () => {
    const isEmpty =
      await userRoleManagementPage.verifyInitialValueOfSearchUser();
    expect(isEmpty).toBe("");
  });

  test(`@SmokeTest Verify search user successfully with Fullname`, async () => {
    await userRoleManagementPage.searchUser(searchUserValid);
    await userRoleManagementPage.expectAllUsersContain(searchUserValid);
  });

  test(`@SmokeTest Verify search user successfully with empty value`, async () => {
    const actualResult = await userRoleManagementPage.getAllTableRowCount();
    await userRoleManagementPage.searchUser(searchUserEmpty);
    const expectedResult = await userRoleManagementPage.getAllTableRowCount();
    expect(actualResult).toEqual(expectedResult);
  });

  test(`Verify search user failed`, async () => {
    await userRoleManagementPage.searchUser(searchUserInvalid.input);
    const actualData = await userRoleManagementPage.getNoDataErrorMessage();
    expect(actualData).toEqual(searchUserInvalid.message);
  });

  searchUserWithFilter.forEach((item, index) => {
    test(`@SmokeTest Verify search user with ${item.filter} filter successfully`, async () => {
      await test.step("Verify default user role value", async () => {
        const isAll = await userRoleManagementPage.getUserRoleTabDefaultValue();
        expect(isAll).toBe("All");
      });

      await test.step("Search user", async () => {
        await userRoleManagementPage.filterUserByRole(item.filter);

        //Validate User Roles Filter
        await test.step("Verify that the filter is applied successfully", async () => {
          const actualResult = await userRoleManagementPage.getUserRole(index);
          expect(actualResult).toEqual(item.filter);
        });

        //Validate Search User
        await test.step(`Verify search with keyword ${item.multipleKeywords} successfully`, async () => {
          await userRoleManagementPage.searchUser(item.multipleKeywords);
          await userRoleManagementPage.expectAllUsersContain(
            item.multipleKeywords
          );
        });
      });
    });
  });
});
