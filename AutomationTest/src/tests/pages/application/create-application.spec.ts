import applicationData from '../../test-data/create_application-data.json';
import { test } from '../../../core/fixture/auth-fixture';
import { MentorApplicationPage } from '../../../pages/application/mentor-application-page';
import { CreateApplication } from '../../../models/application/create-application';

test.describe('@Application Create application tests', () => {
    let mentorApplicationPage: MentorApplicationPage;

    test.beforeEach(async ({ loggedInPageByMentorRole, page }) => {
        mentorApplicationPage = new MentorApplicationPage(page);
        await mentorApplicationPage.goToMentorApplicationPage();
    });

    const applications: { [label: string]: CreateApplication } = {
        '@SmokeTest Valid Application': applicationData.create_valid_application,
        '@Boundary Over length education': applicationData.overlength_education,
        '@Boundary Over length work experience': applicationData.overlength_work_experience,
        '@Boundary Over length certification': applicationData.overlength_certification,
        '@Boundary Over length motivation statement': applicationData.overlength_motivation
    };

    for (const [label, data] of Object.entries(applications)) {
        test(`${label} - Create a new Application`, async () => {
            await test.step('Input application details and submit', async () => {
                await mentorApplicationPage.inputEducation(data.education);
                await mentorApplicationPage.inputWorkExperience(data.workExperience);
                await mentorApplicationPage.inputCertification(data.certifications);
                await mentorApplicationPage.inputMotivation(data.motivationStatement);
                await mentorApplicationPage.clickSubmitButton();
            });
            await test.step('Verify system behavior', async () => {
                await mentorApplicationPage.expectMessage(data.expectedMessage);
            });
        });
    }
});

