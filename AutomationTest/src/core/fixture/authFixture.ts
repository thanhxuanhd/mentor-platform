import { test as baseTest } from '@playwright/test';
import { afterEach } from 'node:test';
import { LoginUser } from '../../models/user/user';
import { loginStep } from '../utils/login-helper';

type AuthFixtures = {
    loggedInPage: any;
};

const defaultUser: LoginUser = {
    email: "string@example.com",
    password: "stringss",
};

export const test = baseTest.extend<AuthFixtures>({
    loggedInPage: async ({ page }, use) => {
        await loginStep(page, defaultUser);
        await use(page);
    }
});
