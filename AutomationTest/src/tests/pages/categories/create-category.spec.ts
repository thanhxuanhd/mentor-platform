import categoryData from '../../test-data/category-data.json';
import { test } from '../../../core/fixture/authFixture';
import { CategoryPage } from '../../../pages/categories/categories-page';
import { withTimestamp } from '../../../core/utils/generate-unique-data';
import { CUCategory } from '../../../models/categories/create-category';
import { buildTestCases } from '../../../core/utils/testcase-data-builder';

test.describe('@Category Create category tests', () => {
    let categoryPage: CategoryPage;

    test.beforeEach(async ({ loggedInPage, page }) => {
        categoryPage = new CategoryPage(page);
        await categoryPage.goToCategoryPage();
        await categoryPage.clickAddCategoryButton();
    });

    const categories = buildTestCases<CUCategory>({
        '@SmokeTest Valid Category': withTimestamp(categoryData.create_valid_category),
        'Duplicate Category': categoryData.create_duplicate_category,
        'Empty Category Name': categoryData.create_empty_category_name,
        '@Boundary Over length Category Name': categoryData.create_over_length_category_name
    });

    for (const { label, data } of categories) {
        test(`${label} - Create a Category`, async () => {
            await test.step('Input category details and submit', async () => {
                await categoryPage.inputName(data.name);
                await categoryPage.inputDescription(data.description);
                await categoryPage.clickAddButton();
            });
            await test.step('Verify system behavior', async () => {
                await categoryPage.expectMessage(data.expectedMessage);
            });
        });
    }
});
