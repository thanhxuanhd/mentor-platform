"use client"

import React, { useState, useEffect } from "react";
import { Button, Card, DatePicker, message, Spin } from "antd";
import { CalendarOutlined, SaveOutlined } from "@ant-design/icons";
import dayjs, { Dayjs } from "dayjs";
import { v4 as uuidv4 } from 'uuid';
import weekday from 'dayjs/plugin/weekday';
import isoWeek from 'dayjs/plugin/isoWeek';
import isSameOrBefore from 'dayjs/plugin/isSameOrBefore';

// Import components
import { WeekNavigation } from "./components/WeekNavigation";
import { ScheduleSettings } from "./components/ScheduleSettings";
import { BulkActions } from "./components/BulkActions";
import { WeekCalendar } from "./components/WeekCalendar";
import { TimeBlocks } from "./components/TimeBlocks";

// Import types and mock data
import type { DayAvailability, TimeSlot, WeekDay } from "./types";
import { generateInitialAvailability, mockSaveAvailability } from "./mock";

// Configure dayjs plugins
dayjs.extend(weekday);
dayjs.extend(isoWeek);
dayjs.extend(isSameOrBefore);

export default function AvailabilityManager() {
  // Settings state
  const [startTime, setStartTime] = useState("09:00");
  const [endTime, setEndTime] = useState("17:00");
  const [sessionDuration, setSessionDuration] = useState(60);
  const [bufferTime, setBufferTime] = useState(15);
  
  // Calendar state
  const [selectedDate, setSelectedDate] = useState<Dayjs>(dayjs());
  const [currentWeekStart, setCurrentWeekStart] = useState<Dayjs>(
    dayjs().startOf('week').weekday(0) // Start from Sunday
  );
  
  // Availability data
  const [availability, setAvailability] = useState<DayAvailability>(generateInitialAvailability());
  
  // UI state
  const [isSaving, setIsSaving] = useState(false);
  const [saveError, setSaveError] = useState(false);
  
  // Check if any sessions are booked
  const hasBookedSessions = React.useMemo(() => {
    return Object.values(availability).some(slots => 
      slots.some(slot => slot.booked)
    );
  }, [availability]);

  // Generate time slots based on work hours and session settings
  const generateTimeSlots = (): TimeSlot[] => {
    const slots: TimeSlot[] = [];
    const start = dayjs(`2024-01-01 ${startTime}`);
    const end = dayjs(`2024-01-01 ${endTime}`);
    
    let current = start;
    while (current.add(sessionDuration, "minute").isSameOrBefore(end)) {
      const slotEnd = current.add(sessionDuration, "minute");
      slots.push({
        id: uuidv4(),
        time: `${current.format("HH:mm")} - ${slotEnd.format("HH:mm")}`,
        startTime: current.format("HH:mm"),
        endTime: slotEnd.format("HH:mm"),
        available: false,
        booked: false,
      });
      current = slotEnd.add(bufferTime, "minute");
    }

    return slots;
  };

  // Get time slots for the currently selected date
  const getCurrentDateSlots = (): TimeSlot[] => {
    const dateKey = selectedDate.format("YYYY-MM-DD");
    return availability[dateKey] || generateTimeSlots();
  };

  // Current time slots based on selected date
  const [currentSlots, setCurrentSlots] = useState<TimeSlot[]>([]);

  // Update current slots when selected date or availability changes
  useEffect(() => {
    setCurrentSlots(getCurrentDateSlots());
  }, [selectedDate, availability]);

  // Toggle availability for a specific time slot
  const toggleSlotAvailability = (slotId: string) => {
    const dateKey = selectedDate.format("YYYY-MM-DD");
    const updatedSlots = currentSlots.map(slot =>
      slot.id === slotId && !slot.booked
        ? { ...slot, available: !slot.available }
        : slot
    );

    setAvailability(prev => ({
      ...prev,
      [dateKey]: updatedSlots,
    }));

    const updatedSlot = updatedSlots.find(slot => slot.id === slotId);
    message.success(
      `Time slot ${updatedSlot?.time} ${updatedSlot?.available ? 'enabled' : 'disabled'}`
    );
  };
  // Week navigation
  const navigateWeek = (direction: "prev" | "next") => {
    const newWeekStart = direction === "prev" 
      ? currentWeekStart.subtract(1, "week") 
      : currentWeekStart.add(1, "week");
    setCurrentWeekStart(newWeekStart);
    
    // Update selected date to be within the new week
    // If current selected date is not in the new week, move it to the same day of week in new week
    const currentDayOfWeek = selectedDate.day(); // 0 = Sunday, 1 = Monday, etc.
    const newSelectedDate = newWeekStart.add(currentDayOfWeek, "day");
    setSelectedDate(newSelectedDate);
  };

  // Go to current week
  const goToCurrentWeek = () => {
    setCurrentWeekStart(dayjs().startOf("week").weekday(0)); // Start from Sunday
    setSelectedDate(dayjs());
  };

  // Generate week days
  const getWeekDays = (): WeekDay[] => {
    const days: WeekDay[] = [];
    for (let i = 0; i < 7; i++) {
      const day = currentWeekStart.add(i, "day");
      days.push({
        day: day.format("dddd"),
        shortDay: day.format("ddd"),
        date: day.format("MMM D"),
        fullDate: day.format(),
        isToday: day.isSame(dayjs(), "day"),
        active: day.isSame(selectedDate, "day"),
        hasAvailability: availability[day.format("YYYY-MM-DD")]?.some(slot => slot.available) || false,
      });
    }
    return days;
  };

  // Select all slots for current date
  const selectAllSlots = () => {
    const dateKey = selectedDate.format("YYYY-MM-DD");
    const updatedSlots = currentSlots.map(slot => 
      slot.booked ? slot : { ...slot, available: true }
    );

    setAvailability(prev => ({
      ...prev,
      [dateKey]: updatedSlots,
    }));

    message.success("All time slots enabled for this date");
  };

  // Clear all slots for current date
  const clearAllSlots = () => {
    const dateKey = selectedDate.format("YYYY-MM-DD");
    const updatedSlots = currentSlots.map(slot => 
      slot.booked ? slot : { ...slot, available: false }
    );

    setAvailability(prev => ({
      ...prev,
      [dateKey]: updatedSlots,
    }));

    message.success("All time slots disabled for this date");
  };

  // Copy schedule to all days in current week
  const copyScheduleToAllDays = () => {
    const sourceSlots = getCurrentDateSlots();
    const weekDays = getWeekDays();
    const updates: DayAvailability = {};

    weekDays.forEach(day => {
      const dateKey = dayjs(day.fullDate).format("YYYY-MM-DD");
      // If the day already has booked sessions, preserve them
      const existingSlots = availability[dateKey] || [];
      const bookedSlots = existingSlots.filter(slot => slot.booked);
      
      const slotMap = new Map();
      bookedSlots.forEach(slot => {
        slotMap.set(`${slot.startTime}-${slot.endTime}`, slot);
      });
      
      // Create new slots based on source, but preserve booked status
      const newSlots = sourceSlots.map(slot => {
        const key = `${slot.startTime}-${slot.endTime}`;
        const existingSlot = slotMap.get(key);
        
        if (existingSlot && existingSlot.booked) {
          return existingSlot;
        }
        
        return { 
          ...slot, 
          id: uuidv4(),
        };
      });
      
      updates[dateKey] = newSlots;
    });

    setAvailability(prev => ({ ...prev, ...updates }));
    message.success("Schedule copied to all days in current week");
  };

  // Save changes
  const saveChanges = async () => {
    setIsSaving(true);
    setSaveError(false);
    
    try {
      const response = await mockSaveAvailability(availability);
      
      if (response.success) {
        message.success(response.message);
      } else {
        setSaveError(true);
        message.error(response.message);
      }
    } catch (error) {
      setSaveError(true);
      message.error("An error occurred while saving your availability.");
      console.error("Save error:", error);
    } finally {
      setIsSaving(false);
    }
  };

  // Apply settings changes and regenerate time slots
  const applySettingsChanges = () => {
    const updatedSlots = generateTimeSlots();
    
    // Update availability data with new slot structure
    const updatedAvailability: DayAvailability = {};
    Object.keys(availability).forEach(dateKey => {
      updatedAvailability[dateKey] = updatedSlots.map(slot => ({
        ...slot,
        // Try to preserve existing availability selections
        available: availability[dateKey]?.some(
          existing => 
            existing.startTime === slot.startTime && 
            existing.endTime === slot.endTime && 
            existing.available
        ) || false
      }));
    });
    
    setAvailability(updatedAvailability);
    setCurrentSlots(updatedSlots);
  };

  // Update settings with validation
  const updateStartTime = (time: string) => {
    setStartTime(time);
    applySettingsChanges();
  };

  const updateEndTime = (time: string) => {
    setEndTime(time);
    applySettingsChanges();
  };

  const updateSessionDuration = (duration: number) => {
    setSessionDuration(duration);
    applySettingsChanges();
  };

  const updateBufferTime = (buffer: number) => {
    setBufferTime(buffer);
    applySettingsChanges();
  };

  const weekDays = getWeekDays();
  
  // Format data for availability preview
  const previewData = weekDays.map(day => ({
    day: day.shortDay,
    date: day.date,
    fullDate: day.fullDate,
    availableSlots: availability[dayjs(day.fullDate).format("YYYY-MM-DD")]?.filter(slot => slot.available) || []
  }));

  return (
    <div className="min-h-screen bg-slate-800 text-white p-4 md:p-6">
      {/* Header */}
      <div className="flex flex-wrap justify-between items-center mb-6 gap-4">
        <h1 className="text-2xl font-semibold">Manage Your Availability</h1>
        <Button
          type="primary"
          icon={<SaveOutlined />}
          onClick={saveChanges}
          loading={isSaving}
          className={`
            px-6 py-2 h-auto 
            ${saveError 
              ? 'bg-red-500 hover:bg-red-600 border-red-500' 
              : 'bg-orange-500 hover:bg-orange-600 border-orange-500'
            }
          `}
        >
          {saveError ? 'Retry Saving' : 'Save Changes'}
        </Button>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-12 gap-6">
        {/* Left Sidebar */}
        <div className="lg:col-span-3 space-y-6">
          {/* Date Picker */}
          <Card className="bg-slate-700 border-slate-600">
            <div className="text-white">
              <h3 className="text-lg font-medium mb-4">Select Date</h3>              <DatePicker
                value={selectedDate}
                onChange={(date) => {
                  if (date) {
                    setSelectedDate(date);
                    // Update the week to show the week containing the selected date
                    setCurrentWeekStart(date.startOf('week').weekday(0));
                  }
                }}
                className="w-full"
                format="YYYY-MM-DD"
                suffixIcon={<CalendarOutlined className="text-slate-400" />}
              />
              <p className="text-xs text-slate-400 mt-2">
                Selected: {selectedDate.format("dddd, MMMM D, YYYY")}
              </p>
            </div>
          </Card>

          {/* Calendar Navigation */}
          <Card className="bg-slate-700 border-slate-600">
            <div className="text-white">
              <WeekNavigation
                currentWeekStart={currentWeekStart}
                onWeekChange={navigateWeek}
                onTodayClick={goToCurrentWeek}
              />
            </div>
          </Card>

          {/* Schedule Settings */}
          <Card className="bg-slate-700 border-slate-600">
            <div className="text-white">
              <ScheduleSettings
                startTime={startTime}
                endTime={endTime}
                sessionDuration={sessionDuration}
                bufferTime={bufferTime}
                onStartTimeChange={updateStartTime}
                onEndTimeChange={updateEndTime}
                onSessionDurationChange={updateSessionDuration}
                onBufferTimeChange={updateBufferTime}
                hasBookedSessions={hasBookedSessions}
              />
            </div>
          </Card>

          {/* Bulk Actions */}
          <Card className="bg-slate-700 border-slate-600">
            <div className="text-white">
              <BulkActions
                selectedDate={selectedDate}
                onSelectAll={selectAllSlots}
                onClearAll={clearAllSlots}
                onCopyToWeek={copyScheduleToAllDays}
              />
            </div>
          </Card>
        </div>

        {/* Main Content */}
        <div className="lg:col-span-9 space-y-6">
          {/* Week Calendar */}
          <Card className="bg-slate-700 border-slate-600">
            <div className="text-white">
              <WeekCalendar
                weekDays={weekDays}
                onDaySelect={setSelectedDate}
              />
            </div>
          </Card>

          {/* Time Blocks */}
          <Card className="bg-slate-700 border-slate-600">
            <div className="text-white">
              <Spin spinning={isSaving}>
                <TimeBlocks
                  selectedDate={selectedDate}
                  timeSlots={currentSlots}
                  onToggleSlot={toggleSlotAvailability}
                />
              </Spin>
            </div>
          </Card>

          {/* Availability Preview */}
          <Card className="bg-slate-700 border-slate-600">
            <div className="text-white">
              <div className="space-y-4">
                <h3 className="text-lg font-medium mb-2">Availability Preview</h3>
                <p className="text-slate-400 text-sm mb-4">
                  This is how your availability will appear to learners:
                </p>

                <div className="grid grid-cols-3 md:grid-cols-4 lg:grid-cols-7 gap-2 mb-4">
                  {previewData.map((day, index) => (
                    <div key={index} className="bg-slate-600 rounded-lg p-3">
                      <div className="text-center mb-2">
                        <div className="font-medium text-sm">{day.day}</div>
                        <div className="text-xs text-slate-400">{day.date}</div>
                      </div>
                      <div className="space-y-1 max-h-48 overflow-y-auto">
                        {day.availableSlots.length > 0 ? (
                          day.availableSlots.map((slot, slotIndex) => (
                            <div
                              key={slotIndex}
                              className="bg-orange-500 text-white text-xs px-2 py-1 rounded text-center truncate"
                              title={slot.time}
                            >
                              {slot.startTime}
                            </div>
                          ))
                        ) : (
                          <div className="text-xs text-slate-500 text-center">No availability</div>
                        )}
                      </div>
                    </div>
                  ))}
                </div>

                <div className="flex flex-wrap items-center justify-between text-sm text-slate-400">
                  <span className="mb-2 md:mb-0">
                    Working hours: {startTime} - {endTime} • Session: {sessionDuration} min • Buffer: {bufferTime} min
                  </span>
                  <div className="flex flex-wrap gap-4">
                    <div className="flex items-center space-x-2">
                      <div className="w-3 h-3 bg-orange-500 rounded"></div>
                      <span>Available</span>
                    </div>
                    <div className="flex items-center space-x-2">
                      <div className="w-3 h-3 bg-slate-500 rounded"></div>
                      <span>Booked</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </Card>
        </div>
      </div>
    </div>
  );
}