import { CUCategory } from '../../../models/categories/create-category';
import { expect } from '@playwright/test';
import categoryData from '../../test-data/category-data.json';
import { test } from '../../../core/fixture/authFixture';
import { CategoryPage } from '../../../pages/categories/categories-page';

test.describe('@Category Update category tests', () => {
    let categoryPage: CategoryPage;

    test.beforeEach(async ({ page }) => {
        categoryPage = new CategoryPage(page);
        await categoryPage.goToCategoryPage();
        await categoryPage.clickUpdateCategoryButton();
    });

    const categories = [
        {
            label: '@SmokeTest Valid Category',
            data: categoryData.update_valid_category,
            expectFn: async () => await categoryPage.expectSuccessUpdated()
        },
        {
            label: 'Duplicate Category',
            data: categoryData.update_duplicate_category,
            expectFn: async () => await categoryPage.expectDuplicateFailed()
        },
        {
            label: 'Empty Category Name',
            data: categoryData.update_empty_category_name,
            expectFn: async () => await categoryPage.expectEmptyCategoryNameMessage()
        },
        {
            label: '@Boundary Over length Category Name',
            data: categoryData.update_over_length_category_name,
            expectFn: async () => await categoryPage.expectOverLengthMessage()
        }
    ];

    for (const { label, data, expectFn } of categories) {
        test(`${label} - Update a Category`, async () => {
            await test.step('Input category details and submit', async () => {
                await categoryPage.inputName(data.name);
                await categoryPage.inputDescription(data.description);
                await categoryPage.clickUpdateButton();
            });
            await test.step('Verify system behavior', expectFn);
        });
    }
});
