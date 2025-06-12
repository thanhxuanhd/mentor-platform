import { test } from "../../../core/fixture/auth-fixture";
import { LearnerDashboardPage } from "../../../pages/dashboard/learner-dashboard";

test.describe.serial("@Learner Dashboard tests", () => {
  let learnerPage: LearnerDashboardPage;

  test.beforeEach(async ({ loggedInPageByLearnerRole, page }) => {
    learnerPage = new LearnerDashboardPage(page);
    await test.step("Go to dashboard page", async () => {
      await learnerPage.navigateToDashboardPage();
    });
  });

  test(`@SmokeTestView Verify user cancel reschedule successfully`, async () => {
    await test.step("Click on reschedule session", async () => {
      await learnerPage.clickOnRescheduleSession();
    });
    await test.step("Click on cancel button", async () => {
      await learnerPage.clickOnCancelButton();
    });
    await test.step("Verify user cancel reschedule successfully", async () => {
      await learnerPage.expectSuccessCancelMessage();
    });
  });

  test(`@SmokeTestView Verify user accept reschedule successfully`, async () => {
    await test.step("Click on reschedule session", async () => {
      await learnerPage.clickOnRescheduleSession();
    });
    await test.step("Click on accept button", async () => {
      await learnerPage.clickOnAcceptButton();
    });
    await test.step("Verify user cancel reschedule successfully", async () => {
      await learnerPage.expectSuccessAcceptMessage();
    });
  });
});
