import { test } from "../../../core/fixture/auth-fixture";
import { withTimestampEmail } from "../../../core/utils/generate-unique-data";
import { SignUpUser, UserProfileCreation } from "../../../models/user/user";
import { SignUpPage } from "../../../pages/authentication/sign-up-page";
import profileSetupData from "../../test-data/user-profile-creation-data.json";
import signUpData from '../../test-data/sign-up-user-data.json'
import { UserProfileSetupPage } from "../../../pages/authentication/user-profile-creation-page";
import { fillAndSubmitRegistrationStep1 } from "../../../core/utils/registration-helper";

test.describe("@Registration Sign Up test", () => {
  let signUpPage: SignUpPage;
  let profileSetupPage: UserProfileSetupPage;
  const validSignUpData: SignUpUser = withTimestampEmail(signUpData.valid_case);

  test.beforeEach(async ({ page }) => {
    signUpPage = new SignUpPage(page);
    profileSetupPage = new UserProfileSetupPage(page);

    await fillAndSubmitRegistrationStep1(signUpPage, validSignUpData);
  });

  const userData: { [label: string]: UserProfileCreation } = {
    "Verify create user profile successfully": profileSetupData.valid_case,
    "Empty fullname": profileSetupData.empty_fullname,
    "Empty phone number": profileSetupData.empty_phone_number,
    "Incorrect phone number format": profileSetupData.wrong_phone_number_format,
    "Empty availability": profileSetupData.empty_availability,
    "Verify user can search for expertise value":
      profileSetupData.search_expertise,
    "Verify display error message when skills exceed 200 characters":
      profileSetupData.professional_skills_exceed_200_characters,
    "Verify display error message when experience exceed 200 characters":
      profileSetupData.industry_experience_exceed_200_characters,
  };

  for (const [label, data] of Object.entries(userData)) {
    test(`${label} - Registration Step 2`, async ({ page }) => {
      await test.step("Input details data and submit", async () => {
        await profileSetupPage.fillInFullnameField(data.fullname);
        await profileSetupPage.fillInPhoneField(data.phoneNumber);
        await profileSetupPage.fillInBioField(data.bio!);
        await profileSetupPage.selectUserRole(data.role!);
        await profileSetupPage.selectExpertise(data.expertise!);
        await profileSetupPage.fillProfessionalSkillsField(data.skills!);
        await profileSetupPage.fillExperienceField(data.experience!);
        await profileSetupPage.selectAvailbilityOptions(data.availbility);
        await profileSetupPage.selectCommunicationMethod(
          data.communication_method!
        );
        await profileSetupPage.fillObjectiveField(data.objective!);
        await profileSetupPage.clickOnNextStepButton();
      });

      await test.step("Verify system behavior", async () => {
        await profileSetupPage.expectMessage(data.expectedMessage);
      });
    });
  }
});
