import { test } from '../../../core/fixture/auth-fixture';
import { AvailabilityPage } from '../../../pages/schedule/availability-management';

test.describe('@Availability Availability management tests', () => {
    let availabilityPage: AvailabilityPage;

    test.beforeEach(async ({ loggedInPageByMentorRole, page }) => {
        availabilityPage = new AvailabilityPage(page);
        await availabilityPage.goToAvailabilityManagementPage();
        await availabilityPage.clickTodayButton();
    });

    test("Verify first time slot in list is show correctly due to current time", async () => {
        await test.step('Select all valid time in current day', async () => {
            await availabilityPage.selectStartTime("00:00");
            await availabilityPage.selectEndTime("23:30");
            await availabilityPage.selectDuration("30 minutes");
            await availabilityPage.selectBufferTime("0 minutes");
            await availabilityPage.clickSelectAllButton();
            await availabilityPage.clickSaveChangesButton();
        });
        await test.step('Verify system behavior', async () => {
            await availabilityPage.expectFirstTimeSlotAfterCurrentTime();
        });
    });

    test("Verify when user select end time < start time", async () => {
        await test.step('Select end time < start time', async () => {
            await availabilityPage.selectStartTime("12:00");
            await availabilityPage.selectEndTime("10:00");
        });
        await test.step('Verify system behavior', async () => {
            await availabilityPage.expectMessage("Start time must be before end time");
        });
    });

    test("Verify there is no timeslot base on sesssion setting", async () => {
        await test.step('Select all valid time in current day', async () => {
            await availabilityPage.selectStartTime("12:00");
            await availabilityPage.selectEndTime("13:00");
            await availabilityPage.selectDuration("90 minutes");
            await availabilityPage.selectBufferTime("0 minutes");
        });
        await test.step('Verify system behavior', async () => {
            await availabilityPage.expectMessage("No time slots available for the current settings");
        });
    });
});