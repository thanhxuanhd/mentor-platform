import { expect } from "@playwright/test";
import { UserRoleManagementPage } from "../../../../pages/user-role-management-page/user-role-management-page";
import userData from "../../../test-data/user-role-management.json";
import { test } from "../../../../core/fixture/authFixture";
import { requestCreateNewUser } from "../../../../core/utils/create-new-user-api";

test.describe("@UserRoleManagement All user role management pagination function", async () => {
  let userRoleManagementPage: UserRoleManagementPage;
  const itemsPerPage = userData.items_per_page;

  test.beforeAll(async ({ request }) => {
    await requestCreateNewUser(request);
  });

  test.beforeEach(async ({ loggedInPage, page }) => {
    userRoleManagementPage = new UserRoleManagementPage(page);
    await test.step("Navigate to User Role Management page", async () => {
      await userRoleManagementPage.navigateToUserRoleManagementPage();
      await userRoleManagementPage.navigateToUsers();
    });
  });

  //Pagination function
  test("@SmokeTest Verify pagination default value", async () => {
    const isTrue = await userRoleManagementPage.getPaginationDefaultValue();
    expect(isTrue).toEqual("5");
  });

  test("@SmokeTest Verify that the Previous button is disable when user is in the first page", async () => {
    await userRoleManagementPage.clickOnNavigationButton(0);
    const isTrue = await userRoleManagementPage.getPreviousButtonStatus();
    expect(isTrue).toBeFalsy();
  });

  test("@SmokeTest Verify that the Next button is disable when user is in the last page", async () => {
    const totalPaging = await userRoleManagementPage.getAllPagingCount();
    await userRoleManagementPage.clickOnNavigationButton(totalPaging - 1);
    const isTrue = await userRoleManagementPage.getNextButtonStatus();
    expect(isTrue).toBeFalsy();
  });

  test("@SmokeTest Verify data changed among pages", async () => {
    const firstPageData = await userRoleManagementPage.getAllUserText();
    await userRoleManagementPage.clickOnNavigationButton(1);
    const secondPageData = await userRoleManagementPage.getAllUserText();
    expect(firstPageData).not.toEqual(secondPageData);
  });

  test("@SmokeTest Verify user list changes when clicking Next and Previous page buttons", async () => {
    const initialPageData = await userRoleManagementPage.getAllUserText();
    console.log("Initial page data: ", initialPageData);
    await userRoleManagementPage.clickOnNextButton();
    const nextPageData = await userRoleManagementPage.getAllUserText();
    expect(nextPageData).not.toEqual(initialPageData);
    console.log("Next page data: ", nextPageData);
    await userRoleManagementPage.clickOnPreviousButton();
    const previousPageData = await userRoleManagementPage.getAllUserText();
    console.log("Previous page data: ", previousPageData);
    expect(previousPageData).toEqual(initialPageData);
  });

  itemsPerPage.forEach((item) => {
    test(`@SmokeTest Verify that user table display ${item} users`, async () => {
      await test.step("Click on items per page dropdown", async () => {
        await userRoleManagementPage.clickOnItemPerPage(item);
      });
      const actualResult = await userRoleManagementPage.getAllTableRowCount();
      expect(actualResult.toString()).toEqual(item);
    });
  });
});
