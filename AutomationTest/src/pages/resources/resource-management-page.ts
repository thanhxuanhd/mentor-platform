import { Page, Locator, expect } from "@playwright/test";
import { BasePage } from "../base-page";
import { PAGE_ENDPOINT_URL } from "../../core/constants/page-url";

export class ResourcePage extends BasePage {
    private LNK_COURSE_URL: string;
    private LNK_RESOURCE_URL: string;
    private BTN_RESOURCE_LOCATOR: Locator;
    private DRD_SELECTCOURSE_LOCATOR: Locator;
    private TXT_TITLE_LOCATOR: Locator;
    private TXT_DESCRIPTION_LOCATOR: Locator;
    private BTN_UPLOADFILE_LOCATOR: Locator;

    //add resource locator
    private BTN_ADDRESOURCE_LOCATOR: Locator;
    private BTN_CREATE_LOCATOR: Locator;

    // //update resource locatorz
    private BTN_EDITICON_LOCATOR: Locator;
    private BTN_UPDATEBUTTON_LOCATOR: Locator;
    private BTN_DELETEFILE_LOCATOR: Locator;

    //delete resource locator
    private BTN_DELETEICON: Locator;
    private BTN_CONFIRMDELETE_LOCATOR: Locator;
    private LBL_SUCCESSDELETE_LOCATOR: Locator;

    //view resource locator
    private DRD_CATEGORY_LOCATOR: Locator;
    private LST_RESOURCELIST_LOCATOR: Locator;

    constructor(page: Page) {
        super(page);
        this.LNK_COURSE_URL = PAGE_ENDPOINT_URL.COURSE_MANAGEMENT;
        this.LNK_RESOURCE_URL = PAGE_ENDPOINT_URL.RESOURCE;
        this.BTN_RESOURCE_LOCATOR = page.locator('button:has(svg[data-icon="folder"])').first();
        this.DRD_SELECTCOURSE_LOCATOR = page.locator("#course_resource_form_courseId")
        this.BTN_ADDRESOURCE_LOCATOR = page.locator('//button[span[text()="Add Material"]]');
        this.BTN_CREATE_LOCATOR = page.locator('//button[span[text()="Add"]]');
        this.TXT_TITLE_LOCATOR = page.locator("#course_resource_form_simple_title");
        this.TXT_DESCRIPTION_LOCATOR = page.locator("#course_resource_form_simple_description");
        this.BTN_UPLOADFILE_LOCATOR = page.locator('#course_resource_form_simple_resource');
        this.BTN_EDITICON_LOCATOR = page.locator('.ant-modal button span[aria-label="edit"]');
        this.BTN_UPDATEBUTTON_LOCATOR = page.locator('//button[span[text()="Update"]]');
        this.BTN_DELETEICON = page.locator('.ant-modal button span[aria-label="delete"]');
        this.BTN_CONFIRMDELETE_LOCATOR = page.locator('//button[span[text()="Yes"]]');
        this.LBL_SUCCESSDELETE_LOCATOR = page.getByText("The resource has been successfully deleted.");
        this.LST_RESOURCELIST_LOCATOR = page.locator('div.ant-card.resource-card');
        this.DRD_CATEGORY_LOCATOR = page.locator("#rc_select_0").first();
        this.BTN_DELETEFILE_LOCATOR = page.getByTitle('Remove file').first();
    }

    async navigateToHomePage(url = "") {
        await this.page.goto(url);
    }

    async goToCoursePage() {
        await this.page.goto(this.LNK_COURSE_URL);
    }

    async goToResoursePage() {
        await this.page.goto(this.LNK_RESOURCE_URL);
    }

    async selectResourceModal() {
        await this.click(this.BTN_RESOURCE_LOCATOR);
    }

    async inputTitle(title: string) {
        await this.fill(this.TXT_TITLE_LOCATOR, title);
    }

    async inputDescription(description: string) {
        await this.fill(this.TXT_DESCRIPTION_LOCATOR, description);
    }

    async clickAddResourceButton() {
        await this.click(this.BTN_ADDRESOURCE_LOCATOR);
    }

    async clickCreateButton() {
        await this.click(this.BTN_CREATE_LOCATOR);
    }

    async uploadResource(filePath: string[]) {
        await this.uploadFile(this.BTN_UPLOADFILE_LOCATOR, filePath);
    }

    async clickEditIcon() {
        await this.click(this.BTN_EDITICON_LOCATOR);
    }

    async clickUpdateButton() {
        await this.click(this.BTN_UPDATEBUTTON_LOCATOR);
    }

    async clickDeleteFile() {
        await this.click(this.BTN_DELETEFILE_LOCATOR);
    }
    async clickDeleteButton() {
        await this.click(this.BTN_DELETEICON);
    }

    async clickConfirmDeleteButton() {
        await this.click(this.BTN_CONFIRMDELETE_LOCATOR);
    }

    async expectSucessDeleteMessage() {
        await this.isVisible(this.LBL_SUCCESSDELETE_LOCATOR);
    }

    async selectCategory(categoryName: string) {
        await this.fill(this.DRD_CATEGORY_LOCATOR, categoryName);
        await this.DRD_CATEGORY_LOCATOR.press("Enter");
    }

    async expectListResourceIsNotNull() {
        await expect(this.LST_RESOURCELIST_LOCATOR).not.toBeNull();
        const resourceCount = this.LST_RESOURCELIST_LOCATOR.count();
        await expect(resourceCount).toBeGreaterThan(0);
    }
}