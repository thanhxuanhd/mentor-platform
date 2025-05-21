import { expect } from "@playwright/test";
import { UserRoleManagementPage } from "../../../../pages/user-role-management-page/user-role-management-page";
import { User } from "../../../../models/edi-tuser/edit-user";
import { test } from "../../../../core/fixture/authFixture";

test.describe("@UserRoleManagement All user role management testcase", async () => {
  let userRoleManagementPage: UserRoleManagementPage;

  test.beforeEach(async ({ loggedInPage, page }) => {
    userRoleManagementPage = new UserRoleManagementPage(page);
    await test.step("Navigate to User Role Management page", async () => {
      await userRoleManagementPage.navigateToUserRoleManagementPage();
      await userRoleManagementPage.navigateToUsers();
    });
  });

  test("@SmokeTest Verify detailed user information is displayed correctly", async () => {
    const actualUserData = await userRoleManagementPage.viewOneUser();

    const userData: User = {
      username: (await actualUserData.fullname) ?? "",
      email: (await actualUserData.email) ?? "",
      role: (await actualUserData.role) ?? "",
      joinDate: (await actualUserData.joinedDate) ?? "",
      lastActive: (await actualUserData.lastActiveDate) ?? "",
      status: (await actualUserData.userStatus) ?? "",
    };

    expect(userData.username).not.toBe("");
    expect(userData.email).toMatch(/@/);
    expect(["Admin", "Mentor", "Learner"]).toContain(userData.role);
    expect(userData.joinDate).toMatch(/\d{2}\/\d{2}\/\d{4}/);
    expect(["Active", "Deactivated", "Pending"]).toContain(userData.status);
  });
});
