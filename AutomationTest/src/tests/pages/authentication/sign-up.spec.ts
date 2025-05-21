import { SignUpPage } from '../../../pages/authentication/sign-up-page';
import { test } from '../../../core/fixture/authFixture';
import { SignUpUser } from '../../../models/user/user';
import testData from '../../test-data/sign-up-user-data.json'

test.describe('@Registration Sign Up test', () => {
    let registrationstep1Page: SignUpPage;

    test.beforeEach(async ({ page }) => {
        registrationstep1Page = new SignUpPage(page);
        await registrationstep1Page.goToRegistrationStep1Modal();
    });

    const userData: { [label: string]: SignUpUser } = {
        //Todo: Uncomment when (User Story 165: User Profile Creation - Step 2) is done
        // '@SmokeTest Valid Case': testData.valid_case,
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
    };

    for (const [label, data] of Object.entries(userData)) {
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
