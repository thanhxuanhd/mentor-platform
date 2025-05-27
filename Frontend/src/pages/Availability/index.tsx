"use client"

import { useState } from "react"
import { Select, Button, Card, DatePicker, message } from "antd"
import { LeftOutlined, RightOutlined, CalendarOutlined } from "@ant-design/icons"
import dayjs, { type Dayjs } from "dayjs"

const { Option } = Select

interface TimeSlot {
  time: string
  startTime: string
  endTime: string
  available: boolean
}

interface DayAvailability {
  [date: string]: TimeSlot[]
}

export default function AvailabilityManager() {
  const [startTime, setStartTime] = useState("09:00")
  const [endTime, setEndTime] = useState("17:00")
  const [sessionDuration, setSessionDuration] = useState("60 minutes")
  const [bufferTime, setBufferTime] = useState("15 minutes")
  const [selectedDate, setSelectedDate] = useState<Dayjs>(dayjs())
  const [currentWeekStart, setCurrentWeekStart] = useState<Dayjs>(dayjs().startOf("week"))

  // Store availability for multiple dates
  const [availability, setAvailability] = useState<DayAvailability>({
    [dayjs().format("YYYY-MM-DD")]: [
      { time: "09:00 - 10:00", startTime: "09:00", endTime: "10:00", available: false },
      { time: "10:15 - 11:15", startTime: "10:15", endTime: "11:15", available: false },
      { time: "11:30 - 12:30", startTime: "11:30", endTime: "12:30", available: true },
      { time: "12:45 - 13:45", startTime: "12:45", endTime: "13:45", available: true },
      { time: "14:00 - 15:00", startTime: "14:00", endTime: "15:00", available: false },
      { time: "15:15 - 16:15", startTime: "15:15", endTime: "16:15", available: false },
    ],
  })

  // Generate time slots based on work hours and session settings
  const generateTimeSlots = (): TimeSlot[] => {
    const slots: TimeSlot[] = []
    const start = dayjs(`2024-01-01 ${startTime}`)
    const end = dayjs(`2024-01-01 ${endTime}`)
    const duration = Number.parseInt(sessionDuration.split(" ")[0])
    const buffer = Number.parseInt(bufferTime.split(" ")[0])

    let current = start
    while (current.add(duration, "minute").isBefore(end) || current.add(duration, "minute").isSame(end)) {
      const slotEnd = current.add(duration, "minute")
      slots.push({
        time: `${current.format("HH:mm")} - ${slotEnd.format("HH:mm")}`,
        startTime: current.format("HH:mm"),
        endTime: slotEnd.format("HH:mm"),
        available: false,
      })
      current = slotEnd.add(buffer, "minute")
    }

    return slots
  }

  // Get current date's time slots
  const getCurrentDateSlots = (): TimeSlot[] => {
    const dateKey = selectedDate.format("YYYY-MM-DD")
    return availability[dateKey] || generateTimeSlots()
  }

  // Toggle availability for a specific time slot
  const toggleSlotAvailability = (slotIndex: number) => {
    const dateKey = selectedDate.format("YYYY-MM-DD")
    const currentSlots = getCurrentDateSlots()
    const updatedSlots = currentSlots.map((slot, index) =>
      index === slotIndex ? { ...slot, available: !slot.available } : slot,
    )

    setAvailability((prev) => ({
      ...prev,
      [dateKey]: updatedSlots,
    }))

    message.success(`Time slot ${updatedSlots[slotIndex].available ? "enabled" : "disabled"}`)
  }

  // Navigate week
  const navigateWeek = (direction: "prev" | "next") => {
    const newWeekStart = direction === "prev" ? currentWeekStart.subtract(1, "week") : currentWeekStart.add(1, "week")
    setCurrentWeekStart(newWeekStart)
  }

  // Go to current week
  const goToCurrentWeek = () => {
    setCurrentWeekStart(dayjs().startOf("week"))
    setSelectedDate(dayjs())
  }

  // Generate week days
  const getWeekDays = () => {
    const days = []
    for (let i = 0; i < 7; i++) {
      const day = currentWeekStart.add(i, "day")
      days.push({
        day: day.format("ddd"),
        date: day.format("MMM D"),
        fullDate: day,
        active: day.isSame(selectedDate, "day"),
        hasAvailability: availability[day.format("YYYY-MM-DD")]?.some((slot) => slot.available) || false,
      })
    }
    return days
  }

  // Select all slots for current date
  const selectAllSlots = () => {
    const dateKey = selectedDate.format("YYYY-MM-DD")
    const currentSlots = getCurrentDateSlots()
    const updatedSlots = currentSlots.map((slot) => ({ ...slot, available: true }))

    setAvailability((prev) => ({
      ...prev,
      [dateKey]: updatedSlots,
    }))

    message.success("All time slots enabled for this date")
  }

  // Clear all slots for current date
  const clearAllSlots = () => {
    const dateKey = selectedDate.format("YYYY-MM-DD")
    const currentSlots = getCurrentDateSlots()
    const updatedSlots = currentSlots.map((slot) => ({ ...slot, available: false }))

    setAvailability((prev) => ({
      ...prev,
      [dateKey]: updatedSlots,
    }))

    message.success("All time slots disabled for this date")
  }

  // Copy schedule to all days in current week
  const copyScheduleToAllDays = () => {
    const sourceSlots = getCurrentDateSlots()
    const weekDays = getWeekDays()
    const updates: DayAvailability = {}

    weekDays.forEach((day) => {
      updates[day.fullDate.format("YYYY-MM-DD")] = sourceSlots.map((slot) => ({ ...slot }))
    })

    setAvailability((prev) => ({ ...prev, ...updates }))
    message.success("Schedule copied to all days in current week")
  }

  // Save changes
  const saveChanges = () => {
    // Here you would typically save to backend
    message.success("Availability changes saved successfully!")
  }

  const weekDays = getWeekDays()
  const currentSlots = getCurrentDateSlots()

  return (
    <div className="min-h-screen bg-slate-800 text-white p-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-semibold">Manage Your Availability</h1>
        <Button
          type="primary"
          onClick={saveChanges}
          className="bg-orange-500 hover:bg-orange-600 border-orange-500 px-6 py-2 h-auto"
        >
          Save Changes
        </Button>
      </div>

      <div className="grid grid-cols-12 gap-6">
        {/* Left Sidebar */}
        <div className="col-span-3 space-y-6">
          {/* Date Picker */}
          <Card className="bg-slate-700 border-slate-600">
            <div className="text-white">
              <h3 className="text-lg font-medium mb-4">Select Date</h3>
              <DatePicker
                value={selectedDate}
                onChange={(date) => date && setSelectedDate(date)}
                className="w-full"
                format="YYYY-MM-DD"
                suffixIcon={<CalendarOutlined className="text-slate-400" />}
              />
              <p className="text-xs text-slate-400 mt-2">Selected: {selectedDate.format("dddd, MMMM D, YYYY")}</p>
            </div>
          </Card>

          {/* Work Hours */}
          <Card className="bg-slate-700 border-slate-600">
            <div className="text-white">
              <h3 className="text-lg font-medium mb-4">Work hours</h3>
              <div className="grid grid-cols-2 gap-4 mb-3">
                <div>
                  <label className="block text-sm text-slate-300 mb-2">Start time</label>
                  <Select value={startTime} onChange={setStartTime} className="w-full">
                    <Option value="08:00">08:00</Option>
                    <Option value="09:00">09:00</Option>
                    <Option value="10:00">10:00</Option>
                  </Select>
                </div>
                <div>
                  <label className="block text-sm text-slate-300 mb-2">End time</label>
                  <Select value={endTime} onChange={setEndTime} className="w-full">
                    <Option value="16:00">16:00</Option>
                    <Option value="17:00">17:00</Option>
                    <Option value="18:00">18:00</Option>
                  </Select>
                </div>
              </div>
              <p className="text-xs text-slate-400">Changing work hours will regenerate your time blocks.</p>
            </div>
          </Card>

          {/* Session Settings */}
          <Card className="bg-slate-700 border-slate-600">
            <div className="text-white">
              <h3 className="text-lg font-medium mb-4">Session settings</h3>
              <div className="space-y-4">
                <div>
                  <label className="block text-sm text-slate-300 mb-2">Session duration</label>
                  <Select value={sessionDuration} onChange={setSessionDuration} className="w-full">
                    <Option value="30 minutes">30 minutes</Option>
                    <Option value="60 minutes">60 minutes</Option>
                    <Option value="90 minutes">90 minutes</Option>
                  </Select>
                </div>
                <div>
                  <label className="block text-sm text-slate-300 mb-2">Buffer time between sessions</label>
                  <Select value={bufferTime} onChange={setBufferTime} className="w-full">
                    <Option value="5 minutes">5 minutes</Option>
                    <Option value="15 minutes">15 minutes</Option>
                    <Option value="30 minutes">30 minutes</Option>
                  </Select>
                </div>
              </div>
            </div>
          </Card>

          {/* Calendar Navigation */}
          <Card className="bg-slate-700 border-slate-600">
            <div className="text-white">
              <h3 className="text-lg font-medium mb-4">Calendar Navigation</h3>
              <div className="flex items-center justify-between mb-4">
                <Button
                  icon={<LeftOutlined />}
                  onClick={() => navigateWeek("prev")}
                  className="bg-transparent border-slate-500 text-white hover:bg-slate-600"
                />
                <span className="text-sm">
                  {currentWeekStart.format("MMM D")} - {currentWeekStart.add(6, "day").format("MMM D")}
                </span>
                <Button
                  icon={<RightOutlined />}
                  onClick={() => navigateWeek("next")}
                  className="bg-transparent border-slate-500 text-white hover:bg-slate-600"
                />
              </div>
              <Button
                onClick={goToCurrentWeek}
                className="w-full bg-slate-600 border-slate-500 text-white hover:bg-slate-500"
              >
                Go to current week
              </Button>
            </div>
          </Card>

          {/* Bulk Actions */}
          <Card className="bg-slate-700 border-slate-600">
            <div className="text-white">
              <h3 className="text-lg font-medium mb-4">Bulk actions</h3>
              <div className="space-y-3">
                <Button
                  onClick={selectAllSlots}
                  className="w-full bg-blue-600 border-blue-600 text-white hover:bg-blue-700"
                >
                  Select all slots for {selectedDate.format("ddd MMM D")}
                </Button>
                <Button
                  onClick={clearAllSlots}
                  className="w-full bg-slate-600 border-slate-500 text-white hover:bg-slate-500"
                >
                  Clear all slots for {selectedDate.format("ddd MMM D")}
                </Button>
                <Button
                  onClick={copyScheduleToAllDays}
                  className="w-full bg-green-600 border-green-600 text-white hover:bg-green-700"
                >
                  Copy schedule to all days
                </Button>
              </div>
            </div>
          </Card>
        </div>

        {/* Main Content */}
        <div className="col-span-9 space-y-6">
          {/* Week Header */}
          <div className="grid grid-cols-7 gap-2">
            {weekDays.map((day, index) => (
              <button
                key={index}
                onClick={() => setSelectedDate(day.fullDate)}
                className={`p-4 rounded-lg text-center transition-colors relative ${day.active ? "bg-orange-500 text-white" : "bg-slate-700 text-slate-300 hover:bg-slate-600"
                  }`}
              >
                <div className="font-medium">{day.day}</div>
                <div className="text-sm">{day.date}</div>
                {day.hasAvailability && (
                  <div className="absolute top-1 right-1 w-2 h-2 bg-green-400 rounded-full"></div>
                )}
              </button>
            ))}
          </div>

          {/* Time Slots */}
          <Card className="bg-slate-700 border-slate-600">
            <div className="text-white">
              <h3 className="text-lg font-medium mb-4">
                Set your availability for {selectedDate.format("dddd, MMMM D")}
              </h3>
              <div className="grid grid-cols-5 gap-3">
                {currentSlots.map((slot, index) => (
                  <button
                    key={index}
                    onClick={() => toggleSlotAvailability(index)}
                    className={`p-4 rounded-lg text-center transition-colors ${slot.available
                      ? "bg-orange-500 hover:bg-orange-600 text-white"
                      : "bg-slate-600 hover:bg-slate-500 text-slate-300"
                      }`}
                  >
                    <div className="text-sm font-medium">{slot.time}</div>
                    <div className="text-xs mt-1">{slot.available ? "Available" : "Unavailable"}</div>
                  </button>
                ))}
              </div>
              {currentSlots.length === 0 && (
                <div className="text-center text-slate-400 py-8">No time slots available for the current settings</div>
              )}
            </div>
          </Card>

          {/* Availability Preview */}
          <Card className="bg-slate-700 border-slate-600">
            <div className="text-white">
              <h3 className="text-lg font-medium mb-2">Availability preview</h3>
              <p className="text-slate-400 text-sm mb-4">This is how your availability will appear to learners:</p>

              <div className="grid grid-cols-7 gap-2 mb-4">
                {weekDays.map((day, index) => {
                  const daySlots = availability[day.fullDate.format("YYYY-MM-DD")] || []
                  const availableSlots = daySlots.filter((slot) => slot.available)

                  return (
                    <div key={index} className="bg-slate-600 rounded-lg p-3">
                      <div className="text-center mb-2">
                        <div className="font-medium text-sm">{day.day}</div>
                        <div className="text-xs text-slate-400">{day.date}</div>
                      </div>
                      <div className="space-y-1">
                        {availableSlots.map((slot, slotIndex) => (
                          <div
                            key={slotIndex}
                            className="bg-orange-500 text-white text-xs px-2 py-1 rounded text-center"
                          >
                            {slot.startTime}
                          </div>
                        ))}
                        {availableSlots.length === 0 && (
                          <div className="text-xs text-slate-500 text-center">No availability</div>
                        )}
                      </div>
                    </div>
                  )
                })}
              </div>

              <div className="flex items-center justify-between text-sm text-slate-400">
                <span>
                  Working hours: {startTime} - {endTime} • Session duration: {sessionDuration} • Buffer: {bufferTime}
                </span>
                <div className="flex items-center space-x-4">
                  <div className="flex items-center space-x-2">
                    <div className="w-3 h-3 bg-orange-500 rounded"></div>
                    <span>Available</span>
                  </div>
                  <div className="flex items-center space-x-2">
                    <div className="w-3 h-3 bg-blue-500 rounded"></div>
                    <span>Booked</span>
                  </div>
                </div>
              </div>
            </div>
          </Card>
        </div>
      </div>
    </div>
  )
}