import { expect } from "@playwright/test";
import { test } from "../../../core/fixture/auth-fixture";
import { withTimestamp } from "../../../core/utils/generate-unique-data";
import { CategoryBrowsingSearch } from "../../../models/categories/category-browsing";
import { CUCategory } from "../../../models/categories/create-category";
import { CategoryPage } from "../../../pages/categories/categories-page";
import { CategoryBrowsingPage } from "../../../pages/categories/category-browsing-page";
import categorySearchTermData from "../../test-data/category-browsing-data.json";
import categoryData from "../../test-data/category-data.json";
import { LoginPage } from "../../../pages/authentication/login-page";

test.describe("@Category Category browsing tests", () => {
  let categoryBrowsingPage: CategoryBrowsingPage;
  let categoryPage: CategoryPage;
  let loginPage: LoginPage;

  test.beforeEach(async ({ loggedInPageByAdminRole, page }) => {
    categoryBrowsingPage = new CategoryBrowsingPage(page);
    categoryPage = new CategoryPage(page);
    loginPage = new LoginPage(page);
    await categoryBrowsingPage.navigateToCategoryPage();
  });

  const categories: { [label: string]: CategoryBrowsingSearch } = {
    "@SmokeTest Search empty category keyword":
      categorySearchTermData.search_empty_category_keyword,
    "@SmokeTest Search nonexistence category keyword":
      categorySearchTermData.search_category_keyword_no_results,
    "@SmokeTest Search multiple category keywords":
      categorySearchTermData.search_category_multiple_keyword,
    "@SmokeTest Search one category keyword":
      categorySearchTermData.search_category_one_keyword,
    "Search wildcard category keyword":
      categorySearchTermData.search_category_wilcard_characters,
    "Search category keyword with extra spaces":
      categorySearchTermData.search_category_extra_spaces_characters,
  };

  for (const [label, data] of Object.entries(categories)) {
    test(`${label} - Verify Search category`, async () => {
      await test.step("Search category", async () => {
        await categoryBrowsingPage.searchCategory(data.search_term);
      });
      await test.step("Verify system behavior", async () => {
        await categoryBrowsingPage.verifyCategoryNameResult(data.search_term);
      });
    });
  }

  test(`@SmokeTest Verifying that category list are updated after editing category`, async () => {
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
      await loginPage.inputEmail(
        categorySearchTermData.learner_role_account.email
      );
      await loginPage.inputPassword(
        categorySearchTermData.learner_role_account.password
      );
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

  test(`@SmokeTest Verifying that category list updated after deleting a category`, async () => {
    await test.step("Verify category is deleted", async () => {
      const beforeDeleteCategory =
        await categoryBrowsingPage.getAllCategoryValue();
      await categoryPage.clickDeleteCategoryButton();
      await categoryPage.clickConfirmDeleteButton();
      await categoryPage.expectSucessDeleteMessage();
      await test.step("Signup to Learner account", async () => {
        await loginPage.clickOnLogoutButton();
        await loginPage.inputEmail(
          categorySearchTermData.learner_role_account.email
        );
        await loginPage.inputPassword(
          categorySearchTermData.learner_role_account.password
        );
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
