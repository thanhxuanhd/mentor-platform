import test, { expect } from '@playwright/test';
import { LoginPage } from '../../../pages/authentication/login-page';
import loginData from './login-data.json'
import { LoginUser } from '../../../data-types/user';


test('@SmokeTest @Login Login with valid User', async ({ page }) => {

    const loginPage = new LoginPage(page);
    const user: LoginUser = loginData;

    await test.step('Navigate to homepage', async () => {
        await loginPage.navigateToHomePage();
    });

    await test.step(`Enter email: ${user.email}`, async () => {
        await loginPage.inputEmail(user.email);
    });

    await test.step('Enter password', async () => {
        await loginPage.inputPassword(user.password);
    });

    await test.step('Click sign in button', async () => {
        await loginPage.clickSignInButton();
    });


    await new Promise(resolve => setTimeout(resolve, 5000));

});
