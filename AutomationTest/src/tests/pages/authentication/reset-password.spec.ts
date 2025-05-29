import { test } from '../../../core/fixture/authFixture';
import { requestNewPasswordFromEmail } from '../../../core/utils/get-new-password-from-email';
import { ResetPasswordUser } from '../../../models/user/user';
import { ResetPasswordPage } from '../../../pages/authentication/reset-password-page';
import testData from '../../test-data/reset-password-data.json'

test.describe.serial('@ResetPassword Reset Password Test', () => {
    let resetPasswordPage: ResetPasswordPage;

    test.beforeEach(async ({ page }) => {
        resetPasswordPage = new ResetPasswordPage(page);
        await resetPasswordPage.goToForgotPasswordModal();
    });

    const invalidData: { [label: string]: ResetPasswordUser } = {
        'Empty new password': testData.empty_new_password,
        'Over max length new password': testData.over_max_length_new_password
    };

    const invalidUserData: { [label: string]: ResetPasswordUser } = {
        '@SmokeTest @InvalidCase Wrong current password': testData.wrong_current_password,
        '@InvalidCase Empty current password': testData.empty_current_password,
    };

    const failedUserData: { [label: string]: ResetPasswordUser } = {
        'Empty Email': testData.empty_email,
        'Disable/Deleted Email': testData.disabled_deleted_notexist_email
    };

    test('@SmokeTest Valid Case - Reset Password', async ({ request }) => {
        const data = testData.valid_case;
        let currentPassword: string;

        await test.step('Send new password to email', async () => {
            await resetPasswordPage.inputEmail(data.email);
            await resetPasswordPage.clickSendNewPasswordButton();
            await Promise.any([
                await resetPasswordPage.expectSendSuccess(),
                await resetPasswordPage.expectSendFail()
            ]);
        });

        await test.step('Input details data and submit', async () => {
            currentPassword = await requestNewPasswordFromEmail(request, data.email);
            await resetPasswordPage.inputEmail(data.email);
            await resetPasswordPage.inputCurrentPassword(currentPassword);
            await resetPasswordPage.inputNewPassword(data.newPassword);
            await resetPasswordPage.clickResetPasswordButton();
        });

        await test.step('Verify system behavior', async () => {
            await resetPasswordPage.expectMessage(data.expectedMessage);
        });
    });

    for (const [label, data] of Object.entries(invalidData)) {
        if (label === '@SmokeTest Valid Case') continue;

        test(`${label} - Reset Password`, async ({ request }) => {
            let currentPassword: string;

            await test.step('Send new password to email', async () => {
                await resetPasswordPage.inputEmail(data.email);
                await resetPasswordPage.clickSendNewPasswordButton();
                await Promise.any([
                    await resetPasswordPage.expectSendSuccess(),
                    await resetPasswordPage.expectSendFail()
                ]);
            });

            await test.step('Input details data and submit', async () => {
                currentPassword = await requestNewPasswordFromEmail(request, data.email);
                await resetPasswordPage.inputEmail(data.email);
                await resetPasswordPage.inputCurrentPassword(currentPassword);
                await resetPasswordPage.inputNewPassword(data.newPassword);
                await resetPasswordPage.clickResetPasswordButton();
            });

            await test.step('Verify system behavior', async () => {
                await resetPasswordPage.expectMessage(data.expectedMessage);
            });
        });
    }

    for (const [label, data] of Object.entries(invalidUserData)) {
        test(`${label} - Reset Password`, async ({ request }) => {
            let currentPassword: string;

            await test.step('Send new password to email', async () => {
                await resetPasswordPage.inputEmail(data.email);
                await resetPasswordPage.clickSendNewPasswordButton();
                await Promise.any([
                    await resetPasswordPage.expectSendSuccess(),
                    await resetPasswordPage.expectSendFail()
                ]);
            });

            await test.step('Input details data and submit', async () => {
                currentPassword = data.currentPassword;
                await resetPasswordPage.inputEmail(data.email);
                await resetPasswordPage.inputCurrentPassword(currentPassword);
                await resetPasswordPage.inputNewPassword(data.newPassword);
                await resetPasswordPage.clickResetPasswordButton();
            });

            await test.step('Verify system behavior', async () => {
                await resetPasswordPage.expectMessage(data.expectedMessage);
            });
        });
    }

    for (const [label, data] of Object.entries(failedUserData)) {
        test(`${label} - Reset Password`, async ({ request }) => {
            await test.step('Send new password to email', async () => {
                await resetPasswordPage.inputEmail(data.email);
                await resetPasswordPage.clickSendNewPasswordButton();
                await resetPasswordPage.expectSendFail();
            });
        });
    }
});

