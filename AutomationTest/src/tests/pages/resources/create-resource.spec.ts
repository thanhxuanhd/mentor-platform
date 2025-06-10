import { test } from '../../../core/fixture/auth-fixture';
import { createTestCategory, createTestCourse, deleteTestCategory, deleteTestCourse } from '../../../core/utils/api-helper';
import { resolvePaths } from '../../../core/utils/path-resolver';
import { CreateAndEditResource } from '../../../models/courses/resource';
import { ResourcePage } from '../../../pages/resources/resource-management-page';
import resourceData from '../../test-data/resource-mng-data.json'

test.describe('@Resource Create resource tests', () => {
    let resourcePage: ResourcePage;
    let testCourse: any;
    let testCategory: any;

    test.beforeAll("Setup precondition", async ({ request }) => {
        testCategory = await createTestCategory(request);
    });

    test.beforeEach(async ({ loggedInPageByMentorRole, page, request }) => {
        testCourse = await createTestCourse(request, testCategory.id)
        resourcePage = new ResourcePage(page);
        await resourcePage.goToCoursePage();
        await resourcePage.selectResourceModal();
        await resourcePage.clickAddResourceButton();
    });

    const course: { [label: string]: CreateAndEditResource } = {
        '@SmokeTest Valid Resource': resourceData.create_valid_resource,
        'Oversize file': (resourceData.create_oversize_file),
        'Empty Resource Title': (resourceData.create_empty_title),
        'Overlength Title': (resourceData.create_overlength_title),
        'Overlength Description': (resourceData.create_overlength_description),
        '@Boundary Empty Resource File': (resourceData.create_empty_resource)
    };

    for (const [label, data] of Object.entries(course)) {
        test(`${label} - Create a Resource`, async ({ }) => {
            await test.step('Input recourse details and submit', async () => {
                await resourcePage.inputTitle(data.title);
                await resourcePage.inputDescription(data.description);
                const resolvedPaths = resolvePaths(data.fileName);
                await resourcePage.uploadResource(resolvedPaths);
                await resourcePage.clickCreateButton();
            });
            await test.step('Verify system behavior', async () => {
                await resourcePage.expectMessage(data.expectedMessage);
            });
        });
    }

    test.afterEach("Clean up test data", async ({ request }) => {
        await deleteTestCourse(request, testCourse.id);
    });

    test.afterAll("Clean up precondition data", async ({ request }) => {
        await deleteTestCategory(request, testCategory.id);
    })
});