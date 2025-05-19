import test, { expect } from "@playwright/test";
import { UserRoleManagementPage } from "../../../pages/user-role-management-page/user-role-management-page";
import userData from "./user-role-management.json";

test.describe("@UserRoleManagement All user role management pagination function", async () => {
  let userRoleManagementPage: UserRoleManagementPage;
  const itemsPerPage = userData.items_per_page;

  test.beforeEach(async ({ page }) => {
    userRoleManagementPage = new UserRoleManagementPage(page);
    await test.step("Navigate to User Role Management page", async () => {
      await userRoleManagementPage.navigateToUserRoleManagementPage();
      await userRoleManagementPage.navigateToUsers();
    });
  });

  //Pagination function
  test("Verify pagination default value", async () => {
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

  test("Verify Next button is enable if user stays from Page 2 to the last page", async () => {
    await userRoleManagementPage.clickOnNavigationButton(1);
    const isTrue = await userRoleManagementPage.getNextButtonStatus();
    expect(isTrue).toBeTruthy();
  });

  test("Verify Previous button is enable if user stays from the page before the last page", async () => {
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
});
