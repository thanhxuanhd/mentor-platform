import { SignUpPage } from '../../pages/authentication/sign-up-page';
import { LoginUser, SignUpUser, UserProfileCreation } from '../../models/user/user';
import { UserProfileSetupPage } from '../../pages/authentication/user-profile-creation-page';
import { LoginPage } from '../../pages/authentication/login-page';

export async function fillAndSubmitRegistrationStep1(
    page: SignUpPage,
    data: SignUpUser
): Promise<void> {
    await page.goToRegistrationStep1Modal();
    await page.inputEmail(data.email);
    await page.inputPassword(data.password);
    await page.inputConfirmPassword(data.confirmPassword);
    await page.checkOnTermCheckBox(data.isTermCheck);
    await page.clickContinueToProfileSetupButton();
    await page.expectMessage(data.expectedMessage);
}

export async function fillAndSubmitRegistrationStep2(
    page: UserProfileSetupPage,
    data: UserProfileCreation
): Promise<void> {
    await page.fillInFullnameField(data.fullname);
    await page.fillInPhoneField(data.phoneNumber);
    await page.fillInBioField(data.bio!);
    await page.selectUserRole(data.role!);
    await page.selectExpertise(data.expertise!);
    await page.fillProfessionalSkillsField(data.skills!);
    await page.fillExperienceField(data.experience!);
    await page.selectAvailbilityOptions(data.availbility);
    await page.selectCommunicationMethod(
        data.communication_method!
    );
    await page.fillObjectiveField(data.objective!);
    await page.clickOnNextStepButton();
}

export async function loginAgain(
    page: any,
    user: LoginUser
): Promise<void> {
    const loginPage = new LoginPage(page);
    await loginPage.inputEmail(user.email);
    await loginPage.inputPassword(user.password);
    await loginPage.clickSignInButton();
    await loginPage.expectLogoutButton();
}

