import { Page, Locator } from "@playwright/test";
import { BasePage } from "../base-page";
import { PAGE_ENDPOINT_URL } from "../../core/constants/page-url";

export class ResetPasswordPage extends BasePage {
    //forgot password modal
    private LNK_FORGOTPASSWORD_URL: string;
    private TXT_EMAIL_LOCATOR: Locator;
    private BTN_SENDNEWPASSWORD_LOCATOR: Locator;

    //reset passsword modal
    private TXT_CURRENTPASSWORD_LOCATOR: Locator;
    private TXT_NEWPASSWORD_LOCATOR: Locator;
    private BTN_RESETPASSWORD_LOCATOR: Locator;

    //expected output
    private LBL_SUCCESSREDIRECT_LOCATOR: Locator;
    private LBL_ERROREMAILINVALID_LOCATOR: Locator;

    constructor(page: Page) {
        super(page);
        this.LNK_FORGOTPASSWORD_URL = PAGE_ENDPOINT_URL.FORGOT_PASSWORD;
        this.TXT_EMAIL_LOCATOR = page.getByPlaceholder('you@example.com');
        this.BTN_SENDNEWPASSWORD_LOCATOR = page.getByRole("button", { name: "Send New Password" });
        this.LBL_SUCCESSREDIRECT_LOCATOR = page.getByText("Reset your password");
        this.TXT_CURRENTPASSWORD_LOCATOR = page.getByPlaceholder("Enter your current password");
        this.TXT_NEWPASSWORD_LOCATOR = page.getByPlaceholder("Enter your new password");
        this.BTN_RESETPASSWORD_LOCATOR = page.getByRole("button", { name: "Reset Password" });
        this.LBL_ERROREMAILINVALID_LOCATOR = page.getByText("Account does not exist. Please check your email.");
    }

    async navigateToHomePage(url = "") {
        await this.page.goto(url);
    }

    async goToForgotPasswordModal() {
        await this.page.goto(this.LNK_FORGOTPASSWORD_URL);
    }

    async inputEmail(email: string) {
        await this.fill(this.TXT_EMAIL_LOCATOR, email);
    }

    async inputCurrentPassword(password: string) {
        await this.fill(this.TXT_CURRENTPASSWORD_LOCATOR, password);
    }

    async inputNewPassword(password: string) {
        await this.fill(this.TXT_NEWPASSWORD_LOCATOR, password);
    }

    async clickSendNewPasswordButton() {
        await this.click(this.BTN_SENDNEWPASSWORD_LOCATOR);
    }

    async expectSendSuccess(): Promise<boolean> {
        return await this.isLocatorVisible(this.LBL_SUCCESSREDIRECT_LOCATOR);
    }

    async expectSendFail(): Promise<boolean> {
        return await this.isLocatorVisible(this.LBL_ERROREMAILINVALID_LOCATOR)
    }

    async clickResetPasswordButton() {
        await this.click(this.BTN_RESETPASSWORD_LOCATOR);
    }

    async expectMessage(message: string) {
        const locator: Locator = this.page.getByText(message);
        await this.isVisible(locator);
    }
}
