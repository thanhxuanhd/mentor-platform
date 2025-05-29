import { expect } from "@playwright/test";
import { CourseListingAndBrowsingPage } from "../../../pages/course-listing-and-browsing/course-listing-and-browsing-page";
import { test } from "../../../core/fixture/authFixture";

test.describe("@SmokeTest Check pagination function", async () => {
  let courseListingAndBrowsing: CourseListingAndBrowsingPage;

  test.beforeEach(async ({ loggedInPage, page }) => {
    courseListingAndBrowsing = new CourseListingAndBrowsingPage(page);
    await test.step("Navigate to Course Listing and Browsing page", async () => {
      await courseListingAndBrowsing.navigateToCourses();
    });
  });
  
  //Pagination function
  test("Verify that the Previous button is disable when user is in the first page", async () => {
    await courseListingAndBrowsing.clickOnNavigationButton(0);
    const result = await courseListingAndBrowsing.getPreviousButtonStatus();
    expect(result).toBeFalsy();
  });

  test("Verify that the Next button is disable when user is in the last page", async () => {
    const totalPaging = await courseListingAndBrowsing.getAllPagingCount();
    await courseListingAndBrowsing.clickOnNavigationButton(totalPaging - 1);
    const result = await courseListingAndBrowsing.getNextButtonStatus();
    expect(result).toBeFalsy();
  });


  test("Verify data changed among pages", async () => {
    const firstPageData = await courseListingAndBrowsing.getAllCourse();
    await courseListingAndBrowsing.clickOnNavigationButton(1);
    const secondPageData = await courseListingAndBrowsing.getAllCourse();
    expect(firstPageData).not.toEqual(secondPageData);
  });
});