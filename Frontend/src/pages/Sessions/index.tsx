import { useEffect, useState } from "react";
import { Button, App } from "antd";
import type { Dayjs } from "dayjs";
import dayjs from "dayjs";
import { CalendarComponent } from "./components/Calendar";
import type { NotificationProps } from "../../types/Notification";
import BookedSessionsModal from "./components/BookedSessionsModal";
import MentorProfile from "./components/MentorProfile";
import type { SessionType } from "../../types/enums/SessionType";
import SessionTypeSelector from "./components/SessionTypeSelector";
import TimeSlotSelector from "./components/TimeSlotSelector";
import { getAvailableTimeSlots, requestBooking } from "../../services/session-booking/sessionBookingService";
import type { BookedSession, Mentor, TimeSlot } from "../../types/SessionsType";
import { MentorSelectionModal } from "./components/MentorSelectionModal";

export default function SessionBooking() {
  const [selectedMentor, setSelectedMentor] = useState<Mentor | null>(null);
  const [selectedDate, setSelectedDate] = useState<Dayjs | null>(null);
  const [selectedTime, setSelectedTime] = useState<string>("");
  const [selectedTimeSlotId, setSelectedTimeSlotId] = useState<string>("");
  const [selectedSessionType, setSelectedSessionType] = useState<SessionType | null>(null);
  const [showMentorModal, setShowMentorModal] = useState(false);
  const [showBookedSessionsModal, setShowBookedSessionsModal] = useState(false);
  const [currentMonth, setCurrentMonth] = useState(dayjs());
  const [timeSlots, setTimeSlots] = useState<TimeSlot[]>([]);
  const [timeSlotsLoading, setTimeSlotsLoading] = useState(false);
  const sessionTypes: SessionType[] = ["Virtual", "OneOnOne", "Onsite"];
  const [notify, setNotify] = useState<NotificationProps | null>(null);
  const [bookedSessions, setBookedSessions] = useState<BookedSession[]>([]);
  const { notification } = App.useApp();

  useEffect(() => {
    if (!selectedMentor || !selectedDate) {
      setTimeSlots([]);
      setSelectedTime("");
      setSelectedTimeSlotId("");
      return;
    }

    const fetchTimeSlots = async () => {
      setTimeSlotsLoading(true);
      try {
        const response = await getAvailableTimeSlots(selectedMentor.id, {
          date: selectedDate.format("YYYY-MM-DD"),
        });
        setTimeSlots(response);
      } catch (error) {
        setNotify({
          type: "error",
          message: "Error",
          description: "Failed to load time slots. Please try again.",
        });
        setTimeSlots([]);
      } finally {
        setTimeSlotsLoading(false);
      }
    };

    fetchTimeSlots();
  }, [selectedMentor, selectedDate]);

  const handleDateSelect = (date: Dayjs) => {
    setSelectedDate(date);
    setSelectedTime("");
    setSelectedTimeSlotId("");
  };

  const handleMonthChange = (month: Dayjs) => {
    setCurrentMonth(month);
  };

  const handleTimeSelect = (time: string, id: string) => {
    setSelectedTime(time);
    setSelectedTimeSlotId(id);
  };

  const handleSessionTypeSelect = (type: SessionType) => {
    setSelectedSessionType(type);
  };

  const handleMentorSelect = (mentor: Mentor) => {
    setSelectedMentor(mentor);
    setShowMentorModal(false);
    setSelectedTime("");
    setSelectedTimeSlotId("");
  };

  useEffect(() => {
    if (notify) {
      notification[notify.type]({
        message: notify.message,
        description: notify.description,
        placement: "topRight",
      });
      setNotify(null);
    }
  }, [notify, notification]);

  const handleConfirmBooking = async () => {
    if (!selectedDate || !selectedTime || !selectedSessionType || !selectedMentor || !selectedTimeSlotId) return;

    try {
      const bookingRequest = {
        timeSlotId: selectedTimeSlotId,
        sessionType: selectedSessionType,
      };

      const response = await requestBooking(bookingRequest);

      const newSession: BookedSession = {
        id: response.sessionId,
        mentor: selectedMentor,
        date: response.day,
        startTime: response.startTime.slice(0, 5),
        endTime: response.endTime.slice(0, 5),
        type: selectedSessionType,
        status: response.bookingStatus as "Pending"
      };

      setBookedSessions((prev) => [newSession, ...prev]);

      setSelectedDate(null);
      setSelectedTime("");
      setSelectedTimeSlotId("");
      setSelectedSessionType(null);
      setSelectedMentor(null);

      setNotify({
        type: "success",
        message: "Success",
        description: "Book successfully! Please wait mentor to accept your booking.",
      });
    } catch (error: string | any) {
      setNotify({
        type: "error",
        message: "Booking Failed",
        description: error.response.data.error || "An error occurred while booking the session. Please try again.",
      });
    }
  };

  const handleCancelSession = (sessionId: string) => {
    setBookedSessions((prev) =>
      prev.map((session) => (session.id === sessionId ? { ...session, status: "Cancelled" as const } : session)),
    );
  };

  return (
    <div className="min-h-screen bg-slate-800 text-white p-6">
      <div className="max-w-4xl mx-auto">
        <div className="mb-8">
          <h1 className="text-3xl font-bold mb-2">Book a mentorship session</h1>
          <p className="text-gray-400">Select date, time, and session type</p>
        </div>

        <MentorProfile
          selectedMentor={selectedMentor}
          onSelectMentor={() => setShowMentorModal(true)}
          onMessage={() => {
            setNotify({
              type: "info",
              message: "Message",
              description: "Messaging functionality not implemented yet.",
            });
          }}
          onViewSessions={() => setShowBookedSessionsModal(true)}
        />

        <CalendarComponent
          selectedDate={selectedDate}
          currentMonth={currentMonth}
          onDateSelect={handleDateSelect}
          onMonthChange={handleMonthChange}
        />

        <TimeSlotSelector
          timeSlots={timeSlots}
          selectedTime={selectedTime}
          onTimeSelect={handleTimeSelect}
          loading={timeSlotsLoading}
        />

        <SessionTypeSelector
          sessionType={sessionTypes}
          selectedSessionType={selectedSessionType}
          onSessionTypeSelect={handleSessionTypeSelect}
        />

        <div className="mt-2">
          <Button
            type="primary"
            size="large"
            className="w-full h-14 bg-orange-500 border-orange-500 hover:bg-orange-600 text-lg font-medium"
            onClick={handleConfirmBooking}
            disabled={!selectedDate || !selectedTime || !selectedSessionType || !selectedMentor || !selectedTimeSlotId}
          >
            Confirm booking
          </Button>
        </div>

        <MentorSelectionModal
          open={showMentorModal}
          onCancel={() => setShowMentorModal(false)}
          onMentorSelect={handleMentorSelect}
        />

        <BookedSessionsModal
          open={showBookedSessionsModal}
          onCancel={() => setShowBookedSessionsModal(false)}
          sessions={bookedSessions}
          onCancelSession={handleCancelSession}
        />
      </div>
    </div>
  );
}