import { UserProfileSetupPage } from './../../../pages/authentication/user-profile-creation-page';
import { fillAndSubmitRegistrationStep1, fillAndSubmitRegistrationStep2, loginAgain } from '../../../core/utils/registration-helper';
import { test } from '../../../core/fixture/authFixture';
import { LoginUser, SignUpUser, UserPreferencesSetup, UserProfileCreation } from '../../../models/user/user';
import signUpData from '../../test-data/sign-up-user-data.json';
import profileSetupData from '../../test-data/user-profile-creation-data.json';
import preferenceSetupData from '../../test-data/preferences-setup-user-data.json';
import { withTimestampEmail } from '../../../core/utils/generate-unique-data';
import { SignUpPage } from '../../../pages/authentication/sign-up-page';
import { PreferentsSetupPage } from '../../../pages/authentication/preferences-setup-page';

test.describe('@Registration User Registration test', () => {
    let signUpPage: SignUpPage;
    let profileSetupPage: UserProfileSetupPage;
    let preferencesSetupPage: PreferentsSetupPage;

    const signUpUser: SignUpUser = withTimestampEmail(signUpData.valid_case);
    const profileSetupUser: UserProfileCreation = profileSetupData.valid_case
    const createdUser: LoginUser = {
        email: signUpUser.email,
        password: signUpUser.password
    };

    test.beforeEach(async ({ page }) => {
        signUpPage = new SignUpPage(page);
        profileSetupPage = new UserProfileSetupPage(page);
        preferencesSetupPage = new PreferentsSetupPage(page);

        await fillAndSubmitRegistrationStep1(signUpPage, signUpUser);
        await fillAndSubmitRegistrationStep2(profileSetupPage, profileSetupUser);
    });

    const validserData: { [label: string]: UserPreferencesSetup } = {
        '@SmokeTest Valid Case': preferenceSetupData.valid_case
    };

    const invalidUserData: { [label: string]: UserPreferencesSetup } = {
        'Create with empty topics': preferenceSetupData.empty_topics,
        'Create with empty teaching approach': preferenceSetupData.empty_teaching_approach
    };

    for (const [label, data] of Object.entries(validserData)) {
        test(`${label} - User Registration 3 steps`, async ({ page }) => {
            await test.step('Input details data and submit', async () => {
                await preferencesSetupPage.selectTopics(data.topics);
                await preferencesSetupPage.selectMultipleTeachingApproaches(data.teachApproach);
                await preferencesSetupPage.checkOnPrivateProfileCheckbox(data.isPrivateProfile);
                await preferencesSetupPage.checkOnAllowMessagesCheckbox(data.isAllowMessage);
                await preferencesSetupPage.checkOnReceiveNotificationsCheckbox(data.isReceiveNotification);
                await preferencesSetupPage.clickCompleteRegistrationButton();
            });

            await test.step('Verify system behavior', async () => {
                await loginAgain(page, createdUser);
            });
        });
    }

    for (const [label, data] of Object.entries(invalidUserData)) {
        test(`${label} - User Registration 3 steps`, async ({ page }) => {
            await test.step('Input details data and submit', async () => {
                await preferencesSetupPage.selectTopics(data.topics);
                await preferencesSetupPage.selectMultipleTeachingApproaches(data.teachApproach);
                await preferencesSetupPage.checkOnPrivateProfileCheckbox(data.isPrivateProfile);
                await preferencesSetupPage.checkOnAllowMessagesCheckbox(data.isAllowMessage);
                await preferencesSetupPage.checkOnReceiveNotificationsCheckbox(data.isReceiveNotification);
                await preferencesSetupPage.clickCompleteRegistrationButton();
            });

            await test.step('Verify system behavior', async () => {
                await preferencesSetupPage.expectMessage(data.expectedMessage);
            });
        });
    }
});
