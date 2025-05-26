import { expect, Locator, Page } from "@playwright/test";
import { BasePage } from "../base-page";
import { PAGE_ENDPOINT_URL } from "../../core/constants/page-url";

export class CategoryBrowsingPage extends BasePage {
  private readonly TXT_SEARCH_CATEGORY: Locator;
  private readonly BTN_SEARCH_BUTTON: Locator;
  private readonly LBL_CATEGORY_NAME: Locator;
  private readonly LBL_EMPTY_MESSAGE: Locator;

  constructor(page: Page) {
    super(page);
    this.TXT_SEARCH_CATEGORY = page.locator(
      "input[placeholder='Search by category name...']"
    );
    this.BTN_SEARCH_BUTTON = page.locator(
      ".ant-input-wrapper .ant-btn-primary"
    );
    this.LBL_CATEGORY_NAME = page.locator("tbody.ant-table-tbody .font-medium");
    this.LBL_EMPTY_MESSAGE = page.locator(".ant-empty-description");
  }

  async navigateToCategoryPage(): Promise<void> {
    await this.page.goto(PAGE_ENDPOINT_URL.CATEGORY_BROWSING);
  }

  async searchCategory(categoryName: string): Promise<void> {
    await this.TXT_SEARCH_CATEGORY.fill(categoryName);
    await this.BTN_SEARCH_BUTTON.click();
  }

  async verifyCategoryNameResult(searchTerm: string) {
    if (searchTerm) {
      const categories = await this.LBL_CATEGORY_NAME.allTextContents();
      if (categories.length === 0) {
        await expect(this.LBL_EMPTY_MESSAGE).toBeVisible();
        await expect(this.LBL_EMPTY_MESSAGE).toHaveText("No data");
      } else {
        for (let category of categories) {
          category = category.trim().toLowerCase();
          console.log(
            `Category: ${category}, Search Term: ${searchTerm
              .trim()
              .toLowerCase()}`
          );
          expect(
            category.includes(searchTerm.trim().toLowerCase())
          ).toBeTruthy();
        }
      }
    }
  }
}
