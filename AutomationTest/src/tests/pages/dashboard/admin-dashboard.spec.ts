import { expect } from "@playwright/test";
import { test } from "../../../core/fixture/auth-fixture";
import { AdminDashboardPage } from "../../../pages/dashboard/admin-dashboard-page";
import searchTerm from "../../test-data/admin-dashboard-data.json";
import { UserRoleManagementPage } from "../../../pages/user-role-management-page/user-role-management-page";

test.describe("@Admin Dashboard test", () => {
  let adminDashboardPage: AdminDashboardPage;
  let userRoleManagementPage: UserRoleManagementPage;
  const searchUserValid = searchTerm;

  test.beforeEach(async ({ loggedInPageByAdminRole, page }) => {
    adminDashboardPage = new AdminDashboardPage(page);
    userRoleManagementPage = new UserRoleManagementPage(page);
  });

  test("@SmokeTest Verify search for Admin report", async ({ page }) => {
    await test.step("Navigate to User Role Management page", async () => {
      await userRoleManagementPage.navigateToUsers();
    });

    await test.step("Perform change user status to get report", async () => {
      const statusList = await userRoleManagementPage.getFirstUserStatus();
      const status = statusList?.toLowerCase();
      if (status) {
        await userRoleManagementPage.clickOnActivateOrDeactivateUserBtn(
          status
        );
      }
    });

    await test.step("Search existed report", async () => {
      await adminDashboardPage.navigateToDashboard();
      await adminDashboardPage.searchActivityReportKeyword(
        searchUserValid.existed_keyword
      );
      await adminDashboardPage.verifyAllActivityReportContain(
        searchUserValid.existed_keyword
      );
    });
  });

  test("@SmokeTest Verify search for non-existed Admin report", async ({
    page,
  }) => {
    await test.step("Search non-existed report", async () => {
      await adminDashboardPage.searchActivityReportKeyword(
        searchUserValid.nonexisted_keyword
      );
      await adminDashboardPage.verifyNoDataMessageExist();
    });
  });
});
