import { AvailabilityPage } from "../../pages/schedule/availability-management";
import { SessionBookingPage } from "../../pages/schedule/session-booking";

export async function setupAvailability(page: any): Promise<AvailabilityPage> {
    const availabilityPage = new AvailabilityPage(page);
    await availabilityPage.goToAvailabilityManagementPage();
    await availabilityPage.clickTodayButton();
    await availabilityPage.selectStartTime("00:30");
    await availabilityPage.selectEndTime("23:30");
    await availabilityPage.selectDuration("30 minutes");
    await availabilityPage.selectBufferTime("0 minutes");
    await availabilityPage.clickSelectAllButton();
    await availabilityPage.clickSaveChangesButton();
    return availabilityPage;
}

export async function setupBookSession(page: any): Promise<SessionBookingPage> {
    const sessionBookingPage = new SessionBookingPage(page);
    await sessionBookingPage.goToSessionBookingPage();
    await sessionBookingPage.clickSelectMentorButton();
    await sessionBookingPage.selectAvailableMentor();
    await sessionBookingPage.selectCurrentDay();
    await sessionBookingPage.selectTimeSlot();
    await sessionBookingPage.selectSessionType();
    await sessionBookingPage.clickConfirmBookingButton();
    return sessionBookingPage;
}
