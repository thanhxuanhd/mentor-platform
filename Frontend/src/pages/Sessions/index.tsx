import React, { useState } from "react"
import { Button, Avatar } from "antd"
import {
  PlusOutlined,
  MessageOutlined,
  CalendarOutlined,
} from "@ant-design/icons"
import type { Dayjs } from "dayjs"
import dayjs from "dayjs"
import { MentorSelectionModal, type Mentor } from "./components/MentorSelectionModal"
import { CalendarComponent } from "./components/Calendar"
import { mentors, sessionTypes, timeSlots } from "./MockData"

export default function MentorshipBooking() {
  const [selectedMentor, setSelectedMentor] = useState<Mentor>(mentors[0])
  const [selectedDate, setSelectedDate] = useState<Dayjs | null>(null)
  const [selectedTime, setSelectedTime] = useState<string>("")
  const [selectedSessionType, setSelectedSessionType] = useState<string>("")
  const [showMentorModal, setShowMentorModal] = useState(false)
  const [currentMonth, setCurrentMonth] = useState(dayjs())

  const handleDateSelect = (date: Dayjs) => {
    setSelectedDate(date)
  }

  const handleMonthChange = (month: Dayjs) => {
    setCurrentMonth(month)
  }

  const handleTimeSelect = (time: string) => {
    setSelectedTime(time)
  }

  const handleSessionTypeSelect = (type: string) => {
    setSelectedSessionType(type)
  }

  const handleMentorSelect = (mentor: Mentor) => {
    setSelectedMentor(mentor)
    setShowMentorModal(false)
  }

  const handleConfirmBooking = () => {
    console.log("Booking confirmed:", {
      mentor: selectedMentor,
      date: selectedDate?.format("YYYY-MM-DD"),
      time: selectedTime,
      sessionType: selectedSessionType,
    })
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
        <div className="flex items-center justify-between mb-8 bg-slate-700 rounded-lg p-6">
          <div className="flex items-center space-x-4">
            <div className="relative">
              <Avatar size={60} src={selectedMentor.avatar} />
              {selectedMentor.isOnline && (
                <div className="absolute -bottom-1 -right-1 w-4 h-4 bg-green-500 rounded-full border-2 border-slate-700"></div>
              )}
            </div>
            <div>
              <h3 className="text-xl font-semibold">{selectedMentor.name}</h3>
              <p className="text-gray-400">{selectedMentor.expertise}</p>
              <p className="text-green-400 text-sm">{selectedMentor.availability}</p>
            </div>
          </div>
          <div className="flex space-x-2">
            <Button
              type="primary"
              shape="circle"
              icon={<PlusOutlined />}
              className="bg-orange-500 border-orange-500 hover:bg-orange-600"
              onClick={() => setShowMentorModal(true)}
            />
            <Button
              type="primary"
              shape="circle"
              icon={<MessageOutlined />}
              className="bg-orange-500 border-orange-500 hover:bg-orange-600"
            />
            <Button
              type="primary"
              shape="circle"
              icon={<CalendarOutlined />}
              className="bg-orange-500 border-orange-500 hover:bg-orange-600"
            />
          </div>
        </div>

        {/* Calendar */}
        <CalendarComponent
          selectedDate={selectedDate}
          currentMonth={currentMonth}
          onDateSelect={handleDateSelect}
          onMonthChange={handleMonthChange}
        />

        {/* Time Slots */}
        <div className="mt-8">
          <h3 className="text-lg font-medium mb-4 text-center">Select a time slot</h3>
          <div className="grid grid-cols-5 gap-3">
            {timeSlots.map((time) => (
              <Button
                key={time}
                type={selectedTime === time ? "primary" : "default"}
                className={`h-12 ${selectedTime === time
                  ? "bg-orange-500 border-orange-500"
                  : "bg-orange-500 border-orange-500 text-white hover:bg-orange-600"
                  }`}
                onClick={() => handleTimeSelect(time)}
              >
                {time}
              </Button>
            ))}
          </div>
        </div>

        {/* Session Type */}
        <div className="mt-8">
          <h3 className="text-lg font-medium mb-4 text-center">Session type</h3>
          <div className="grid grid-cols-3 gap-4">
            {sessionTypes.map((type) => (
              <div
                key={type.id}
                className={`
                  cursor-pointer transition-all rounded-lg border-2 p-6
                  ${selectedSessionType === type.id
                    ? "border-orange-500 bg-slate-600"
                    : "border-slate-600 bg-slate-700 hover:border-slate-500"
                  }
                `}
                onClick={() => handleSessionTypeSelect(type.id)}
              >
                <div className="text-center">
                  <div className={`mb-3 ${selectedSessionType === type.id ? "text-orange-400" : "text-gray-400"}`}>
                    {React.cloneElement(type.icon, {
                      className: `text-2xl ${selectedSessionType === type.id ? "text-orange-400" : "text-gray-400"}`,
                    })}
                  </div>
                  <h4 className={`font-medium ${selectedSessionType === type.id ? "text-white" : "text-gray-300"}`}>
                    {type.title}
                  </h4>
                </div>
              </div>
            ))}
          </div>
        </div>

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
      </div>
    </div>
  )
}