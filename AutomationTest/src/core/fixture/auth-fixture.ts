import { test as baseTest } from '@playwright/test';
import { afterEach } from 'node:test';
import { LoginUser } from '../../models/user/user';
import { loginStep } from '../utils/login-helper';

export type AuthFixtures = {
    loggedInPageByAdminRole: any;
    loggedInPageByLearnerRole: any;
    loggedInPageByMentorRole: any;
};

export const defaultAdmin: LoginUser = {
    email: process.env.ADMIN_USER_NAME!,
    password: process.env.ADMIN_PASSWORD!,
};
export const defaultLearner: LoginUser = {
    email: process.env.LEARNER_USER_NAME!,
    password: process.env.LEARNER_PASSWORD!,
};
export const defaultMentor: LoginUser = {
    email: process.env.MENTOR_USER_NAME!,
    password: process.env.MENTOR_PASSWORD!,
};

export const test = baseTest.extend<AuthFixtures>({
    loggedInPageByAdminRole: async ({ page }, use) => {
        await loginStep(page, defaultAdmin);
        await use(page);
        await page.context().clearCookies();
        await page.evaluate(() => {
            localStorage.clear();
            sessionStorage.clear();
        });
    },
    loggedInPageByLearnerRole: async ({ page }, use) => {
        await loginStep(page, defaultLearner);
        await use(page);
        await page.context().clearCookies();
        await page.evaluate(() => {
            localStorage.clear();
            sessionStorage.clear();
        });
    },
    loggedInPageByMentorRole: async ({ page }, use) => {
        await loginStep(page, defaultMentor);
        await use(page);
        await page.context().clearCookies();
        await page.evaluate(() => {
            localStorage.clear();
            sessionStorage.clear();
        });
    }
});
