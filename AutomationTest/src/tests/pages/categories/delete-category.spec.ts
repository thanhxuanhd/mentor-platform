import { CategoryPage } from '../../../pages/categories/categories-page';
import { test } from '../../../core/fixture/auth-fixture';
import categoryData from '../../test-data/category-data.json'
import { createTestCategory } from '../../../core/utils/api-helper';
import { endWithTimestamp } from '../../../core/utils/generate-unique-data';

test.describe('@Category Delete category tests', () => {
    let categoryPage : CategoryPage;

    test.beforeEach(async ({ loggedInPageByAdminRole, page, request }) => {
        categoryPage = new CategoryPage(page);
        await createTestCategory(request);
    });

    test('@SmokeTest @Category Delete a valid category successful', async ({ page }) => {
        categoryPage = new CategoryPage(page);

        await test.step('Navigate to Delete Category Modal', async () => {
            await categoryPage.navigateToHomePage();
            await categoryPage.goToCategoryPage();
            await categoryPage.clickDeleteCategoryButton();
        });

        await test.step('Confirm delete selected category', async () => {
            await categoryPage.clickConfirmDeleteButton();
        });

        await test.step('Verify system behavior', async () => {
            await categoryPage.expectSucessDeleteMessage();
        });
    });
});