import { test as baseTest } from '@playwright/test';
import { LoginUser } from '../../models/user/user';
import { loginStep } from '../utils/login-helper';
import { createTestUser, deleteTestUser } from '../utils/api-helper';
import testUserData from '../../tests/test-data/test-user-data.json';
import { withTimestampEmail } from '../utils/generate-unique-data';

type Role = 'admin' | 'learner' | 'mentor';

export type AuthFixtures = {
    loggedInPageByAdminRole: any;
    loggedInPageByLearnerRole: any;
    loggedInPageByMentorRole: any;
    createTestAdmin: any;
    createTestLearner: any;
    createTestMentor: any;
};

const testUserDataMap: Record<Role, any> = {
    admin: testUserData.test_admin,
    learner: testUserData.test_learner,
    mentor: testUserData.test_mentor,
};

function createLoggedInFixture(role: Role) {
    return async ({ request, page }, use) => {
        const rawData = testUserDataMap[role];
        const userData = withTimestampEmail(rawData);
        const email = await createTestUser(request, userData);
        const createUser: LoginUser = {
            email: userData.email,
            password: userData.password,
        }
        await loginStep(page, createUser);
        await use();
        await page.context().clearCookies();
        await page.evaluate(() => {
            localStorage.clear();
            sessionStorage.clear();
        });
        await deleteTestUser(request, email);
    };
}

export const test = baseTest.extend<AuthFixtures>({
    loggedInPageByAdminRole: createLoggedInFixture('admin'),
    loggedInPageByLearnerRole: createLoggedInFixture('learner'),
    loggedInPageByMentorRole: createLoggedInFixture('mentor'),
});
