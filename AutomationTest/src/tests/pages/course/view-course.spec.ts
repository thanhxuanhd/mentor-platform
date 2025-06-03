import { test } from '../../../core/fixture/auth-fixture';
import { createTestCategory, createTestCourse, deleteTestCategory, deleteTestCourse } from '../../../core/utils/api-helper';
import { CourseViewPage } from '../../../pages/courses/course-view-page';

test.describe('@Course View course tests', () => {
    let coursePage: CourseViewPage;
    let testCourse: any;
    let testCategory: any;

    test.beforeAll("Setup precondition", async ({ request }) => {
        testCategory = await createTestCategory(request);
    });

    test.beforeEach(async ({ loggedInPageByAdminRole, page, request }) => {
        testCourse = await createTestCourse(request, testCategory.id)
        coursePage = new CourseViewPage(page);
        await coursePage.goToCoursePage();
    });

    test("@SmokeTest View Course", async () => {
        await test.step('Input course details', async () => {
            await coursePage.inputKeyword(testCourse.title);
            await coursePage.selectDifficulty(testCourse.difficulty);
            await coursePage.selectMentor(testCourse.mentorName);
            await coursePage.selectStatus(testCourse.status);
            await coursePage.selectCategory(testCourse.categoryName);
        });
        await test.step('Verify system behavior', async () => {
            await coursePage.assertCourseTableIsNotEmpty();
        });
    });

    test.afterEach("Clean up test data", async ({ request }) => {
        await deleteTestCourse(request, testCourse.id);
    });

    test.afterAll("Clean up precondition data", async ({ request }) => {
        await deleteTestCategory(request, testCategory.id);
    })
});