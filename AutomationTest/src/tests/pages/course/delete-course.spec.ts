import { CoursePage } from '../../../pages/courses/course-management-page';
import { test } from '../../../core/fixture/auth-fixture';
import courseData from '../../test-data/course-data.json'
import { createTestCourse, getLatestCategory } from '../../../core/utils/api-helper';
import { endWithTimestamp } from '../../../core/utils/generate-unique-data';

test.describe('@Course Delete course tests', () => {
    let coursePage: CoursePage;

    test.beforeEach(async ({ loggedInPageByMentorRole, page, request }) => {
        coursePage = new CoursePage(page);
        const testData = courseData.create_valid_course;
        const tempCourse = {
            title: endWithTimestamp(testData.title),
            description: testData.description,
            categoryId: await getLatestCategory(request),
            difficulty: testData.difficulty,
            tags: testData.tags,
            dueDate: testData.dueDate
        };
        await createTestCourse(request, tempCourse);
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
});