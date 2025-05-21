import { test } from '../../../core/fixture/authFixture';
import { requestNewPasswordFromEmail } from '../../../core/utils/get-new-password-from-email';
import { ResetPasswordUser } from '../../../models/user/user';
import { ResetPasswordPage } from '../../../pages/authentication/reset-password-page';
import testData from '../../test-data/reset-password-data.json'

test.describe('@ResetPassword Reset Password Test', () => {
    let resetPasswordPage: ResetPasswordPage;

    test.beforeEach(async ({ page }) => {
        resetPasswordPage = new ResetPasswordPage(page);
        await resetPasswordPage.goToForgotPasswordModal();
    });

    const userData = [
        {
            label: '@SmokeTest Valid Case',
            data: testData.valid_case as ResetPasswordUser,
        }
    ];

    for (const { label, data } of userData) {
        test(`${label} - Reset Password`, async ({ request }) => {
            let currentPassword: string;

            await test.step('Send new password to email', async () => {
                await resetPasswordPage.inputEmail(data.email);
                await resetPasswordPage.clickSendNewPasswordButton();
                await resetPasswordPage.expectSendSuccess();
                currentPassword = await requestNewPasswordFromEmail(request, data.email);
            });

            await test.step('Input details data and submit', async () => {
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
});
