import test from '@playwright/test';
import { LoginPage } from '../../pages/authentication/login-page'
import { LoginUser } from '../../models/user/user';

export async function loginStep(page: any, user: LoginUser): Promise<LoginPage> {
    const loginPage = new LoginPage(page);

    await test.step('Navigate to Login Page', async () => {
        await loginPage.navigateToHomePage();
    });

    await test.step('Input default account', async () => {
        await loginPage.inputEmail(user.email);
        await loginPage.inputPassword(user.password);

    });

    await test.step('Click Signin button', async () => {
        await loginPage.clickSignInButton();
    });

    return loginPage;
}