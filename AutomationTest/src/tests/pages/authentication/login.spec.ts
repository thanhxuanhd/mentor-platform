import { LoginPage } from '../../../pages/authentication/login-page';
import { test } from '../../../core/fixture/auth-fixture';
import { createTestLoginUser } from '../../../core/utils/api-helper';

test.describe('@Login Login tests', () => {
    let loginPage: LoginPage;
    let testUser: any;

    test.beforeEach(async ({ page, request }) => {
        loginPage = new LoginPage(page);
        testUser = await createTestLoginUser(request);
        await test.step('Navigate to Login Page', async () => {
            await loginPage.navigateToHomePage();
            await loginPage.goToLoginPage();
        });
    });

    test('@SmokeTest @Login Login success with valid User', async ({ }) => {
        await test.step('Input valid password', async () => {
            await loginPage.inputEmail(testUser.email);
            await loginPage.inputPassword(testUser.password);
        });
        await test.step('Click Signin button', async () => {
            await loginPage.clickSignInButton();
        });
        await test.step('Verify system behavior', async () => {
            await loginPage.expectSuccessLogin();
        });
    });

    test('@Login Login failed with invalid User', async ({ }) => {
        await test.step('Input invalid password', async () => {
            await loginPage.inputEmail(testUser.email);
            await loginPage.inputPassword("InvalidPassword@123");
        });
        await test.step('Click Signin button', async () => {
            await loginPage.clickSignInButton();
        });
        await test.step('Verify system behavior', async () => {
            await loginPage.expectFailedLogin();
        });
    });

});
