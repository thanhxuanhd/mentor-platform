import { CategoryPage } from '../../../pages/categories/categories-page';
import { test } from '../../../core/fixture/authFixture';

test('@SmokeTest @Category Delete a valid category sucessful', async ({ loggedInPage, page }) => {
    // await new Promise(resolve => setTimeout(resolve, 5000));
    const categoryPage = new CategoryPage(page);

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