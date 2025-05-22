import { Locator, Page } from "@playwright/test";
import { BasePage } from "../base-page";

export class SignUpStep2Page extends BasePage {
  private TXT_FULL_NAME: Locator;
  private TXT_PHONE_NUMBER: Locator;
  private TXT_BIO: Locator;
  private DDL_ROLE = (role: string) => {};
  private DDL_EXPERTISE: Locator;
  private TXT_SKILLS: Locator;
  private TXT_INDUSTRY_EXPERIENCE: Locator;
  private DDL_AVAILBILITY: Locator;
  private DDL_COMMUNICATION_METHOD: Locator;
  private TXT_OBJECTIVE: Locator;
  private BTN_NEXT_STEP: Locator;
  private LBL_EMPTY_ERROR_MESSAGE: Locator;

  constructor(page: Page) {
    super(page);

    this.TXT_FULL_NAME = page.locator("#user_profile_form_fullName");
    this.TXT_PHONE_NUMBER = page.locator("#user_profile_form_phoneNumber");
    this.TXT_BIO = page.locator("#user_profile_form_bio");
    this.DDL_ROLE = (role) => {
      this.page.locator(`.ant-radio-button-label span:has-text('${role}')`);
    };
    this.DDL_EXPERTISE = page.locator("div.ant-select-selector");
    this.TXT_SKILLS = page.locator("#user_profile_form_skills");
    this.TXT_INDUSTRY_EXPERIENCE = page.locator(
      "#user_profile_form_experiences"
    );
    this.DDL_AVAILBILITY = page.locator(
      "#user_profile_form_availabilityIds button"
    );
    this.DDL_COMMUNICATION_METHOD = page.locator(
      "#user_profile_form_preferredCommunicationMethod label"
    );
    this.TXT_OBJECTIVE = page.locator("user_profile_form_goal");
    this.BTN_NEXT_STEP = page.getByRole("button", { name: "Next Step" });
  }

  async fillInFullnameField(fullname: string) {
    await this.fill(this.TXT_FULL_NAME, fullname);
  }

  async fillInPhoneField(phone: string) {
    await this.fill(this.TXT_PHONE_NUMBER, phone);
  }

  async selectAvailbilityOptions(availbility: number) {
    await this.click(this.DDL_AVAILBILITY.nth(availbility));
  }

  async clickOnNextStepButton() {
    await this.click(this.BTN_NEXT_STEP);
  }
}
