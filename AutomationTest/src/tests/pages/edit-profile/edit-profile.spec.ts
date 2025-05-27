import editUserProfileData from "../../test-data/user-edit-profile-data.json";
import { EditUserProfile } from "../../../pages/edit-user-profile/edit-user-profile-page";
import { test } from "../../../core/fixture/authFixture";
import { EditUserProfileInterface } from "../../../models/edit-user/edit-user";

test.describe("@Edit profile test", () => {
  let editUserProfile: EditUserProfile;

  test.beforeEach(async ({ loggedInPage, page }) => {
    editUserProfile = new EditUserProfile(page);
  });

  const userData: { [label: string]: EditUserProfileInterface } = {
    "Verify update user profile successfully": editUserProfileData.valid_case,
    "Empty fullname": editUserProfileData.empty_fullname,
    "Empty phone number": editUserProfileData.empty_phone_number,
    "Empty availability": editUserProfileData.empty_availability,
    "Incorrect phone number format":
      editUserProfileData.wrong_phone_number_format,
    "Verify display error message when skills exceed 200 characters":
      editUserProfileData.professional_skills_exceed_200_characters,
    "Verify display error message when experience exceed 200 characters":
      editUserProfileData.industry_experience_exceed_200_characters,
  };

  for (const [label, data] of Object.entries(userData)) {
    test(`${label} - Edit profile`, async ({ page }) => {
      await test.step("Input details data and submit", async () => {
        await editUserProfile.navigateToViewProfilePage();
        await editUserProfile.clickOnEditProfileButton();
        await editUserProfile.fillInFullnameField(data.fullname);
        await editUserProfile.fillInPhoneField(data.phoneNumber);
        await editUserProfile.fillInBioField(data.bio!);
        await editUserProfile.selectExpertise(data.expertise!);
        await editUserProfile.fillProfessionalSkillsField(data.skills!);
        await editUserProfile.fillExperienceField(data.experience!);
        await editUserProfile.unselectedAvailabilityOptions();
        await editUserProfile.selectAvailabilityOptions(data.availbility);
        await editUserProfile.selectTeaching(data.teaching!);
        await editUserProfile.selectCategory(data.category!);
        await editUserProfile.selectCommunicationMethod(
          data.communication_method!
        );
        await editUserProfile.fillObjectiveField(data.objective!);
        await editUserProfile.clickOnSaveChangeButton();
      });

      await test.step("Verify system behavior", async () => {
        await editUserProfile.expectMessage(data.expectedMessage);
      });
    });
  }
});
