import { Page, Locator } from "@playwright/test";
import { BasePage } from "../base-page";
import { PAGE_ENDPOINT_URL } from "../../core/constants/page-url";

export class CategoryPage extends BasePage {
    private LNK_CATEGORY_URL: string;
    private TXT_NAME_LOCATOR: Locator;
    private TXT_DESCRIPTION_LOCATOR: Locator;

    //add category locator
    private BTN_ADD_LOCATOR: Locator;
    private BTN_ADDCATEGORY_LOCATOR: Locator;
    private BTN_ACTIVE_LOCATOR: Locator;
    private BTN_INACTIVE_LOCATOR: Locator;

    //update category locator
    private BTN_UPDATECATEGORY_LOCATOR: Locator;
    private BTN_UPDATEBUTTON_LOCATOR: Locator;

    //delete category locator
    private BTN_DELETECATEGORY_LOCATOR: Locator;
    private BTN_CONFIRMDELETE_LOCATOR: Locator;

    //expected output
    private LBL_SUCCESSDELETE_LOCATOR: Locator;

    constructor(page: Page) {
        super(page);
        this.LNK_CATEGORY_URL = PAGE_ENDPOINT_URL.CATEGORIES;
        this.BTN_ADDCATEGORY_LOCATOR = page.getByText("Add Category")
        this.TXT_NAME_LOCATOR = page.getByPlaceholder('Enter new category name');
        this.TXT_DESCRIPTION_LOCATOR = page.getByPlaceholder('Enter your description');
        this.BTN_ADD_LOCATOR = page.locator('//button[span[text()="Add"]]');
        this.BTN_UPDATECATEGORY_LOCATOR = page.locator('[aria-label="edit"]').first().locator('xpath=ancestor::button');
        this.BTN_UPDATEBUTTON_LOCATOR = page.locator('//button[span[text()="Update"]]');
        this.BTN_DELETECATEGORY_LOCATOR = page.locator('[aria-label="delete"]').first().locator('xpath=ancestor::button');
        this.BTN_CONFIRMDELETE_LOCATOR = page.locator('//button[span[text()="Yes"]]');
        this.LBL_SUCCESSDELETE_LOCATOR = page.getByText("Category deleted successfully");
        this.BTN_ACTIVE_LOCATOR = page.locator('label:has-text("Active") input[type="radio"]');
        this.BTN_INACTIVE_LOCATOR = page.locator('label:has-text("Inactive") input[type="radio"]');
    }

    async navigateToHomePage(url = "") {
        await this.page.goto(url);
    }

    async goToCategoryPage() {
        await this.page.goto(this.LNK_CATEGORY_URL);
    }

    async clickAddCategoryButton() {
        await this.click(this.BTN_ADDCATEGORY_LOCATOR);
    }

    async inputName(name: string) {
        await this.fill(this.TXT_NAME_LOCATOR, name);
    }

    async inputDescription(description: string) {
        await this.fill(this.TXT_DESCRIPTION_LOCATOR, description);
    }

    async clickAddButton() {
        await this.click(this.BTN_ADD_LOCATOR);
    }

    async clickUpdateCategoryButton() {
        await this.click(this.BTN_UPDATECATEGORY_LOCATOR);
    }

    async clickUpdateButton() {
        await this.click(this.BTN_UPDATEBUTTON_LOCATOR);
    }

    async clickDeleteCategoryButton() {
        await this.click(this.BTN_DELETECATEGORY_LOCATOR);
    }

    async clickConfirmDeleteButton() {
        await this.click(this.BTN_CONFIRMDELETE_LOCATOR);
    }

    async expectSucessDeleteMessage() {
        await this.isVisible(this.LBL_SUCCESSDELETE_LOCATOR);
    }

    async expectMessage(message: string) {
        const locator: Locator = this.page.getByText(message)
        await this.isVisible(locator);
    }

    async clickStatusButton(isActive: boolean) {
        const statusBtn = isActive ? this.BTN_ACTIVE_LOCATOR : this.BTN_INACTIVE_LOCATOR;
        await statusBtn.check();
    }
}