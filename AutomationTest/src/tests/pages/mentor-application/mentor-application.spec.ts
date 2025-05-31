import { expect } from "@playwright/test";
import { test } from "../../../core/fixture/auth-fixture";
import { MentorApplicationStatusTrackingPage } from "../../../pages/mentor-application/mentor-application-status-tracking-page";
import { LoginPage } from "../../../pages/authentication/login-page";
import statusTrackingData from "../../test-data/mentor-application-status-tracking-data.json";
import { requestCreateNewApplication } from "../../../core/utils/api-helper";
import { MentorApplicationReview } from "../../../pages/mentor-application/mentor-application-review-page";

test.describe
  .serial("@MentorApplicationStatusTracking All Mentor Status Tracking tests", () => {
  let statusTrackingPage: MentorApplicationStatusTrackingPage;
  let mentorApplicationReview: MentorApplicationReview;
  let loginPage: LoginPage;
  const adminUser = statusTrackingData.admin_role;
  const mentorUser = statusTrackingData.mentor_role;
  const statusApplication = statusTrackingData.tracking_status;

  test.beforeEach(async ({ loggedInPageByAdminRole, page, request }) => {
    statusTrackingPage = new MentorApplicationStatusTrackingPage(page);
    mentorApplicationReview = new MentorApplicationReview(page);
    loginPage = new LoginPage(page);
    await test.step("Navigate to applications page", async () => {
      await mentorApplicationReview.navigateToApplicationsPage();
    });
    await requestCreateNewApplication(request);
  });

  test("@SmokeTest verify Edit button is enable when Application status = Waiting Info + Admin requests info from Mentor", async () => {
    //Admin requests info from Mentor
    await test.step("Request info of mentor application", async () => {
      await mentorApplicationReview.selectApplicationStatus(
        mentorUser.mentor_name,
        statusApplication.waiting_info
      );
      await mentorApplicationReview.verifyNotificationMessage();
    });

    await test.step("Signup to Mentor account", async () => {
      await loginPage.loginAsRole(mentorUser.email, mentorUser.password);
    });

    //verify Edit button is enable when Application status = Waiting Info
    await test.step("Verify Edit button is enable", async () => {
      await statusTrackingPage.navigateToStatusTrackingPage();
      await statusTrackingPage.verifyEditMentorApplicationButtonIsEnable();
    });

    //Admin rejects mentor application
    await test.step("Signup to Admin account", async () => {
      await loginPage.loginAsRole(adminUser.email, adminUser.password);
    });

    await test.step("Navigate to applications page", async () => {
      await mentorApplicationReview.navigateToApplicationsPage();
    });

    await test.step("Reject mentor application", async () => {
      await mentorApplicationReview.rejectMentorApplication(
        mentorUser.mentor_name,
        "Waiting Info",
        statusApplication.reject
      );
    });
  });

  test("@SmokeTest verify Add button is enable when Application status = Rejected", async () => {
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

    await test.step("Signup to Mentor account", async () => {
      await loginPage.clickOnLogoutButton();
      await loginPage.inputEmail(statusTrackingData.mentor_role.email);
      await loginPage.inputPassword(statusTrackingData.mentor_role.password);
    });

    await test.step("Click Signin button", async () => {
      await loginPage.clickSignInButton();
      await loginPage.expectLogoutButton();
    });

    await test.step("Verify Create New Application button is enable", async () => {
      await statusTrackingPage.navigateToStatusTrackingPage();
      await statusTrackingPage.verifyCreateNewApplicaitonButtonIsEnable();
    });
  });

  test("@SmokeTest Admin rejects application", async () => {
    await test.step("Reject mentor application", async () => {
      await mentorApplicationReview.clickOnMentorApplicationAdmin(
        statusTrackingData.mentor_role.mentor_name
      );
      await mentorApplicationReview.clickOnStatusActionButton(
        statusTrackingData.tracking_status.reject
      );
      await mentorApplicationReview.verifyNotificationMessage();
    });
  });

  test("@SmokeTest Admin approves application", async () => {
    await test.step("Approve mentor application", async () => {
      await mentorApplicationReview.clickOnMentorApplicationAdmin(
        statusTrackingData.mentor_role.mentor_name
      );
      await mentorApplicationReview.clickOnStatusActionButton(
        statusTrackingData.tracking_status.approve
      );
      await mentorApplicationReview.verifyNotificationMessage();
    });
  });
});
