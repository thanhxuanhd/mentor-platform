import { test } from '../../../core/fixture/auth-fixture';
import { createTestCategory, createTestCourse, deleteTestCategory, deleteTestCourse } from '../../../core/utils/api-helper';
import { createAndVerifyResource } from '../../../core/utils/path-resolver';
import { ResourcePage } from '../../../pages/resources/resource-management-page';
import resourceData from '../../test-data/resource-mng-data.json'

test.describe('@ResourceView Browsing resources tests', () => {
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
        await resourcePage.goToResoursePage();
    });

    test("View Resource", async ({ }) => {
        await test.step('Select test resource and browing', async () => {
            await resourcePage.selectCategory(testCategory.name);
        });
        await test.step('Verify system behavior', async () => {
            await resourcePage.expectListResourceIsNotNull();
        });
    });

    test.afterEach("Clean up test data", async ({ request }) => {
        await deleteTestCourse(request, testCourse.id);
    });

    test.afterAll("Clean up precondition data", async ({ request }) => {
        await deleteTestCategory(request, testCategory.id);
    })
});