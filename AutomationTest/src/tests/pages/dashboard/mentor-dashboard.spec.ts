import { expect } from "@playwright/test";
import { test } from "../../../core/fixture/auth-fixture";
import { withTimestampTitleAndFutureDate } from "../../../core/utils/generate-unique-data";
import { CoursePage } from "../../../pages/courses/course-management-page";
import { MentorDashboardPage } from "../../../pages/dashboard/mentor-dashboard-page";
import courseData from "../../test-data/course-data.json";
import { MentorApplicationReview } from "../../../pages/mentor-application/mentor-application-review-page";
import statusTrackingData from "../../test-data/mentor-application-status-tracking-data.json";
import { loginStep } from "../../../core/utils/login-helper";
import { LoginUser } from "../../../models/user/user";
import { LoginPage } from "../../../pages/authentication/login-page";

test.describe.serial("@Mentor Dashboard test", () => {
  let mentorDashboardPage: MentorDashboardPage;
  let mentorApplicationReview: MentorApplicationReview;
  let coursePage: CoursePage;
  let loginPage: LoginPage;

  const mentorUser = statusTrackingData.mentor_role;
  const statusApplication = statusTrackingData.tracking_status;
  const validCourse = withTimestampTitleAndFutureDate(
    courseData.create_valid_course,
    2
  );
  const defaultAdmin: LoginUser = {
    email: process.env.ADMIN_USER_NAME!,
    password: process.env.ADMIN_PASSWORD!,
  };

  test.beforeAll(async ({ browser }) => {
    const context = await browser.newContext();
    const page = await context.newPage();
    loginPage = new LoginPage(page);
    mentorApplicationReview = new MentorApplicationReview(page);

    await test.step("Login as Admin", async () => {
      await loginStep(page, defaultAdmin);
    });

    await test.step("Navigate to applications page", async () => {
      await mentorApplicationReview.navigateToApplicationsPage();
    });
    await test.step("Approve mentor application", async () => {
      await mentorApplicationReview.clickOnMentorApplicationAdmin(
        mentorUser.mentor_name
      );
      await mentorApplicationReview.clickOnStatusActionButton(
        statusApplication.approve
      );
      await mentorApplicationReview.verifyNotificationMessage();
    });
    await test.step("Logout as Admin", async () => {
      await loginPage.clickOnLogoutButton();
    });
  });

  test.beforeEach(async ({ loggedInPageByMentorRole, page }) => {
    mentorDashboardPage = new MentorDashboardPage(page);
    coursePage = new CoursePage(page);
  });

  test(`@SmokeTest Verify create new course update the total course`, async ({ page }) => {
    let beforeResult: number;
    let afterResult: number;
    await test.step("Get total course before update", async () => {
      await mentorDashboardPage.goToDashboardPage();
      await page.waitForLoadState("networkidle");
      const beforeValue = await mentorDashboardPage.getTotalCourseValue();
      beforeResult = beforeValue !== null ? Number(beforeValue) : 0;
    });
    await test.step("Create new course", async () => {
      await coursePage.goToCoursePage();
      await coursePage.clickAddNewCourseButton();
      await coursePage.inputTitle(validCourse.title);
      await coursePage.selectCategory(validCourse.category);
      await coursePage.selectDifficulty(validCourse.difficulty);
      await coursePage.inputDescription(validCourse.description);
      await coursePage.clickCreateCourseButton();
    });
    await test.step("Verify total course is updated", async () => {
      await mentorDashboardPage.goToDashboardPage();
      await page.waitForLoadState("networkidle");
      const afterValue = await mentorDashboardPage.getTotalCourseValue();
      afterResult = afterValue !== null ? Number(afterValue) : 0;
      console.log(afterResult + " " + beforeResult);
      expect(afterResult).not.toEqual(beforeResult);
    });
  });

  test(`@SmokeTest Verify delete course update the total course`, async ({ page }) => {
    let beforeResult: number;
    let afterResult: number;
    await test.step("Get total course before update", async () => {
      await mentorDashboardPage.goToDashboardPage();
      await page.waitForLoadState("networkidle");
      const beforeValue = await mentorDashboardPage.getTotalCourseValue();
      beforeResult = beforeValue !== null ? Number(beforeValue) : 0;
    });
    await test.step("Navigate to Delete Category Modal", async () => {
      await coursePage.goToCoursePage();
      await coursePage.clickDeleteCourseIcon();
    });
    await test.step("Confirm delete selected category", async () => {
      await coursePage.clickConfirmDeleteButton();
    });
    await test.step("Verify total course is updated", async () => {
      await mentorDashboardPage.goToDashboardPage();
      await page.waitForLoadState("networkidle");
      const afterValue = await mentorDashboardPage.getTotalCourseValue();
      afterResult = afterValue !== null ? Number(afterValue) : 0;
      console.log(afterResult + " " + beforeResult);
      expect(afterResult).not.toEqual(beforeResult);
    });
  });
});
