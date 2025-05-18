import { expect } from '@playwright/test';
import { LoginPage } from '../../../pages/authentication/login-page';
import loginData from '../../test-data/login-data.json'
import { LoginUser } from '../../../models/user/user';
import { test } from '../../../core/fixture/authFixture';
import { loginStep } from '../../../core/utils/login-helper';

test('@SmokeTest @Login Login success with valid User', async ({ page }) => {
    const loginPage = new LoginPage(page);
    const user: LoginUser = loginData.valid_user;
    await loginStep(page, user);
    await test.step('Verify system behavior', async () => {
        await loginPage.expectSuccessLogin();
    });
});

test('@SmokeTest @Login Login failed with invalid User', async ({ page }) => {
    const loginPage = new LoginPage(page);
    const user: LoginUser = loginData.invalid_user;
    await loginStep(page, user);
    await test.step('Verify system behavior', async () => {
        await loginPage.expectFailedLogin();
    });
});

