import { expect } from "@playwright/test";
import { test } from "../../../core/fixture/auth-fixture";
import { CoursePage } from "../../../pages/courses/course-management-page";
import { MentorDashboardPage } from "../../../pages/dashboard/mentor-dashboard-page";
import { createTestCategory, createTestCourse } from "../../../core/utils/api-helper";

test.describe.serial("@Mentor Dashboard test", () => {
  let mentorDashboardPage: MentorDashboardPage;
  let coursePage: CoursePage;
  let testCategory: any;

  test.beforeEach(async ({ loggedInPageByMentorRole, page }) => {
    mentorDashboardPage = new MentorDashboardPage(page);
    coursePage = new CoursePage(page);
  });

  test(`@SmokeTest @Regression Verify create new course update the total course`, async ({ page, request }) => {
    let beforeResult: number;
    let afterResult: number;
    let testCourse: any;

    await test.step("Get total course before update", async () => {
      await mentorDashboardPage.goToDashboardPage();
      await page.waitForLoadState("networkidle");
      const beforeValue = await mentorDashboardPage.getTotalCourseValue();
      beforeResult = beforeValue !== null ? Number(beforeValue) : 0;
    });
    await test.step("Create new course", async () => {
      testCategory = await createTestCategory(request);
      testCourse = await createTestCourse(request, testCategory.id)
    });
    await test.step("Verify total course is updated", async () => {
      await mentorDashboardPage.goToDashboardPage();
      await page.waitForLoadState("networkidle");
      const afterValue = await mentorDashboardPage.getTotalCourseValue();
      afterResult = afterValue !== null ? Number(afterValue) : 0;
      expect(afterResult).not.toEqual(beforeResult);
    });
  });

  test(`@SmokeTest @Regression Verify delete course update the total course`, async ({ page }) => {
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
      expect(afterResult).not.toEqual(beforeResult);
    });
  });
});
