import { Page, Locator } from "@playwright/test";
import { BasePage } from "../base-page";
import { PAGE_ENDPOINT_URL } from "../../core/constants/page-url";

export class LoginPage extends BasePage {
    private LNK_LOGINPAGE_URL: string;
    private TXT_EMAIL_LOCATOR: Locator;
    private TXT_PASSWORD_LOCATOR: Locator;
    private BTN_SIGNIN_LOCATOR: Locator;
    private LBL_SUCCESSMESSAGE_LOCATOR: Locator;
    private LBL_FAILEDMESSAGE_LOCATOR: Locator;
    private BTN_LOGOUT_LOCATOR: Locator;

    constructor(page: Page) {
        super(page);
        this.LNK_LOGINPAGE_URL = PAGE_ENDPOINT_URL.LOGIN;
        this.TXT_EMAIL_LOCATOR = page.getByPlaceholder('Enter your email');
        this.TXT_PASSWORD_LOCATOR = page.getByPlaceholder('Enter your password');
        this.BTN_SIGNIN_LOCATOR = page.getByRole("button", { name: "Sign In" });
        this.LBL_SUCCESSMESSAGE_LOCATOR = page.getByText("Sign in successfully!");
        this.LBL_FAILEDMESSAGE_LOCATOR = page.getByText("Email or password is not correct");
        this.BTN_LOGOUT_LOCATOR = page.getByText("Logout");
    }

    async navigateToHomePage(url = "") {
        await this.page.goto(url);
    }

    async goToLoginPage() {
        await this.page.goto(this.LNK_LOGINPAGE_URL);
    }

    async inputEmail(email: string) {
        await this.fill(this.TXT_EMAIL_LOCATOR, email);
    }

    async inputPassword(password: string) {
        await this.fill(this.TXT_PASSWORD_LOCATOR, password);
    }

    async clickSignInButton() {
        await this.click(this.BTN_SIGNIN_LOCATOR);
    }

    async expectSuccessLogin() {
        await this.isVisible(this.LBL_SUCCESSMESSAGE_LOCATOR);
    }

    async expectLogoutButton() {
        await this.isVisible(this.BTN_LOGOUT_LOCATOR);
    }

    async expectFailedLogin() {
        await this.isVisible(this.LBL_FAILEDMESSAGE_LOCATOR);
    }
}