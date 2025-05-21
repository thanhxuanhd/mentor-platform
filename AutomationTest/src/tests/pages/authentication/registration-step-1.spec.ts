import { RegistrationStep1Page } from './../../../pages/authentication/registration-step-1-page';
import { test } from '../../../core/fixture/authFixture';
import { User_Registration_Step_1 } from '../../../models/user/user';
import testData from '../../test-data/user-registration-step-1-data.json'
import { buildTestCases } from '../../../core/utils/testcase-data-builder';

test.describe('@Registration Registration step 1 test', () => {
    let registrationstep1Page: RegistrationStep1Page;

    test.beforeEach(async ({ page }) => {
        registrationstep1Page = new RegistrationStep1Page(page);
        await registrationstep1Page.goToRegistrationStep1Modal();
    });

    const userData = buildTestCases<User_Registration_Step_1>({
        '@SmokeTest Valid Case': testData.valid_case,
        'Term is unchecked': testData.term_is_uncheck,
        'Empty Email': testData.empty_email,
        'Over length email': testData.over_length_email,
        'Incorrect email format': testData.incorrect_format_email,
        '@SmokeTest Already registered email': testData.already_registered_email,
        'Incorrect confirm password': testData.incorrect_confirm_password,
        'Empty Password': testData.empty_password,
        'Over length password': testData.over_length_password,
        'Password include space characters': testData.space_password,
        'Password is not mix': testData.not_mix_password
    });

    for (const { label, data } of userData) {
        test(`${label} - Registration Step 1`, async ({ request }) => {
            await test.step('Input details data and submit', async () => {
                await registrationstep1Page.inputEmail(data.email);
                await registrationstep1Page.inputPassword(data.password);
                await registrationstep1Page.inputConfirmPassword(data.confirmPassword);
                await registrationstep1Page.checkOnTermCheckBox(data.isTermCheck);
                await registrationstep1Page.clickContinueToProfileSetupButton();
            });

            await test.step('Verify system behavior', async () => {
                await registrationstep1Page.expectMessage(data.expectedMessage);
            });
        });
    }
});
