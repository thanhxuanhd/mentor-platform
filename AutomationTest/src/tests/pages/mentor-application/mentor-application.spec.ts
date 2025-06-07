import { expect } from "@playwright/test";
import { test } from "../../../core/fixture/auth-fixture";
import { MentorApplicationStatusTrackingPage } from "../../../pages/mentor-application/mentor-application-status-tracking-page";
import { LoginPage } from "../../../pages/authentication/login-page";
import statusTrackingData from "../../test-data/mentor-application-status-tracking-data.json";
import { requestCreateNewApplication } from "../../../core/utils/api-helper";
import { MentorApplicationReview } from "../../../pages/mentor-application/mentor-application-review-page";
import { loginStep } from "../../../core/utils/login-helper";

test.describe
  .serial("@MentorApplicationStatusTracking All Mentor Status Tracking tests", () => {
  let statusTrackingPage: MentorApplicationStatusTrackingPage;
  let mentorApplicationReview: MentorApplicationReview;
  let loginPage: LoginPage;
  const adminUser = statusTrackingData.admin_role;
  const mentorUser = statusTrackingData.mentor_role;
  const statusApplication = statusTrackingData.tracking_status;
  const statusFilter = statusTrackingData.status_filter;

  test.beforeEach(async ({ loggedInPageByAdminRole, page, request }) => {
    statusTrackingPage = new MentorApplicationStatusTrackingPage(page);
    mentorApplicationReview = new MentorApplicationReview(page);
    loginPage = new LoginPage(page);
    await requestCreateNewApplication(request);
    await test.step("Navigate to applications page", async () => {
      await mentorApplicationReview.navigateToApplicationsPage();
    });
  });

  test("@SmokeTest verify Edit button is enable when Application status = Waiting Info + Admin requests info from Mentor", async ({
    page,
  }) => {
    //Admin requests info from Mentor
    await test.step("Request info of mentor application", async () => {
      await mentorApplicationReview.adminMentorApplicationAction(
        statusFilter.submit,
        mentorUser.mentor_name,
        adminUser.admin_notes,
        statusApplication.waiting_info
      );
      await mentorApplicationReview.verifyNotificationMessage();
    });

    await test.step("Signup to Mentor account", async () => {
      await loginPage.clickOnLogoutButton();
      await loginStep(page, mentorUser);
    });

    //verify Edit button is enable when Application status = Waiting Info
    await test.step("Verify Edit button is enable", async () => {
      await statusTrackingPage.navigateToStatusTrackingPage();
      await statusTrackingPage.clickOnApplication();
      await statusTrackingPage.verifyEditMentorApplicationButtonIsEnable();
    });
  });

  test("@SmokeTest Admin rejects application when Application status = Waiting Info", async ({
    page,
  }) => {
    //Admin rejects mentor application
    await test.step("Signup to Admin account", async () => {
      await loginPage.clickOnLogoutButton();
      await loginStep(page, adminUser);
    });

    await test.step("Navigate to applications page", async () => {
      await mentorApplicationReview.navigateToApplicationsPage();
    });

    await test.step("Reject mentor application", async () => {
      await mentorApplicationReview.adminMentorApplicationAction(
        statusFilter.waiting_info,
        mentorUser.mentor_name,
        adminUser.admin_notes,
        statusApplication.reject
      );
      await mentorApplicationReview.verifyNotificationMessage();
    });
  });

  test("@SmokeTest verify Add button is enable when Application status = Rejected", async ({
    page,
  }) => {
    //Admin rejects mentor application
    await test.step("Request info of mentor application", async () => {
      await mentorApplicationReview.clickOnMentorApplicationAdmin(
        statusTrackingData.mentor_role.mentor_name
      );
      await mentorApplicationReview.clickOnStatusActionButton(
        statusTrackingData.tracking_status.reject
      );
      await mentorApplicationReview.verifyNotificationMessage();
    });

    await test.step("Login as Mentor", async () => {
      await loginPage.clickOnLogoutButton();
      await loginStep(page, mentorUser);
    });

    await test.step("Verify Create New Application button is enable", async () => {
      await statusTrackingPage.navigateToStatusTrackingPage();
      await statusTrackingPage.verifyCreateNewApplicaitonButtonIsEnable();
    });
  });

  test("@SmokeTest Admin rejects application", async () => {
    await test.step("Reject mentor application", async () => {
      await mentorApplicationReview.clickOnMentorApplicationAdmin(
        mentorUser.mentor_name
      );
      await mentorApplicationReview.clickOnStatusActionButton(
        statusApplication.reject
      );
      await mentorApplicationReview.verifyNotificationMessage();
    });
  });

  test("@SmokeTest Admin approves application", async () => {
    await test.step("Approve mentor application", async () => {
      await mentorApplicationReview.clickOnMentorApplicationAdmin(
        mentorUser.mentor_name
      );
      await mentorApplicationReview.clickOnStatusActionButton(
        statusApplication.approve
      );
      await mentorApplicationReview.verifyNotificationMessage();
    });
  });
});
