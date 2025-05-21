import { test } from '../../../core/fixture/authFixture';
import { requestNewPasswordFromEmail } from '../../../core/utils/get-new-password-from-email';
import { ResetPasswordUser } from '../../../models/user/user';
import { ResetPasswordPage } from '../../../pages/authentication/reset-password-page';
import testData from '../../test-data/reset-password-data.json'
import { buildTestCases } from '../../../core/utils/testcase-data-builder';

test.describe('@ResetPassword Reset Password Test', () => {
    let resetPasswordPage: ResetPasswordPage;

    test.beforeEach(async ({ page }) => {
        resetPasswordPage = new ResetPasswordPage(page);
        await resetPasswordPage.goToForgotPasswordModal();
    });

    const userData = buildTestCases<ResetPasswordUser>({
        '@SmokeTest Valid Case': testData.valid_case,
        'Empty Email': testData.empty_email,
        'Disable/Deleted Email': testData.disabled_deleted_notexist_email,
        '@SmokeTest @InvalidCase Wrong current password': testData.wrong_current_password,
        '@InvalidCase Empty current password': testData.empty_current_password,
        'Empty new password': testData.empty_new_password,
        'Over max length new password': testData.over_max_length_new_password
    });

    for (const { label, data } of userData) {
        test(`${label} - Reset Password`, async ({ request }) => {
            let currentPassword: string;
            let isSuccess: boolean;
            await test.step('Send new password to email', async () => {
                await resetPasswordPage.inputEmail(data.email);
                await resetPasswordPage.clickSendNewPasswordButton();
                await Promise.any([
                    await resetPasswordPage.expectSendSuccess(),
                    await resetPasswordPage.expectSendFail()
                ]);
            });
            isSuccess = await resetPasswordPage.expectSendSuccess();
            if (!isSuccess) return;
            else {
                await test.step('Input details data and submit', async () => {
                    if (label.includes("@InvalidCase")) {
                        currentPassword = data.currentPassword
                    }
                    else {
                        currentPassword = await requestNewPasswordFromEmail(request, data.email)
                    }
                    await resetPasswordPage.inputEmail(data.email);
                    await resetPasswordPage.inputCurrentPassword(currentPassword);
                    await resetPasswordPage.inputNewPassword(data.newPassword);
                    await resetPasswordPage.clickResetPasswordButton();
                });

                await test.step('Verify system behavior', async () => {
                    await resetPasswordPage.expectMessage(data.expectedMessage);
                });
            }
        });
    }
});
