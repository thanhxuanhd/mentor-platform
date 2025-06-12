import { test } from '../../../core/fixture/auth-fixture';
import { createTestCategory, createTestCourse, deleteTestCategory, deleteTestCourse } from '../../../core/utils/api-helper';
import { createAndVerifyResource, resolvePaths } from '../../../core/utils/path-resolver';
import { CreateAndEditResource } from '../../../models/courses/resource';
import { ResourcePage } from '../../../pages/resources/resource-management-page';
import resourceData from '../../test-data/resource-mng-data.json'

test.describe('@Resource Edit resource tests', () => {
    let resourcePage: ResourcePage;
    let testCourse: any;
    let testCategory: any;

    test.beforeAll("Setup precondition", async ({ request }) => {
        testCategory = await createTestCategory(request);
    });

    test.beforeEach(async ({ loggedInPageByMentorRole, page, request }) => {
        testCourse = await createTestCourse(request, testCategory.id)
        resourcePage = new ResourcePage(page);
        await createAndVerifyResource(resourceData.create_valid_resource, resourcePage);
        await resourcePage.goToCoursePage();
        await resourcePage.selectResourceModal();
        await resourcePage.clickEditIcon();
    });

    const course: { [label: string]: CreateAndEditResource } = {
        '@SmokeTest Valid Resource': resourceData.update_valid_resource,
        'Oversize file': (resourceData.update_oversize_file),
        'Empty Resource Title': (resourceData.update_empty_title),
        'Overlength Title': (resourceData.update_overlength_title),
        'Overlength Description': (resourceData.update_overlength_description),
        '@Boundary Empty Resource File': (resourceData.update_empty_resource)
    };

    for (const [label, data] of Object.entries(course)) {
        test(`${label} - Edit a Resource`, async ({ }, testInfo) => {
            await test.step('Input recourse details and submit', async () => {
                await resourcePage.inputTitle(data.title);
                await resourcePage.inputDescription(data.description);
                const resolvedPaths = resolvePaths(data.fileName);
                await resourcePage.uploadResource(resolvedPaths);
                if (testInfo.title.includes("Empty")) {
                    await resourcePage.clickDeleteFile();
                }
                await resourcePage.clickUpdateButton();
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