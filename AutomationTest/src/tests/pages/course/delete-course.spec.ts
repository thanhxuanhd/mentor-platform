import { CoursePage } from '../../../pages/courses/course-management-page';
import { test } from '../../../core/fixture/authFixture';

test('@SmokeTest @Course Delete a valid course sucessful', async ({ loggedInPage, page }) => {
    const coursePage = new CoursePage(page);

    await test.step('Navigate to Delete Category Modal', async () => {
        await coursePage.navigateToHomePage();
        await coursePage.goToCoursePage();
        await coursePage.clickDeleteCourseIcon();
    });

    await test.step('Confirm delete selected category', async () => {
        await coursePage.clickConfirmDeleteButton();
    });

    await test.step('Verify system behavior', async () => {
        await coursePage.expectSucessDeleteMessage();
    });
});