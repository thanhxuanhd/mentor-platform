import { test } from "../../../core/fixture/authFixture";
import { CategoryBrowsingSearch } from "../../../models/categories/category-browsing";
import { CategoryBrowsingPage } from "../../../pages/categories/category-browsing-page";
import categorySearchTermData from "../../test-data/category-browsing-data.json";

test.describe("@Category Category browsing tests", () => {
  let categoryBrowsingPage: CategoryBrowsingPage;

  test.beforeEach(async ({ loggedInPage, page }) => {
    categoryBrowsingPage = new CategoryBrowsingPage(page);
    await categoryBrowsingPage.navigateToCategoryPage();
  });

  const categories: { [label: string]: CategoryBrowsingSearch } = {
    "Search empty category keyword":
      categorySearchTermData.search_empty_category_keyword,
    "Search nonexistence category keyword":
      categorySearchTermData.search_category_keyword_no_results,
    "Search multiple category keywords":
      categorySearchTermData.search_category_multiple_keyword,
    "Search one category keyword":
      categorySearchTermData.search_category_one_keyword,
    "Search wildcard category keyword":
      categorySearchTermData.search_category_wilcard_characters,
    "Search category keyword with extra spaces":
      categorySearchTermData.search_category_extra_spaces_characters,
  };

  for (const [label, data] of Object.entries(categories)) {
    test(`${label} - Create a Category`, async () => {
      await test.step("Search category", async () => {
        await categoryBrowsingPage.searchCategory(data.search_term);
      });
      await test.step("Verify system behavior", async () => {
        await categoryBrowsingPage.verifyCategoryNameResult(data.search_term);
      });
    });
  }
});
