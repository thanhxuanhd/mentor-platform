import categoryData from '../../test-data/category-data.json';
import { test } from '../../../core/fixture/authFixture';
import { CategoryPage } from '../../../pages/categories/categories-page';
import { withTimestamp } from '../../../core/utils/generate-unique-data';
import { CUCategory } from '../../../models/categories/create-category';

test.describe('@Category Update category tests', () => {
    let categoryPage: CategoryPage;

    test.beforeEach(async ({ loggedInPage, page }) => {
        categoryPage = new CategoryPage(page);
        await categoryPage.goToCategoryPage();
        await categoryPage.clickUpdateCategoryButton();
    });

    const categories: { [label: string]: CUCategory } = {
        '@SmokeTest Valid Category': withTimestamp(categoryData.update_valid_category),
        'Duplicate Category': categoryData.update_duplicate_category,
        'Empty Category Name': categoryData.update_empty_category_name,
        '@Boundary Over length Category Name': categoryData.update_over_length_category_name
    };

    for (const [label, data] of Object.entries(categories)) {
        test(`${label} - Update a Category`, async () => {
            await test.step('Input category details and submit', async () => {
                await categoryPage.inputName(data.name);
                await categoryPage.inputDescription(data.description);
                await categoryPage.clickUpdateButton();
            });
            await test.step('Verify system behavior', async () => {
                await categoryPage.expectMessage(data.expectedMessage);
            });
        });
    }
});
