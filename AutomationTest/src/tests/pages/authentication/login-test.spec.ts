import { expect } from '@playwright/test';
import { LoginPage } from '../../../pages/authentication/login-page';
import loginData from './login-data.json'
import { LoginUser } from '../../../models/user/user';
import { test } from '../../../core/fixture/authFixture';


test('@SmokeTest @Login Login with valid User', async ({ loggedInPage, page }) => {

    const loginPage = new LoginPage(page);
    const user: LoginUser = loginData;

    await new Promise(resolve => setTimeout(resolve, 5000));

});
