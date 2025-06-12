import { test } from "../../../core/fixture/auth-fixture";
import { LearnerDashboardPage } from "../../../pages/dashboard/learner-dashboard";

test.describe.serial("@Learner Dashboard tests", () => {
  let learnerDashboardPage: LearnerDashboardPage;

  test.beforeEach(async ({ loggedInPageByLearnerRole, page }) => {
    learnerDashboardPage = new LearnerDashboardPage(page);
    await test.step("Go to dashboard page", async () => {
      await learnerDashboardPage.navigateToDashboardPage();
    });
  });

  test(`Verify user cancel reschedule successfully`, async () => {
    await test.step("Click on reschedule session", async () => {
      await learnerDashboardPage.clickOnRescheduleSession();
    });
    await test.step("Click on cancel button", async () => {
      await learnerDashboardPage.clickOnCancelButton();
    });
    await test.step("Verify user cancel reschedule successfully", async () => {
      await learnerDashboardPage.expectSuccessCancelMessage();
    });
  });

  test(`Verify user accept reschedule successfully`, async () => {
    await test.step("Click on reschedule session", async () => {
      await learnerDashboardPage.clickOnRescheduleSession();
    });
    await test.step("Click on accept button", async () => {
      await learnerDashboardPage.clickOnAcceptButton();
    });
    await test.step("Verify user cancel reschedule successfully", async () => {
      await learnerDashboardPage.expectSuccessAcceptMessage();
    });
  });
});
