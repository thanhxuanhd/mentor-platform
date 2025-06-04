import { useEffect, useState } from "react"
import { Button, App } from "antd"
import type { Dayjs } from "dayjs"
import dayjs from "dayjs"
import { MentorSelectionModal, type Mentor } from "./components/MentorSelectionModal"
import { CalendarComponent } from "./components/Calendar"
import { mentors, timeSlots } from "./MockData"
import type { NotificationProps } from "../../types/Notification"
import BookedSessionsModal, { type BookedSession } from "./components/BookedSessionsModal"
import MentorProfile from "./components/MentorProfile"
import type { SessionType } from "../../types/enums/SessionType"
import SessionTypeSelector from "./components/SessionTypeSelector"
import TimeSlotSelector from "./components/TimeSlotSelector"

export default function SessionBooking() {
  const [selectedMentor, setSelectedMentor] = useState<Mentor>(mentors[0])
  const [selectedDate, setSelectedDate] = useState<Dayjs | null>(null)
  const [selectedTime, setSelectedTime] = useState<string>("")
  const [selectedSessionType, setSelectedSessionType] = useState<SessionType | null>(null)
  const [showMentorModal, setShowMentorModal] = useState(false)
  const [showBookedSessionsModal, setShowBookedSessionsModal] = useState(false)
  const [currentMonth, setCurrentMonth] = useState(dayjs())
  const sessionTypes: SessionType[] = ["Virtual", "OneOnOne", "OnSite"];
  const [notify, setNotify] = useState<NotificationProps | null>(null);
  const [bookedSessions, setBookedSessions] = useState<BookedSession[]>([
    {
      id: "1",
      mentor: mentors[0],
      date: "2024-12-10",
      time: "10:00 AM",
      sessionType: "Virtual",
      status: "Pending"
    },
    {
      id: "2",
      mentor: mentors[1],
      date: "2024-12-15",
      time: "2:00 PM",
      sessionType: "OneOnOne",
      status: "Approved"
    },
    {
      id: "3",
      mentor: mentors[2],
      date: "2024-11-20",
      time: "11:00 AM",
      sessionType: "Virtual",
      status: "Completed"
    },
  ])
  const { notification } = App.useApp();

  const handleDateSelect = (date: Dayjs) => {
    setSelectedDate(date)
  }

  const handleMonthChange = (month: Dayjs) => {
    setCurrentMonth(month)
  }

  const handleTimeSelect = (time: string) => {
    setSelectedTime(time)
  }

  const handleSessionTypeSelect = (type: SessionType) => {
    setSelectedSessionType(type)
  }

  const handleMentorSelect = (mentor: Mentor) => {
    setSelectedMentor(mentor)
    setShowMentorModal(false)
  }

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

  const handleConfirmBooking = () => {
    if (!selectedDate || !selectedTime || !selectedSessionType || !selectedMentor) return

    const newSession: BookedSession = {
      id: Date.now().toString(),
      mentor: selectedMentor,
      date: selectedDate.format("YYYY-MM-DD"),
      time: selectedTime,
      sessionType: selectedSessionType,
      status: "Pending"
    }

    setBookedSessions((prev) => [newSession, ...prev])

    // Reset form
    setSelectedDate(null)
    setSelectedTime("")
    setSelectedSessionType(null)

    setNotify({
      type: "success",
      message: "Success",
      description: "Book successfully! Please wait mentor to accept your booking.",
    });
  }

  const handleCancelSession = (sessionId: string) => {
    setBookedSessions((prev) =>
      prev.map((session) => (session.id === sessionId ? { ...session, status: "Cancelled" as const } : session)),
    )
  }

  return (
    <div className="min-h-screen bg-slate-800 text-white p-6">
      <div className="max-w-4xl mx-auto">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold mb-2">Book a mentorship session</h1>
          <p className="text-gray-400">Select date, time, and session type</p>
        </div>

        {/* Mentor Profile */}
        <MentorProfile
          selectedMentor={selectedMentor}
          onSelectMentor={() => setShowMentorModal(true)}
          onMessage={() => { }}
          onViewSessions={() => setShowBookedSessionsModal(true)}
        />

        {/* Calendar */}
        <CalendarComponent
          selectedDate={selectedDate}
          currentMonth={currentMonth}
          onDateSelect={handleDateSelect}
          onMonthChange={handleMonthChange}
        />

        {/* Time Slots */}
        <TimeSlotSelector
          timeSlots={timeSlots}
          selectedTime={selectedTime}
          onTimeSelect={handleTimeSelect}
        />

        {/* Session Type */}
        <SessionTypeSelector
          sessionTypes={sessionTypes}
          selectedSessionType={selectedSessionType}
          onSessionTypeSelect={handleSessionTypeSelect}
        />

        {/* Confirm Button */}
        <div className="mt-8">
          <Button
            type="primary"
            size="large"
            className="w-full h-14 bg-orange-500 border-orange-500 hover:bg-orange-600 text-lg font-medium"
            onClick={handleConfirmBooking}
            disabled={!selectedDate || !selectedTime || !selectedSessionType}
          >
            Confirm booking
          </Button>
        </div>

        {/* Mentor Selection Modal */}
        <MentorSelectionModal
          open={showMentorModal}
          onCancel={() => setShowMentorModal(false)}
          mentors={mentors}
          onMentorSelect={handleMentorSelect}
        />

        {/* Booked Sessions Modal */}
        <BookedSessionsModal
          open={showBookedSessionsModal}
          onCancel={() => setShowBookedSessionsModal(false)}
          sessions={bookedSessions}
          onCancelSession={handleCancelSession}
        />
      </div>
    </div>
  )
}