import { Page, Locator } from "@playwright/test";
import { BasePage } from "../base-page";

export class LoginPage extends BasePage {
    private LNK_LOGINPAGE_URL: string;
    private TXT_EMAIL_LOCATOR: Locator;
    private TXT_PASSWORD_LOCATOR: Locator;
    private BTN_SIGNIN_LOCATOR: Locator;

    constructor(page: Page) {
        super(page);
        this.LNK_LOGINPAGE_URL = "/login";
        this.TXT_EMAIL_LOCATOR = page.getByPlaceholder('Enter your email');
        this.TXT_PASSWORD_LOCATOR = page.getByPlaceholder('Enter your password');
        this.BTN_SIGNIN_LOCATOR = page.getByRole("button", { name: "Sign In" });
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
}