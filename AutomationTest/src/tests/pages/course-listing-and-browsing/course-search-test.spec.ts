import { expect } from "@playwright/test";
import { CourseListingAndBrowsingPage } from "../../../pages/course-listing-and-browsing/course-listing-and-browsing-page";
import courseData from "./course-data.json";
import { test } from "../../../core/fixture/authFixture";

test.describe("@courseListingAndBrowsing All course listing and browsing testcase", async () => {
  let courseListingAndBrowsing: CourseListingAndBrowsingPage;
  const searchCourse = courseData.search_course;

  test.beforeEach(async ({ loggedInPage, page }) => {
    courseListingAndBrowsing = new CourseListingAndBrowsingPage(page);
    await test.step("Navigate to Courses page", async () => {
      await courseListingAndBrowsing.navigateToCourseListingAndBrowsingPage();
      await courseListingAndBrowsing.navigateToCourses();
    });
  });

  //Search course
  searchCourse.forEach((Course, index) => {
    test(`@SmokeTest Verify search course successfully ${index};`, async () => {
      await test.step("Perform search course", async () => {
        await courseListingAndBrowsing.searchCourse();
      });

      await test.step("Search course", async () => {
        if (Course.fullname) {
          await courseListingAndBrowsing.searchCourse(Course.fullname);
          await courseListingAndBrowsing.expectAllCoursesContain(
            Course.fullname
          );
        } else if (Course.oneKeyword) {
          await courseListingAndBrowsing.searchCourse(Course.oneKeyword);
          await courseListingAndBrowsing.expectAllCoursesContain(
            Course.oneKeyword
          );
        } else if (Course.multipleKeywords) {
          await courseListingAndBrowsing.searchCourse(Course.multipleKeywords);
          await courseListingAndBrowsing.expectAllCoursesContain(
            Course.multipleKeywords
          );
        } else if (Course.invalidKeyword) {
          await courseListingAndBrowsing.searchCourse(Course.invalidKeyword);
          const errorMessage =
            await courseListingAndBrowsing.getNoDataErrorMessage();
          expect(errorMessage).toBe("No data");
        } else if (Course.wildCard) {
          await courseListingAndBrowsing.searchCourse(Course.wildCard);
          await courseListingAndBrowsing.expectAllCoursesContain(
            Course.wildCard
          );
        } else if (Course.empty) {
          const actualResult =
            await courseListingAndBrowsing.getAllTableRowCount();
          await courseListingAndBrowsing.searchCourse(Course.empty);
          const expectedResult =
            await courseListingAndBrowsing.expectAllCoursesContain(
              Course.empty
            );
          expect(actualResult).toEqual(expectedResult);
        }
      });
    });
  });

  searchCourse.forEach((Course, index) => {
    test(`@SmokeTest Verify search Course with filter successfully ${index}`, async () => {
      await test.step("Perform search course", async () => {
        await courseListingAndBrowsing.searchCourse();
        await courseListingAndBrowsing.getAllCourse();
      });

      await test.step("Search Course by Category filter", async () => {
        await courseListingAndBrowsing.selectCategory();
        await courseListingAndBrowsing.getAllCourse();
      });

      await test.step("Search Course by Level filter", async () => {
        await courseListingAndBrowsing.selectLevel();
        await courseListingAndBrowsing.getAllCourse();
      });

      await test.step("Search Course by Mentor filter", async () => {
        await courseListingAndBrowsing.selectMentor();
        await courseListingAndBrowsing.getAllCourse();
      });
    });
  });
});
