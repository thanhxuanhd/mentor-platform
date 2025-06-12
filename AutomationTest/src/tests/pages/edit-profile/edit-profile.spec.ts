import editUserProfileData from "../../test-data/user-edit-profile-data.json";
import { EditUserProfile } from "../../../pages/edit-user-profile/edit-user-profile-page";
import { test } from "../../../core/fixture/auth-fixture";
import { EditUserProfileInterface } from "../../../models/edit-user/edit-user";

test.describe("@Edit profile test", () => {
  let editUserProfile: EditUserProfile;

  test.beforeEach(async ({ loggedInPageByLearnerRole, page }) => {
    editUserProfile = new EditUserProfile(page);
  });

  const userData: { [label: string]: EditUserProfileInterface } = {
    "@SmokeTest @Regression Verify update user profile successfully":
      editUserProfileData.valid_case,
    "@Regression Empty fullname": editUserProfileData.empty_fullname,
    "@Regression Empty phone number": editUserProfileData.empty_phone_number,
    "@Regression Empty availability": editUserProfileData.empty_availability,
    "@Regression Incorrect phone number format":
      editUserProfileData.wrong_phone_number_format,
    "@Regression Verify display error message when skills exceed 200 characters":
      editUserProfileData.professional_skills_exceed_200_characters,
    "@Regression Verify display error message when experience exceed 200 characters":
      editUserProfileData.industry_experience_exceed_200_characters,
  };

  for (const [label, data] of Object.entries(userData)) {
    test(`${label} - Edit profile`, async ({ page }) => {
      await test.step("Go to Profile Page", async () => {
        await editUserProfile.navigateToViewProfilePage();
      });

      await test.step("Click on Edit button", async () => {
        await editUserProfile.clickOnEditProfileButton();
      });

      await test.step("Input details data in all fields", async () => {
        await editUserProfile.fillInFullnameField(data.fullname);
        await editUserProfile.fillInPhoneField(data.phoneNumber);
        await editUserProfile.fillInBioField(data.bio!);
        await editUserProfile.selectExpertise(data.expertise!);
        await editUserProfile.fillProfessionalSkillsField(data.skills!);
        await editUserProfile.fillExperienceField(data.experience!);
        await editUserProfile.unselectAvailabilityOptions();
        await editUserProfile.selectAvailabilityOptions(data.availbility);
        await editUserProfile.selectTeaching(data.teaching!);
        await editUserProfile.selectCategory(data.category!);
        await editUserProfile.selectCommunicationMethod(
          data.communication_method!
        );
        await editUserProfile.fillObjectiveField(data.objective!);
      });

      await test.step("Click on Save change button", async () => {
        await editUserProfile.clickOnSaveChangeButton();
      });

      await test.step("Verify user edit profile successfully", async () => {
        await editUserProfile.expectMessage(data.expectedMessage);
      });
    });
  }
});
