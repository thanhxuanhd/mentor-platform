import { expect } from "@playwright/test";
import { UserRoleManagementPage } from "../../../pages/user-role-management-page/user-role-management-page";
import { requestCreateNewUser } from "../../../core/utils/api-helper";
import { test } from "../../../core/fixture/auth-fixture";
import userData from "../../test-data/user-role-management.json";

test.describe
  .serial("@UserRoleManagement All user role management pagination function", async () => {
  let userRoleManagementPage: UserRoleManagementPage;
  const itemsPerPage = userData.items_per_page;

  test.beforeAll(async ({ request }) => {
    await requestCreateNewUser(request);
  });

  test.beforeEach(async ({ loggedInPageByAdminRole, page }) => {
    userRoleManagementPage = new UserRoleManagementPage(page);
    await test.step("Navigate to User Role Management page", async () => {
      await userRoleManagementPage.navigateToUserRoleManagementPage();
      await userRoleManagementPage.navigateToUsers();
    });
  });

  //Pagination function
  test(" @Regression Verify data changed among pages", async () => {
    await test.step("Verify default value of item per page is 5", async () => {
      const result = await userRoleManagementPage.getPaginationDefaultValue();
      expect(result).toEqual("5 / page");
    });

    await test.step("Verify that the Previous button is disable when user is in the first page", async () => {
      await userRoleManagementPage.clickOnNavigationButton(0);
      const result = await userRoleManagementPage.getPreviousButtonStatus();
      expect(result).toBeFalsy();
    });

    await test.step("Verify that the Next button is disable when user is in the last page", async () => {
      const totalPaging = await userRoleManagementPage.getAllPagingCount();
      await userRoleManagementPage.clickOnNavigationButton(totalPaging - 1);
      const result = await userRoleManagementPage.getNextButtonStatus();
      expect(result).toBeFalsy();
    });

    await test.step("Verify that the datas changes among ages", async () => {
      const firstPageData = await userRoleManagementPage.getAllUserText();
      await userRoleManagementPage.clickOnNavigationButton(1);
      const secondPageData = await userRoleManagementPage.getAllUserText();
      expect(firstPageData).not.toEqual(secondPageData);
    });
  });

  test("Verify user list changes when clicking Next and Previous page buttons", async () => {
    const initialPageData = await userRoleManagementPage.getAllUserText();
    await userRoleManagementPage.clickOnNextButton();

    const nextPageData = await userRoleManagementPage.getAllUserText();
    expect(nextPageData).not.toEqual(initialPageData);

    await userRoleManagementPage.clickOnPreviousButton();
    const previousPageData = await userRoleManagementPage.getAllUserText();

    expect(previousPageData).toEqual(initialPageData);
  });

  itemsPerPage.forEach((item) => {
    test(`Verify that user table display ${item} users`, async () => {
      await test.step("Click on items per page dropdown", async () => {
        await userRoleManagementPage.clickOnItemPerPage(item);
      });
      const actualResult = await userRoleManagementPage.getAllTableRowCount();
      expect(actualResult.toString()).toEqual(item);
    });
  });
});
