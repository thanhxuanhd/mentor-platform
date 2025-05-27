import { Page, Locator } from "@playwright/test";
import { BasePage } from "../base-page";
import { PAGE_ENDPOINT_URL } from "../../core/constants/page-url";

export class SignUpPage extends BasePage {
    private LNK_REGISTRATION1_URL: string;
    private TXT_EMAIL_LOCATOR: Locator;
    private TXT_PASSWORD_LOCATOR: Locator;
    private TXT_CONFIRMPASSWORD_LOCATOR: Locator;
    private BTN_CONTINUETOSTEP2_LOCATOR: Locator;
    private CBX_TERM_LOCATOR: Locator;

    constructor(page: Page) {
        super(page);
        this.LNK_REGISTRATION1_URL = PAGE_ENDPOINT_URL.REGISTRATION_STEP_1;
        this.TXT_EMAIL_LOCATOR = page.getByPlaceholder('Enter your email');
        this.BTN_CONTINUETOSTEP2_LOCATOR = page.getByRole("button", { name: "Continue to Profile Setup" });
        this.TXT_PASSWORD_LOCATOR = page.getByPlaceholder("Enter your password");
        this.TXT_CONFIRMPASSWORD_LOCATOR = page.getByPlaceholder("Enter your confirm password");
        this.CBX_TERM_LOCATOR = page.locator("#terms");
    }

    async navigateToHomePage(url = "") {
        await this.page.goto(url);
    }

    async goToRegistrationStep1Modal() {
        await this.page.goto(this.LNK_REGISTRATION1_URL);
    }

    async inputEmail(email: string) {
        await this.fill(this.TXT_EMAIL_LOCATOR, email);
    }

    async inputPassword(password: string) {
        await this.fill(this.TXT_PASSWORD_LOCATOR, password);
    }

    async inputConfirmPassword(password: string) {
        await this.fill(this.TXT_CONFIRMPASSWORD_LOCATOR, password);
    }

    async clickContinueToProfileSetupButton() {
        await this.click(this.BTN_CONTINUETOSTEP2_LOCATOR);
    }

    async checkOnTermCheckBox(isCheck: boolean) {
        if (isCheck) {
            await this.CBX_TERM_LOCATOR.check();
        }
    }
}
