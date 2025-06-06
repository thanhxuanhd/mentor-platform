import { CoursePage } from '../../../pages/courses/course-management-page';
import { test } from '../../../core/fixture/auth-fixture';
import courseData from '../../test-data/course-data.json'
import { createTestCategory, createTestCourse, deleteTestCategory, getLatestCategory } from '../../../core/utils/api-helper';
import { endWithTimestamp } from '../../../core/utils/generate-unique-data';

test.describe('@Course Delete course tests', () => {
    let coursePage: CoursePage;
    let categoryId: string;

    test.beforeAll("Setup precondition", async ({ request }) => {
        categoryId = await createTestCategory(request);
    });

    test.beforeEach(async ({ loggedInPageByMentorRole, page, request }) => {
        coursePage = new CoursePage(page);
        await createTestCourse(request, categoryId);
    });
    test('@SmokeTest @Course Delete a valid course successful', async ({ page }) => {
        coursePage = new CoursePage(page);
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

    test.afterAll("Clean up precondition data", async ({ request }) => {
        await deleteTestCategory(request, categoryId);
    })
});