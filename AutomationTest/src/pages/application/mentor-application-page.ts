import { Page, Locator, expect } from "@playwright/test";
import { BasePage } from "../base-page";
import { PAGE_ENDPOINT_URL } from "../../core/constants/page-url";

export class MentorApplicationPage extends BasePage {
    private LNK_MENTORAPPLICATION_URL: string;
    private TXT_EDUCATION_LOCATOR: Locator;
    private TXT_WORKEXPERIENCE_LOCATOR: Locator;
    private TXT_CERTIFICATION_LOCATOR: Locator;
    private TXT_MOTIVATION_LOCATOR: Locator;
    private BTN_UPLOAD_LOCATOR: Locator;
    private BTN_SUBMIT_LOCATOR: Locator;
    private LBL_SUCCESSMSG_LOCATOR: Locator;

    constructor(page: Page) {
        super(page);
        this.LNK_MENTORAPPLICATION_URL = PAGE_ENDPOINT_URL.MENTOR_APPLICATION;
        this.TXT_EDUCATION_LOCATOR = page.locator('#mentor_application_form_education');
        this.TXT_WORKEXPERIENCE_LOCATOR = page.locator('#mentor_application_form_workExperience');
        this.TXT_CERTIFICATION_LOCATOR = page.locator("#mentor_application_form_certifications");
        this.TXT_MOTIVATION_LOCATOR = page.locator("#mentor_application_form_statement");
        this.BTN_SUBMIT_LOCATOR = page.locator('//button[span[text()="Submit"]]');
        this.BTN_UPLOAD_LOCATOR = page.locator("#mentor_application_form_documents");
        this.LBL_SUCCESSMSG_LOCATOR = page.locator('.ant-notification-notice');
    }

    async navigateToHomePage(url = "") {
        await this.page.goto(url);
    }

    async goToMentorApplicationPage() {
        await this.page.goto(this.LNK_MENTORAPPLICATION_URL);
    }

    async clickSubmitButton() {
        await this.click(this.BTN_SUBMIT_LOCATOR);
    }

    async inputEducation(education: string) {
        await this.fill(this.TXT_EDUCATION_LOCATOR, education);
    }

    async inputWorkExperience(we: string) {
        await this.fill(this.TXT_WORKEXPERIENCE_LOCATOR, we);
    }

    async inputCertification(certification: string) {
        await this.fill(this.TXT_CERTIFICATION_LOCATOR, certification);
    }

    async inputMotivation(motivation: string) {
        await this.fill(this.TXT_MOTIVATION_LOCATOR, motivation);
    }

    async uploadFiles(filePaths: string[]) {
        await this.uploadFile(this.BTN_UPLOAD_LOCATOR, filePaths);
    }

    async expecetSuccessMessage() {
        await expect(this.LBL_SUCCESSMSG_LOCATOR.getByText('Thank you! We will review your application soon.')).toBeVisible();
    }
}