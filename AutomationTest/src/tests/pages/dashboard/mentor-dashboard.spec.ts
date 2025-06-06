import { test } from "../../../core/fixture/auth-fixture";
import { MentorDashBoardPage } from "../../../pages/dashboard/mentor-dashboard-page";

test.describe("@Mentor Dashboard", () => {
  let mentorDashBoard: MentorDashBoardPage;
  test.beforeEach(async ({ loggedInPageByLearnerRole, page }) => {
    mentorDashBoard = new MentorDashBoardPage(page);
  });

  test("@SmokeTest Verify start date is after today", async ({ page }) => {
    await test.step("Go to dashboard page", async () => {
      await mentorDashBoard.goToDashboardPage();
    });

    await test.step("Verify start date is after today", async () => {
      await mentorDashBoard.verifyStartDateIsAfterCurrentDate();
    });
  });
});
