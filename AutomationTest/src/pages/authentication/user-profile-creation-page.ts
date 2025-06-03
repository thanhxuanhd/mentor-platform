import { Locator, Page } from "@playwright/test";
import { BasePage } from "../base-page";
import path from "path";

export class UserProfileSetupPage extends BasePage {
  private TXT_AVATAR: Locator;
  private TXT_FULL_NAME: Locator;
  private TXT_PHONE_NUMBER: Locator;
  private TXT_BIO: Locator;
  private DDL_ROLE: (role: string) => Locator;
  private DDL_EXPERTISE: Locator;
  private DDL_EXPERTISE_OPTIONS: (expertise: string) => Locator;
  private TXT_SKILLS: Locator;
  private TXT_INDUSTRY_EXPERIENCE: Locator;
  private DDL_AVAILBILITY: Locator;
  private DDL_COMMUNICATION_METHOD: (method: string) => Locator;
  private TXT_OBJECTIVE: Locator;
  private BTN_NEXT_STEP: Locator;

  constructor(page: Page) {
    super(page);

    this.TXT_AVATAR = page.locator("#user_profile_form_profilePhotoUrl");
    this.TXT_FULL_NAME = page.locator("#user_profile_form_fullName");
    this.TXT_PHONE_NUMBER = page.locator("#user_profile_form_phoneNumber");
    this.TXT_BIO = page.locator("#user_profile_form_bio");
    this.DDL_ROLE = (role) => {
      return this.page.locator(
        `.ant-radio-button-label span.font-medium:has-text('${role}')`
      );
    };
    this.DDL_EXPERTISE = page.locator("#user_profile_form_expertiseIds");
    this.DDL_EXPERTISE_OPTIONS = (expertise) => {
      return page
        .locator(`div.ant-select-item-option-content`)
        .filter({ hasText: new RegExp(expertise, "i") });
    };
    this.TXT_SKILLS = page.locator("#user_profile_form_skills");
    this.TXT_INDUSTRY_EXPERIENCE = page.locator(
      "#user_profile_form_experiences"
    );
    this.DDL_AVAILBILITY = page.locator(
      "#user_profile_form_availabilityIds button"
    );
    this.DDL_COMMUNICATION_METHOD = (expertise) => {
      return page.locator(
        `#user_profile_form_preferredCommunicationMethod label:has-text('${expertise}')`
      );
    };
    this.TXT_OBJECTIVE = page.locator("#user_profile_form_goal");
    this.BTN_NEXT_STEP = page.getByRole("button", { name: "Next Step" });
  }

  async selectAvatarPhoto(imgURL: string) {
    const filePath = path.resolve(
      __dirname,
      `../../tests/test-data/img/${imgURL}`
    );
    await this.TXT_AVATAR.setInputFiles(filePath);
  }

  async fillInFullnameField(fullname: string) {
    await this.fill(this.TXT_FULL_NAME, fullname);
  }

  async fillInPhoneField(phone: string) {
    await this.fill(this.TXT_PHONE_NUMBER, phone);
  }

  async fillInBioField(bio: string) {
    if (bio) await this.fill(this.TXT_BIO, bio);
  }

  async selectUserRole(role: string) {
    if (role) {
      const ddl_role_loc = this.DDL_ROLE(role);
      await this.click(ddl_role_loc);
    }
  }

  async selectExpertise(expertise: string[]) {
    await this.selectFromDropdown(
      this.DDL_EXPERTISE,
      this.DDL_EXPERTISE_OPTIONS,
      expertise
    );
  }

  async fillProfessionalSkillsField(skills: string) {
    if (skills) await this.fill(this.TXT_SKILLS, skills);
  }

  async fillExperienceField(experience: string) {
    if (experience) await this.fill(this.TXT_INDUSTRY_EXPERIENCE, experience);
  }

  async selectAvailbilityOptions(availbility: number[]) {
    for (const item of availbility) {
      await this.click(this.DDL_AVAILBILITY.nth(item));
    }
  }

  async selectCommunicationMethod(method: string) {
    if (method) {
      const communicationLOC = this.DDL_COMMUNICATION_METHOD(method);
      await this.click(communicationLOC);
    }
  }

  async fillObjectiveField(objective: string) {
    if (objective) await this.fill(this.TXT_OBJECTIVE, objective);
  }

  async clickOnNextStepButton() {
    await this.click(this.BTN_NEXT_STEP);
  }
}
