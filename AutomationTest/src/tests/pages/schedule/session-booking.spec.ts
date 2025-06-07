import { test } from '../../../core/fixture/auth-fixture';
import { logout } from '../../../core/utils/login-helper';
import { setupAvailability } from '../../../core/utils/schedule-helper';
import { SessionBookingPage } from '../../../pages/schedule/session-booking';

test.describe('@SessionBooking Session Booking tests', () => {
    let sessionBookingPage: SessionBookingPage;

    test.beforeEach(async ({ loggedInPageByMentorRole, page }) => {
        await setupAvailability(page);
        await logout(page);
    });
    test.beforeEach(async ({ loggedInPageByLearnerRole, page }) => {
        sessionBookingPage = new SessionBookingPage(page);
        await sessionBookingPage.goToSessionBookingPage();
    });

    test("Verify when learner book a session successfully", async () => {
        await test.step('Select an available mentor', async () => {
            await sessionBookingPage.clickSelectMentorButton();
            await sessionBookingPage.selectAvailableMentor();
        });

        await test.step('Select day', async () => {
            await sessionBookingPage.selectCurrentDay();
        });

        await test.step('Select time slot and session type', async () => {
            await sessionBookingPage.selectTimeSlot();
            await sessionBookingPage.selectSessionType();
        });

        await test.step('Verify system behavior', async () => {
            await sessionBookingPage.clickConfirmBookingButton();
            await sessionBookingPage.expectMessage("Book successfully! Please wait mentor to accept your booking.");
        });
    });
});