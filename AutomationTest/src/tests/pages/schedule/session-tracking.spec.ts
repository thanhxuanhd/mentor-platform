import { defaultLearner, defaultMentor, test } from '../../../core/fixture/auth-fixture';
import { logout, loginStep } from '../../../core/utils/login-helper';
import { setupAvailability, setupBookSession } from '../../../core/utils/schedule-helper';
import { SessionTrackingPage } from '../../../pages/schedule/session-tracking';
import { LoginUser } from '../../../models/user/user';


test.describe('@SessionTracking Session Tracking tests', () => {
    let sessionTrackingPage: SessionTrackingPage;

    test.beforeEach(async ({ browser, page }) => {
        const mentorContext = await browser.newContext();
        const mentorPage = await mentorContext.newPage();
        await loginStep(mentorPage, defaultMentor);
        await setupAvailability(mentorPage);
        await logout(mentorPage);
        await mentorContext.close();

        const learnerContext = await browser.newContext();
        const learnerPage = await learnerContext.newPage();
        await loginStep(learnerPage, defaultLearner);
        await setupBookSession(learnerPage);
        await logout(learnerPage);
        await learnerContext.close();

        await loginStep(page, defaultMentor);
        sessionTrackingPage = new SessionTrackingPage(page);
        await sessionTrackingPage.goToSessionTrackingPage();
    });

    test('Verify when mentor accept a booked session successfully', async () => {
        await sessionTrackingPage.clickAcceptButton();
        await sessionTrackingPage.expectMessage('Session approved successfully');
    });
});
