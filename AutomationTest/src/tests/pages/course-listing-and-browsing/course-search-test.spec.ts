import { CourseListingAndBrowsingPage } from "../../../pages/course-listing-and-browsing/course-listing-and-browsing-page";
import courseData from "../../test-data/course-data.json";
import { test } from "../../../core/fixture/authFixture";
import { ViewCourse } from "../../../models/courses/view-course";
import { withTimestamp } from "../../../core/utils/generate-unique-data";


test.describe("@SmokeTest Search course testcases", () => {
  let courseListingAndBrowsing: CourseListingAndBrowsingPage;

  const courses: { [label: string]: ViewCourse } = {
    "Valid Course": withTimestamp(
      courseData.search_course_with_full_valid_data
    ),
    "Non-existence Test": courseData.search_course_with_non_existence_keyword,
    "Lowercase Test": courseData.search_course_with_lowercase_valid_title,
    "Uppercase Test": courseData.search_course_with_uppercase_valid_title,
    "Empty Search Test": courseData.search_course_with_empty_data,
    "Substrings Search Test": courseData.search_course_with_substrings_data
  };

  test.beforeEach(async ({ loggedInPage, page }) => {
    courseListingAndBrowsing = new CourseListingAndBrowsingPage(page);
    await test.step("Navigate to Courses page", async () => {
      await courseListingAndBrowsing.navigateToCourses();
    });
  });

  for (const [label, data] of Object.entries(courses)) {
    test(`${label} - List and filter course`, async () => {
      await test.step("Perform search course", async () => {
        await courseListingAndBrowsing.searchCourse(data.searchText);
        await courseListingAndBrowsing.selectCategory(data.category);
        await courseListingAndBrowsing.selectLevel(data.difficulty);
        await courseListingAndBrowsing.selectMentor(data.mentor);
        await courseListingAndBrowsing.expectAllCoursesContain(data.searchText);
      });
    });
  }
});
