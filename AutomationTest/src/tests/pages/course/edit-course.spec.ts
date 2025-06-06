import { CreateAndEditCourse } from './../../../models/courses/course';
import courseData from '../../test-data/course-data.json';
import { test } from '../../../core/fixture/auth-fixture';
import { CoursePage } from '../../../pages/courses/course-management-page';
import { endWithTimestamp, withTimestampTitleAndFutureDate } from '../../../core/utils/generate-unique-data';
import { createTestCourse, deleteTestCourse, getLatestCategory, getLatestCourse } from '../../../core/utils/api-helper';

test.describe('@Course Edit course tests', () => {
    let coursePage: CoursePage;
    let courseId: string | null = null;

    test.beforeEach(async ({ loggedInPageByMentorRole, page, request }, testInfo) => {
        if (testInfo.title.includes('@SmokeTest')) {
            const testData = courseData.create_valid_course;
            const tempCourse = {
                title: endWithTimestamp(testData.title),
                description: testData.description,
                categoryId: await getLatestCategory(request),
                difficulty: testData.difficulty,
                tags: testData.tags,
                dueDate: testData.dueDate
            };
            courseId = await createTestCourse(request, tempCourse);
        }
        coursePage = new CoursePage(page);
        await coursePage.goToCoursePage();
        await coursePage.clickUpdateCourseIcon();
    });

    const courses: { [label: string]: CreateAndEditCourse } = {
        '@SmokeTest Valid Course': withTimestampTitleAndFutureDate(courseData.update_valid_course, 1),
        'Duplicate Course': courseData.update_duplicate_course,
        'Empty Course Title': courseData.update_empty_course_title,
        '@Boundary Over length Course Title': courseData.create_over_length_title
    };

    for (const [label, data] of Object.entries(courses)) {
        test(`${label} - Update a Course`, async () => {
            await test.step('Input course details and submit', async () => {
                await coursePage.inputTitle(data.title);
                await coursePage.selectCategory(data.category);
                await coursePage.selectDifficulty(data.difficulty);
                await coursePage.inputDescription(data.description);
                await coursePage.inputTagsName(data.tags);
                await coursePage.selectDueDate(data.dueDate);
                await coursePage.clickSaveChangesButton();
            });
            await test.step('Verify system behavior', async () => {
                await coursePage.expectMessage(data.expectedMessage);
            });
        });
    }

    test.afterEach("Clean up test data", async ({ request }, testInfo) => {
        if (testInfo.title.includes('@SmokeTest')) {
            courseId = await getLatestCourse(request);
            await deleteTestCourse(request, courseId);
            courseId = null;
        }
    });
});
