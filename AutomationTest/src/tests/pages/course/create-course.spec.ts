import { CreateAndEditCourse } from './../../../models/courses/course';
import courseData from '../../test-data/course-data.json';
import { test } from '../../../core/fixture/auth-fixture';
import { CoursePage } from '../../../pages//courses/course-management-page';
import { withFutureDate, withTimestampTitleAndFutureDate } from '../../../core/utils/generate-unique-data';
import { deleteTestCourse, getLatestCourse } from '../../../core/utils/api-helper';

test.describe('@Course Create course tests', () => {
    let coursePage: CoursePage;
    let courseId: string | null = null;

    test.beforeEach(async ({ loggedInPageByMentorRole, page }) => {
        coursePage = new CoursePage(page);
        await coursePage.goToCoursePage();
        await coursePage.clickAddNewCourseButton();
    });

    const course: { [label: string]: CreateAndEditCourse } = {
        '@SmokeTest Valid Course': withTimestampTitleAndFutureDate(courseData.create_valid_course, 2),
        'Empty Course Title': (courseData.create_empty_title),
        'Over length Course Title': (courseData.create_over_length_title),
        'Duplicate Course': withFutureDate(courseData.create_duplicate_course),
        'Over length description': (courseData.create_over_length_description)
    };

    for (const [label, data] of Object.entries(course)) {
        test(`${label} - Create a Course`, async () => {
            await test.step('Input course details and submit', async () => {
                await coursePage.inputTitle(data.title);
                await coursePage.selectCategory(data.category);
                await coursePage.selectDifficulty(data.difficulty);
                await coursePage.inputDescription(data.description);
                await coursePage.inputTagsName(data.tags);
                await coursePage.clickCreateCourseButton();
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