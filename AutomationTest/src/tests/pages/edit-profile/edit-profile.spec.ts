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

      await test.step("Fill in fullname field", async () => {
        await editUserProfile.fillInFullnameField(data.fullname);
      });
      
      await test.step("Fill in phone number field", async () => {
        await editUserProfile.fillInPhoneField(data.phoneNumber);
      });
      
      await test.step("Fill in bio field", async () => {
        await editUserProfile.fillInBioField(data.bio!);
      });
      
      await test.step("Select expertise", async () => {
        await editUserProfile.selectExpertise(data.expertise!);
      });
      
      await test.step("Fill in professional skills field", async () => {
        await editUserProfile.fillProfessionalSkillsField(data.skills!);
      });
      
      await test.step("Fill in experience field", async () => {
        await editUserProfile.fillExperienceField(data.experience!);
      });
      
      await test.step("Select availability options", async () => {
        await editUserProfile.unselectAvailabilityOptions();
        await editUserProfile.selectAvailabilityOptions(data.availbility);
      });
      
      await test.step("Select teaching", async () => {
        await editUserProfile.selectTeaching(data.teaching!);
      });
      
      await test.step("Select communication method", async () => {
        await editUserProfile.selectCommunicationMethod(data.communication_method!);
      });
      
      await test.step("Fill in objective field", async () => {
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
