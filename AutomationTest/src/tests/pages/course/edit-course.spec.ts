import { CUCourse } from './../../../models/courses/course';
import courseData from '../../test-data/course-data.json';
import { test } from '../../../core/fixture/authFixture';
import { CoursePage } from '../../../pages/courses/course-management-page';
import { withFutureDate } from '../../../core/utils/generate-unique-data';

test.describe('@Course Edit course tests', () => {
    let coursePage: CoursePage;

    test.beforeEach(async ({ loggedInPage, page }) => {
        coursePage = new CoursePage(page);
        await coursePage.goToCoursePage();
        await coursePage.clickUpdateCourseIcon();
    });

    const courses: { [label: string]: CUCourse } = {
        '@SmokeTest Valid Course': withFutureDate(courseData.update_valid_course, 1),
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
});
