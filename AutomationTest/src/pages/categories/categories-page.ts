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
    private LBL_CREATESUCCESSMESSAGE_LOCATOR: Locator;
    private LBL_DUPLICATEFAILEDMESSAGE_LOCATOR: Locator;
    private LBL_UPDATESUCCESSMESSAGE_LOCATOR: Locator;
    private LBL_EMPTYCATEGORYNAME_LOCATOR: Locator;
    private LBL_OVERLENGTHNAME_LOCATOR: Locator;
    private LBL_SUCCESSDELETE_LOCATOR: Locator;

    constructor(page: Page) {
        super(page);
        this.LNK_CATEGORY_URL = PAGE_ENDPOINT_URL.CATEGORIES;
        this.BTN_ADDCATEGORY_LOCATOR = page.getByText("Add Category")
        this.TXT_NAME_LOCATOR = page.getByPlaceholder('Enter new category name');
        this.TXT_DESCRIPTION_LOCATOR = page.getByPlaceholder('Enter your description');
        this.BTN_ADD_LOCATOR = page.locator('//button[span[text()="Add"]]');
        this.LBL_CREATESUCCESSMESSAGE_LOCATOR = page.getByText('Category created successfully');
        this.LBL_DUPLICATEFAILEDMESSAGE_LOCATOR = page.getByText("Already have this category");
        this.BTN_UPDATECATEGORY_LOCATOR = page.locator('[aria-label="edit"]').first().locator('xpath=ancestor::button');
        this.LBL_UPDATESUCCESSMESSAGE_LOCATOR = page.getByText("Category updated successfully");
        this.BTN_UPDATEBUTTON_LOCATOR = page.locator('//button[span[text()="Update"]]');
        this.LBL_EMPTYCATEGORYNAME_LOCATOR = page.getByText("Please enter category name");
        this.LBL_OVERLENGTHNAME_LOCATOR = page.getByText("Category name should not exceed 50 characters");
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
    async expectSuccessCreated() {
        await this.isVisible(this.LBL_CREATESUCCESSMESSAGE_LOCATOR);
    }

    async expectDuplicateFailed() {
        await this.isVisible(this.LBL_DUPLICATEFAILEDMESSAGE_LOCATOR);
    }

    async clickUpdateCategoryButton() {
        await this.click(this.BTN_UPDATECATEGORY_LOCATOR);
    }

    async clickUpdateButton() {
        await this.click(this.BTN_UPDATEBUTTON_LOCATOR);
    }

    async expectSuccessUpdated() {
        await this.isVisible(this.LBL_UPDATESUCCESSMESSAGE_LOCATOR);
    }

    async expectEmptyCategoryNameMessage() {
        await this.isVisible(this.LBL_EMPTYCATEGORYNAME_LOCATOR);
    }

    async expectOverLengthMessage() {
        await this.isVisible(this.LBL_OVERLENGTHNAME_LOCATOR);
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

    async clickStatusButton(isActive: boolean) {
        const statusBtn = isActive ? this.BTN_ACTIVE_LOCATOR : this.BTN_INACTIVE_LOCATOR;
        await this.click(statusBtn);
    }
}