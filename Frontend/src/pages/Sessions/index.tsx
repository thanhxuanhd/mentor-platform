"use client"

import { useEffect, useState } from "react"
import { Button, App } from "antd"
import type { Dayjs } from "dayjs"
import dayjs from "dayjs"
import { CalendarComponent } from "./components/Calendar"
import type { NotificationProps } from "../../types/Notification"
import BookedSessionsModal from "./components/BookedSessionsModal"
import MentorProfile from "./components/MentorProfile"
import type { SessionType } from "../../types/enums/SessionType"
import SessionTypeSelector from "./components/SessionTypeSelector"
import TimeSlotSelector from "./components/TimeSlotSelector"
import { getAvailableTimeSlots, requestBooking } from "../../services/session-booking/sessionBookingService"
import type { BookedSession, Mentor, TimeSlot } from "../../types/SessionsType"
import { MentorSelectionModal } from "./components/MentorSelectionModal"
import { convertUTCDateTimeToLocal } from "../../utils/timezoneUtils"

export default function SessionBooking() {
  const [selectedMentor, setSelectedMentor] = useState<Mentor | null>(null)
  const [selectedDate, setSelectedDate] = useState<Dayjs | null>(null)
  const [selectedTime, setSelectedTime] = useState<string>("")
  const [selectedTimeSlotId, setSelectedTimeSlotId] = useState<string>("")
  const [selectedSessionType, setSelectedSessionType] = useState<SessionType | null>(null)
  const [showMentorModal, setShowMentorModal] = useState(false)
  const [showBookedSessionsModal, setShowBookedSessionsModal] = useState(false)
  const [currentMonth, setCurrentMonth] = useState(dayjs())
  const [timeSlots, setTimeSlots] = useState<TimeSlot[]>([])
  const [timeSlotsLoading, setTimeSlotsLoading] = useState(false)
  const sessionTypes: SessionType[] = ["Virtual", "OneOnOne", "Onsite"]
  const [notify, setNotify] = useState<NotificationProps | null>(null)
  const [bookedSessions, setBookedSessions] = useState<BookedSession[]>([])
  const { notification } = App.useApp()
  const [userTimezone, setUserTimezone] = useState<string>("")

  useEffect(() => {
    // Get user's timezone
    const timezone = Intl.DateTimeFormat().resolvedOptions().timeZone
    setUserTimezone(timezone)
  }, [])

  useEffect(() => {
    if (!selectedMentor || !selectedDate || !userTimezone) {
      setTimeSlots([])
      setSelectedTime("")
      setSelectedTimeSlotId("")
      return
    }

    const fetchTimeSlots = async () => {
      setTimeSlotsLoading(true)
      try {
        // Convert selected local date to UTC to query the correct UTC date range
        const localDateStart = selectedDate.startOf("day")
        const localDateEnd = selectedDate.endOf("day")

        // Convert to UTC to get the range of UTC dates we need to query
        const utcDateStart = localDateStart.utc()
        const utcDateEnd = localDateEnd.utc()

        // We might need to query multiple UTC dates if the local date spans across UTC dates
        const utcDatesToQuery = []
        let currentUtcDate = utcDateStart.startOf("day")

        while (currentUtcDate.isSameOrBefore(utcDateEnd, "day")) {
          utcDatesToQuery.push(currentUtcDate.format("YYYY-MM-DD"))
          currentUtcDate = currentUtcDate.add(1, "day")
        }

        // Fetch time slots for all relevant UTC dates
        const allTimeSlots = []
        for (const utcDate of utcDatesToQuery) {
          try {
            const response = await getAvailableTimeSlots(selectedMentor.id, {
              date: utcDate,
            })

            // Add the UTC date to each slot for conversion
            const slotsWithDate = response.map((slot) => ({
              ...slot,
              utcDate: utcDate,
            }))

            allTimeSlots.push(...slotsWithDate)
          } catch (error) {
            console.error(`Failed to fetch slots for ${utcDate}:`, error)
          }
        }

        // Convert UTC time slots to local time and filter for the selected local date
        const localTimeSlots = allTimeSlots
          .map((slot) => {
            const { localDate, localStartTime, localEndTime } = convertUTCDateTimeToLocal(
              slot.utcDate,
              slot.startTime,
              slot.endTime,
              userTimezone,
            )

            return {
              ...slot,
              localDate,
              startTime: localStartTime,
              endTime: localEndTime,
              originalDate: slot.utcDate,
              originalStartTime: slot.startTime,
              originalEndTime: slot.endTime,
            }
          })
          .filter((slot) => {
            // Only show slots that fall on the selected local date
            return slot.localDate === selectedDate.format("YYYY-MM-DD")
          })

        setTimeSlots(localTimeSlots)
      } catch (error) {
        setNotify({
          type: "error",
          message: "Error",
          description: "Failed to load time slots. Please try again.",
        })
        setTimeSlots([])
      } finally {
        setTimeSlotsLoading(false)
      }
    }

    fetchTimeSlots()
  }, [selectedMentor, selectedDate, userTimezone])

  const handleDateSelect = (date: Dayjs) => {
    setSelectedDate(date)
    setSelectedTime("")
    setSelectedTimeSlotId("")
  }

  const handleMonthChange = (month: Dayjs) => {
    setCurrentMonth(month)
  }

  const handleTimeSelect = (time: string, id: string) => {
    setSelectedTime(time)
    setSelectedTimeSlotId(id)
  }

  const handleSessionTypeSelect = (type: SessionType) => {
    setSelectedSessionType(type)
  }

  const handleMentorSelect = (mentor: Mentor) => {
    setSelectedMentor(mentor)
    setShowMentorModal(false)
    setSelectedTime("")
    setSelectedTimeSlotId("")
  }

  useEffect(() => {
    if (notify) {
      notification[notify.type]({
        message: notify.message,
        description: notify.description,
        placement: "topRight",
      })
      setNotify(null)
    }
  }, [notify, notification])

  const handleConfirmBooking = async () => {
    if (!selectedDate || !selectedTime || !selectedSessionType || !selectedMentor || !selectedTimeSlotId) return

    try {
      const bookingRequest = {
        timeSlotId: selectedTimeSlotId,
        sessionType: selectedSessionType,
      }

      const response = await requestBooking(bookingRequest)

      // Convert the response times to local time for display
      const { localDate, localStartTime, localEndTime } = convertUTCDateTimeToLocal(
        response.day,
        response.startTime,
        response.endTime,
        userTimezone,
      )

      const newSession: BookedSession = {
        id: response.sessionId,
        mentor: selectedMentor,
        date: localDate,
        startTime: localStartTime,
        endTime: localEndTime,
        type: selectedSessionType,
        status: response.bookingStatus,
        originalDate: response.day,
        originalStartTime: response.startTime,
        originalEndTime: response.endTime,
      }

      setBookedSessions((prev) => [newSession, ...prev])

      setSelectedDate(null)
      setSelectedTime("")
      setSelectedTimeSlotId("")
      setSelectedSessionType(null)
      setSelectedMentor(null)

      setNotify({
        type: "success",
        message: "Success",
        description: "Book successfully! Please wait mentor to accept your booking.",
      })
    } catch (error: any) {
      setNotify({
        type: "error",
        message: "Booking Failed",
        description: error.response.data.error || "An error occurred while booking the session. Please try again.",
      })
    }
  }

  const handleCancelSession = (sessionId: string) => {
    setBookedSessions((prev) =>
      prev.map((session) => (session.id === sessionId ? { ...session, status: "Cancelled" as const } : session)),
    )
  }

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
            })
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
  )
}
