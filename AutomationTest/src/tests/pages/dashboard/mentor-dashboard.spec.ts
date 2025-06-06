import { expect } from "@playwright/test";
import { test } from "../../../core/fixture/auth-fixture";
import { withTimestampTitleAndFutureDate } from "../../../core/utils/generate-unique-data";
import { CoursePage } from "../../../pages/courses/course-management-page";
import { MentorDashboardPage } from "../../../pages/dashboard/mentor-dashboard-page";
import courseData from "../../test-data/course-data.json";

test.describe.serial("@Mentor Dashboard test", () => {
  let mentorDashboardPage: MentorDashboardPage;
  let coursePage: CoursePage;
  const validCourse = withTimestampTitleAndFutureDate(
    courseData.create_valid_course,
    2
  );

  test.beforeEach(async ({ loggedInPageByMentorRole, page }) => {
    mentorDashboardPage = new MentorDashboardPage(page);
    coursePage = new CoursePage(page);
  });

  test(`Verify create new course update the total course`, async ({ page }) => {
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

  test(`Verify delete course update the total course`, async ({ page }) => {
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
