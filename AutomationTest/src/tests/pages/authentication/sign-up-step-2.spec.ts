import { test } from "../../../core/fixture/authFixture";
import { withTimestampEmail } from "../../../core/utils/generate-unique-data";
import { SignUpUser } from "../../../models/user/user";
import { SignUpPage } from "../../../pages/authentication/sign-up-page";
import testData from "../../test-data/sign-up-user-data.json";
import testDataStep2 from "../../test-data/sign-up-step-2-data.json";
import testDataStep1 from "../../test-data/sign-up-user-data.json";
import { SignUpStep2Page } from "../../../pages/authentication/sign-up-step-2-page";

test.describe("@Registration Sign Up test", () => {
  let registrationstep1Page: SignUpPage;
  let registrationstep2Page: SignUpStep2Page;
  const registrationStep1Data = testDataStep1.valid_case;
  const registrationStep2Data = testDataStep2.valid_case;

  test.beforeEach(async ({ page }) => {
    registrationstep1Page = new SignUpPage(page);
    registrationstep2Page = new SignUpStep2Page(page);
    await registrationstep1Page.goToRegistrationStep1Modal();
  });

  test(`Valid case - Registration Step 2`, async ({ page }) => {
    await test.step("Input details data and submit", async () => {
      await registrationstep1Page.inputEmail(registrationStep1Data.email);
      await registrationstep1Page.inputPassword(registrationStep1Data.password);
      await registrationstep1Page.inputConfirmPassword(
        registrationStep1Data.confirmPassword
      );
      await registrationstep1Page.checkOnTermCheckBox(
        registrationStep1Data.isTermCheck
      );
      await registrationstep1Page.clickContinueToProfileSetupButton();
    });

    await test.step("Input details data and submit", async () => {
      await registrationstep2Page.fillInFullnameField(
        registrationStep2Data.fullname
      );
      await registrationstep2Page.fillInPhoneField(
        registrationStep2Data.phoneNumber
      );
      await registrationstep2Page.selectAvailbilityOptions(
        registrationStep2Data.availbility
      );
      await registrationstep2Page.clickOnNextStepButton();
    });
  });
});
