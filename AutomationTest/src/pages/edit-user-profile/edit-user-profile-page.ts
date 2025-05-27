import { Locator, Page } from "@playwright/test";
import { BasePage } from "../base-page";
import { PAGE_ENDPOINT_URL } from "../../core/constants/page-url";

export class EditUserProfile extends BasePage {
  private LNK_PROFILE_URL: string;
  private BTN_EDIT_USER: Locator;
  private TXT_FULL_NAME: Locator;
  private TXT_PHONE_NUMBER: Locator;
  private TXT_BIO: Locator;
  private DDL_EXPERTISE: Locator;
  private DDL_EXPERTISE_OPTIONS: (expertise: string) => Locator;
  private TXT_SKILLS: Locator;
  private TXT_INDUSTRY_EXPERIENCE: Locator;
  private DDL_AVAILBILITY: Locator;
  private DDL_SELECTED_AVAILBILITY: Locator;
  private DDL_TEACHING_APPROACH: Locator;
  private DDL_TEACHING_APPROACH_OPTIONS: (teaching: string) => Locator;
  private DDL_CATEGORIES: Locator;
  private DDL_CATEGORIES_OPTIONS: (categories: string) => Locator;
  private DDL_COMMUNICATION_METHOD: (method: string) => Locator;
  private TXT_OBJECTIVE: Locator;
  private BTN_SAVE_CHANGE: Locator;

  constructor(page: Page) {
    super(page);
    this.BTN_EDIT_USER = page.getByRole("button", { name: "Edit Profile" });
    this.TXT_FULL_NAME = page.locator("#user_profile_form_fullname");
    this.TXT_PHONE_NUMBER = page.locator("#user_profile_form_phone");
    this.TXT_BIO = page.locator("#user_profile_form_bio");
    this.DDL_EXPERTISE = page.locator("#user_profile_form_expertise");
    this.DDL_EXPERTISE_OPTIONS = (expertise) => {
      return page.locator(
        `div.ant-select-item-option-content:has-text('${expertise}')`
      );
    };
    this.TXT_SKILLS = page.locator("#user_profile_form_skills");
    this.TXT_INDUSTRY_EXPERIENCE = page.locator(
      "#user_profile_form_experience"
    );
    this.DDL_AVAILBILITY = page.locator(
      "#user_profile_form_availability button"
    );
    this.DDL_SELECTED_AVAILBILITY = page.locator(
      "#user_profile_form_availability .ant-btn-primary"
    );
    this.DDL_TEACHING_APPROACH = page.locator(
      "#user_profile_form_teachingApproach"
    );
    this.DDL_TEACHING_APPROACH_OPTIONS = (teaching) => {
      return page.locator(
        `div.ant-select-item-option-content:has-text('${teaching}')`
      );
    };
    this.DDL_CATEGORIES = page.locator("#user_profile_form_categoryIds");
    this.DDL_CATEGORIES_OPTIONS = (teaching) => {
      return page.locator(
        `div.ant-select-item-option-content:has-text('${teaching}')`
      );
    };
    this.DDL_COMMUNICATION_METHOD = (communication) => {
      return page.locator(
        `#user_profile_form_communicationMethod label:has-text('${communication}')`
      );
    };
    this.TXT_OBJECTIVE = page.locator("#user_profile_form_objective");
    this.BTN_SAVE_CHANGE = page.getByRole("button", { name: "Save Changes" });
    this.LNK_PROFILE_URL = PAGE_ENDPOINT_URL.VIEW_PROFILE;
  }

  async navigateToViewProfilePage() {
    await this.page.goto(this.LNK_PROFILE_URL);
  }
  async clickOnEditProfileButton() {
    await this.click(this.BTN_EDIT_USER);
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

  async selectExpertise(expertise: string[]) {
    if (expertise) {
      await this.click(this.DDL_EXPERTISE);
      for (const item of expertise) {
        const expertise_loc = this.DDL_EXPERTISE_OPTIONS(item);
        await this.click(expertise_loc);
      }
      await this.click(this.DDL_EXPERTISE);
    }
  }

  async selectTeaching(teaching: string[]) {
    if (teaching) {
      await this.click(this.DDL_TEACHING_APPROACH);
      for (const item of teaching) {
        const teaching_loc = this.DDL_TEACHING_APPROACH_OPTIONS(item);
        await this.click(teaching_loc);
      }
      await this.click(this.DDL_TEACHING_APPROACH);
    }
  }

  async selectCategory(category: string[]) {
    if (category) {
      await this.click(this.DDL_CATEGORIES);
      for (const item of category) {
        const category_loc = this.DDL_CATEGORIES_OPTIONS(item);
        await this.click(category_loc);
      }
      await this.click(this.DDL_CATEGORIES);
    }
  }

  async fillProfessionalSkillsField(skills: string) {
    if (skills) await this.fill(this.TXT_SKILLS, skills);
  }

  async fillExperienceField(experience: string) {
    if (experience) await this.fill(this.TXT_INDUSTRY_EXPERIENCE, experience);
  }

  async selectAvailabilityOptions(availbility: number[]) {
    if (availbility) {
      for (const item of availbility) {
        await this.click(this.DDL_AVAILBILITY.nth(item));
      }
    }
  }

  async unselectedAvailabilityOptions() {
    const optionsCount = await this.DDL_SELECTED_AVAILBILITY.count();
    for (let i = 0; i < optionsCount; i++) {
      const button = this.DDL_SELECTED_AVAILBILITY.nth(0);
      await button.click();
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

  async clickOnSaveChangeButton() {
    await this.click(this.BTN_SAVE_CHANGE);
  }
}
