import { expect } from "@playwright/test";
import { test } from "../../../core/fixture/auth-fixture";
import { withTimestamp } from "../../../core/utils/generate-unique-data";
import { CUCategory } from "../../../models/categories/create-category";
import { CategoryPage } from "../../../pages/categories/categories-page";
import { CategoryBrowsingPage } from "../../../pages/categories/category-browsing-page";
import categoryData from "../../test-data/category-data.json";
import { LoginPage } from "../../../pages/authentication/login-page";
import {
  deleteTestCategory,
  getLatestCategory,
} from "../../../core/utils/api-helper";

test.describe.serial("@Category Category browsing tests", () => {
  let categoryBrowsingPage: CategoryBrowsingPage;
  let categoryPage: CategoryPage;
  let loginPage: LoginPage;
  const data = withTimestamp(categoryData.create_valid_category);

  test.beforeEach(async ({ loggedInPageByAdminRole, page }) => {
    categoryBrowsingPage = new CategoryBrowsingPage(page);
    categoryPage = new CategoryPage(page);
    loginPage = new LoginPage(page);
    await test.step(`Create a Category`, async () => {
      await categoryBrowsingPage.navigateToCategoryPage();
      await categoryPage.goToCategoryPage();
      await categoryPage.clickAddCategoryButton();
      await categoryPage.inputName(data.name);
      await categoryPage.inputDescription(data.description);
      await categoryPage.clickAddButton();
    });
  });

  test(`Verifying that category list are updated after editing category`, async () => {
    const categoryUniqueName: CUCategory = withTimestamp(
      categoryData.update_valid_category
    );
    await test.step("Input category details and submit", async () => {
      await categoryPage.clickUpdateCategoryButton();
      await categoryPage.inputName(categoryUniqueName.name);
      await categoryPage.inputDescription(
        categoryData.update_valid_category.description
      );
    });
    await test.step("Update selected category", async () => {
      await categoryPage.clickUpdateButton();
    });
    await test.step("Signup to Learner account", async () => {
      await loginPage.clickOnLogoutButton();
      await loginPage.inputEmail(process.env.LEARNER_USER_NAME!);
      await loginPage.inputPassword(process.env.LEARNER_PASSWORD!);
    });

    await test.step("Click Signin button", async () => {
      await loginPage.clickSignInButton();
      await loginPage.expectLogoutButton();
    });
    await test.step("Verify category is updated", async () => {
      await categoryPage.goToCategoryPage();
      await categoryPage.expectMessage(categoryUniqueName.name);
    });
  });

  test(`Verifying that category list updated after deleting a category`, async () => {
    await test.step("Verify category is deleted", async () => {
      const beforeDeleteCategory =
        await categoryBrowsingPage.getAllCategoryValue();
      await categoryPage.clickDeleteCategoryButton();
      await categoryPage.clickConfirmDeleteButton();
      await categoryPage.expectSucessDeleteMessage();
      await test.step("Signup to Learner account", async () => {
        await loginPage.clickOnLogoutButton();
        await loginPage.inputEmail(process.env.LEARNER_USER_NAME!);
        await loginPage.inputPassword(process.env.LEARNER_PASSWORD!);
      });

      await test.step("Click Signin button", async () => {
        await loginPage.clickSignInButton();
        await loginPage.expectLogoutButton();
      });
      await categoryPage.goToCategoryPage();
      const afterDeleteCategory =
        await categoryBrowsingPage.getAllCategoryValue();
      expect(afterDeleteCategory.includes(beforeDeleteCategory[0])).toBeFalsy();
    });
  });
});
