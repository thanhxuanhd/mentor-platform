import { test } from "../../../core/fixture/authFixture";
import { withTimestampEmail } from "../../../core/utils/generate-unique-data";
import { UserProfileCreation } from "../../../models/user/user";
import { SignUpPage } from "../../../pages/authentication/sign-up-page";
import testDataStep2 from "../../test-data/user-profile-creation-data.json";
import testDataStep1 from "../../test-data/sign-up-user-data.json";
import { SignUpStep2Page } from "../../../pages/authentication/user-profile-creation-page";

test.describe("@Registration Sign Up test", () => {
  let registrationstep1Page: SignUpPage;
  let registrationstep2Page: SignUpStep2Page;
  const validRegisterStep1Data = testDataStep1.valid_case;

  test.beforeEach(async ({ page }) => {
    registrationstep1Page = new SignUpPage(page);
    registrationstep2Page = new SignUpStep2Page(page);
    await registrationstep1Page.goToRegistrationStep1Modal();

    await test.step("Input details data and submit", async () => {
      const randomEmail = withTimestampEmail(validRegisterStep1Data);
      await registrationstep1Page.inputEmail(randomEmail.email);
      await registrationstep1Page.inputPassword(
        validRegisterStep1Data.password
      );
      await registrationstep1Page.inputConfirmPassword(
        validRegisterStep1Data.confirmPassword
      );
      await registrationstep1Page.checkOnTermCheckBox(
        validRegisterStep1Data.isTermCheck
      );
      await registrationstep1Page.clickContinueToProfileSetupButton();
    });
  });

  const userData: { [label: string]: UserProfileCreation } = {
    "Verify create user profile successfully": testDataStep2.valid_case,
    "Empty fullname": testDataStep2.empty_fullname,
    "Empty phone number": testDataStep2.empty_phone_number,
    "Incorrect phone number format": testDataStep2.wrong_phone_number_format,
    "Verify user can search for expertise value":
      testDataStep2.search_expertise,
    "Verify display error message when skills exceed 200 characters":
      testDataStep2.professional_skills_exceed_200_characters,
    "Verify display error message when experience exceed 200 characters":
      testDataStep2.industry_experience_exceed_200_characters,
  };

  for (const [label, data] of Object.entries(userData)) {
    test(`${label} - Registration Step 2`, async ({ page }) => {
      await test.step("Input details data and submit", async () => {
        await registrationstep2Page.fillInFullnameField(data.fullname);
        await registrationstep2Page.fillInPhoneField(data.phoneNumber);
        await registrationstep2Page.fillInBioField(data.bio!);
        await registrationstep2Page.selectUserRole(data.role!);
        await registrationstep2Page.selectExpertise(data.expertise!);
        await registrationstep2Page.fillProfessionalSkillsField(data.skills!);
        await registrationstep2Page.fillExperienceField(data.experience!);
        await registrationstep2Page.selectAvailbilityOptions(data.availbility);
        await registrationstep2Page.selectCommunicationMethod(
          data.communication_method!
        );
        await registrationstep2Page.fillObjectiveField(data.objective!);
        await registrationstep2Page.clickOnNextStepButton();
      });

      await test.step("Verify system behavior", async () => {
        await registrationstep2Page.expectMessage(data.expectedMessage);
      });
    });
  }
});
