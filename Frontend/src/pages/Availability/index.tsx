import React, { useState, useEffect, useContext } from "react";
import { Button, Card, message, Spin, App } from "antd";
import { SaveOutlined } from "@ant-design/icons";
import dayjs, { Dayjs } from "dayjs";
import weekday from 'dayjs/plugin/weekday';
import isoWeek from 'dayjs/plugin/isoWeek';
import isSameOrBefore from 'dayjs/plugin/isSameOrBefore';
import isSameOrAfter from 'dayjs/plugin/isSameOrAfter';
import { WeekNavigation } from "./components/WeekNavigation";
import { ScheduleSettings } from "./components/ScheduleSettings";
import { BulkActions } from "./components/BulkActions";
import { WeekCalendar } from "./components/WeekCalendar";
import { TimeBlocks } from "./components/TimeBlocks";
import type { DayAvailability, TimeBlock, WeekDay } from "./types";
import { availabilityService } from "../../services/availability/availabilityService";
import { AuthContext } from "../../contexts/AuthContext";
import { convertApiScheduleToAvailability, convertAvailabilityToApiFormat, generateTimeSlotsForWeek, generateTimeSlotsForDay } from "./utils";

dayjs.extend(weekday);
dayjs.extend(isoWeek);
dayjs.extend(isSameOrBefore);
dayjs.extend(isSameOrAfter);

export default function AvailabilityManager() {
  const { user } = useContext(AuthContext);
  const { notification } = App.useApp();
  const [startTime, setStartTime] = useState("09:00");
  const [endTime, setEndTime] = useState("17:00");
  const [sessionDuration, setSessionDuration] = useState(60);
  const [bufferTime, setBufferTime] = useState(15);
  const [selectedDate, setSelectedDate] = useState<Dayjs>(dayjs());
  const [currentWeekStart, setCurrentWeekStart] = useState<Dayjs>(
    dayjs().startOf('week').weekday(0)
  );
  const [availability, setAvailability] = useState<DayAvailability>({});
  const [isSaving, setIsSaving] = useState(false);
  const [saveError, setSaveError] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [isLocked, setIsLocked] = useState(false);
  const [currentSlots, setCurrentSlots] = useState<TimeBlock[]>([]);
  const [notify, setNotify] = useState<{ type: "success" | "error" | "info" | "warning"; message: string; description: string; } | null>(null);

  const loadScheduleSettings = async (weekStart?: Dayjs) => {
    if (!user?.id) return;

    try {
      setIsLoading(true);
      const weekStartDate = (weekStart || currentWeekStart).format('YYYY-MM-DD');
      const weekEndDate = (weekStart || currentWeekStart).add(6, 'days').format('YYYY-MM-DD');

      const settings = await availabilityService.getScheduleSettings(user.id, {
        weekStartDate,
        weekEndDate
      });

      setStartTime(settings.startTime);
      setEndTime(settings.endTime);
      setSessionDuration(settings.sessionDuration);
      setBufferTime(settings.bufferTime);
      setIsLocked(settings.isLocked);

      const availabilityData = convertApiScheduleToAvailability(settings);
      setAvailability(availabilityData);

    } catch (error) {
      console.error('Failed to load schedule settings:', error);
      message.error('Failed to load availability settings');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadScheduleSettings();
  }, [user?.id, currentWeekStart]);

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

  const hasBookedSessions = isLocked;

  const getCurrentDateSlots = React.useCallback((): TimeBlock[] => {
    const dateKey = selectedDate.format("YYYY-MM-DD");
    return availability[dateKey] || [];
  }, [selectedDate, availability]);

  useEffect(() => {
    setCurrentSlots(getCurrentDateSlots());
  }, [getCurrentDateSlots]);

  const toggleSlotAvailability = (slotId: string) => {
    const dateKey = selectedDate.format("YYYY-MM-DD");
    const targetSlot = currentSlots.find(slot => slot.id === slotId);

    if (!targetSlot || targetSlot.booked || targetSlot.isPast) {
      if (targetSlot?.isPast) {
        message.warning("Cannot modify time slots in the past");
      }
      return;
    }

    const updatedSlots = currentSlots.map(slot =>
      slot.id === slotId
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

  const navigateWeek = (direction: "prev" | "next") => {
    const newWeekStart = direction === "prev"
      ? currentWeekStart.subtract(1, "week")
      : currentWeekStart.add(1, "week");
    setCurrentWeekStart(newWeekStart);

    // Update selected date to be within the new week
    // If current selected date is not in the new week, move it to the same day of week in new week
    const currentDayOfWeek = selectedDate.day(); // 0 = Sunday, 1 = Monday, etc.
    let newSelectedDate = newWeekStart.add(currentDayOfWeek, "day");

    // Check if the new selected date has time slots
    const newSelectedDateKey = newSelectedDate.format("YYYY-MM-DD");
    const newSelectedDateSlots = availability[newSelectedDateKey] || [];

    // If the new date doesn't have time slots, find the first day in the week that does, but not for the last day of the week
    if (newSelectedDateSlots.length === 0 && currentDayOfWeek !== 6) {
      let foundDayInCurrentWeek = false;

      for (let i = 0; i < 7; i++) {
        // Only check days within the current week (don't go to next week)
        const candidateDate = newWeekStart.add(i, "day");
        const candidateDateKey = candidateDate.format("YYYY-MM-DD");
        const candidateSlots = availability[candidateDateKey] || [];

        if (candidateSlots.length > 0) {
          newSelectedDate = candidateDate;
          foundDayInCurrentWeek = true;
          break;
        }
      }

      // If no day in the current week has slots, just stay on the original day
      if (!foundDayInCurrentWeek) {
        newSelectedDate = newWeekStart.add(currentDayOfWeek, "day");
      }
    }

    setSelectedDate(newSelectedDate);
  };

  const goToCurrentWeek = () => {
    const newWeekStart = dayjs().startOf("week").weekday(0);
    setCurrentWeekStart(newWeekStart);
    setSelectedDate(dayjs());
  };

  const getWeekDays = (): WeekDay[] => {
    const days: WeekDay[] = [];
    for (let i = 0; i < 7; i++) {
      const day = currentWeekStart.add(i, "day");
      const daySlots = availability[day.format("YYYY-MM-DD")] || [];
      const hasTimeSlots = daySlots.length > 0;

      days.push({
        day: day.format("dddd"),
        shortDay: day.format("ddd"),
        date: day.format("MMM D"),
        fullDate: day.format(),
        isToday: day.isSame(dayjs(), "day"),
        active: day.isSame(selectedDate, "day"),
        hasAvailability: daySlots.some(slot => slot.available) || false,
        hasTimeSlots: hasTimeSlots, // New property to track if day has any time slots
      });
    }
    return days;
  };

  const selectAllSlots = () => {
    const dateKey = selectedDate.format("YYYY-MM-DD");
    const updatedSlots = currentSlots.map(slot =>
      slot.booked || slot.isPast ? slot : { ...slot, available: true }
    );

    setAvailability(prev => ({
      ...prev,
      [dateKey]: updatedSlots,
    }));

    message.success("All available time slots enabled for this date");
  };

  const clearAllSlots = () => {
    const dateKey = selectedDate.format("YYYY-MM-DD");
    const updatedSlots = currentSlots.map(slot =>
      slot.booked || slot.isPast ? slot : { ...slot, available: false }
    );

    setAvailability(prev => ({
      ...prev,
      [dateKey]: updatedSlots,
    }));

    message.success("All available time slots disabled for this date");
  };

  const copyScheduleToAllDays = () => {
    const sourceSlots = getCurrentDateSlots();
    const weekDays = getWeekDays();
    const updates: DayAvailability = {};

    const availabilityPattern = new Map<string, boolean>();
    sourceSlots.forEach(slot => {
      const key = `${slot.startTime}-${slot.endTime}`;
      availabilityPattern.set(key, slot.available);
    });

    weekDays.forEach(day => {
      const dateKey = dayjs(day.fullDate).format("YYYY-MM-DD");
      const dayDate = dayjs(day.fullDate);

      if (dateKey === selectedDate.format("YYYY-MM-DD")) {
        return;
      }

      if (dayDate.isBefore(dayjs(), 'day')) {
        return;
      }

      const existingSlots = availability[dateKey] || [];

      const existingSlotsMap = new Map();
      existingSlots.forEach(slot => {
        const key = `${slot.startTime}-${slot.endTime}`;
        existingSlotsMap.set(key, slot);
      });

      const targetCompleteSlots = generateTimeSlotsForDay(
        dateKey,
        startTime,
        endTime,
        sessionDuration,
        bufferTime,
        existingSlots.filter(slot => slot.booked)
      );

      // Apply the availability pattern from the source day
      const newSlots: TimeBlock[] = targetCompleteSlots.map(slot => {
        const key = `${slot.startTime}-${slot.endTime}`;
        const existingSlot = existingSlotsMap.get(key);

        // Get availability pattern from source (default to false if not in pattern)
        const shouldBeAvailable = availabilityPattern.get(key) || false;

        if (existingSlot) {
          // If this slot already exists, preserve its booked status but apply the availability pattern
          return {
            ...existingSlot,
            // Only copy available status if the slot is not booked
            available: existingSlot.booked ? existingSlot.available : shouldBeAvailable
          };
        } else {
          // For new slots, apply the availability pattern
          return {
            ...slot,
            available: shouldBeAvailable
          };
        }
      });

      updates[dateKey] = newSlots;
    });

    setAvailability(prev => ({ ...prev, ...updates }));
    message.success("Schedule copied to future days in current week (preserving existing bookings)");
  };

  const saveChanges = async () => {
    if (!user?.id) {
      message.error('User not authenticated');
      return;
    }
    setIsSaving(true);
    setSaveError(false);

    try {
      const weekStartDate = currentWeekStart.format('YYYY-MM-DD');
      const weekEndDate = currentWeekStart.add(6, 'days').format('YYYY-MM-DD');
      const saveRequest = convertAvailabilityToApiFormat(availability, {
        weekStartDate,
        weekEndDate,
        startTime,
        endTime,
        sessionDuration,
        bufferTime
      });

      const response = await availabilityService.saveScheduleSettings(user.id, saveRequest);

      if (response.success) {
        setNotify({
          type: "success",
          message: `${response.message}`,
          description: "",
        });
        await loadScheduleSettings(currentWeekStart);
      } else {
        setSaveError(true);
        message.error('Failed to save availability settings');
      }
    } catch (error) {
      setSaveError(true);
      setNotify({
        type: "error",
        message: "Error",
        description: "An error occurred while saving your availability.",
      });
      console.error("Save error:", error);
    } finally {
      setIsSaving(false);
    }
  };

  const regenerateTimeSlots = React.useCallback((overrides?: {
    startTime?: string;
    endTime?: string;
    sessionDuration?: number;
    bufferTime?: number;
  }) => {
    try {
      const weekStartDate = currentWeekStart.format('YYYY-MM-DD');
      const effectiveStartTime = overrides?.startTime ?? startTime;
      const effectiveEndTime = overrides?.endTime ?? endTime;
      const effectiveSessionDuration = overrides?.sessionDuration ?? sessionDuration;
      const effectiveBufferTime = overrides?.bufferTime ?? bufferTime;

      setAvailability(currentAvailability => {
        const freshAvailability = generateTimeSlotsForWeek(
          weekStartDate,
          effectiveStartTime,
          effectiveEndTime,
          effectiveSessionDuration,
          effectiveBufferTime,
          currentAvailability
        );

        return freshAvailability;
      });

      message.info('Time slots updated. Please reselect your available slots.');

    } catch (error) {
      console.error('Failed to regenerate time slots:', error);
      message.error('Failed to update time slots');
    }
  }, [currentWeekStart, startTime, endTime, sessionDuration, bufferTime]);
  const updateStartTime = (time: string) => {
    setStartTime(time);
    regenerateTimeSlots({ startTime: time });
  };

  const updateEndTime = (time: string) => {
    setEndTime(time);
    regenerateTimeSlots({ endTime: time });
  };

  const updateSessionDuration = (duration: number) => {
    setSessionDuration(duration);
    regenerateTimeSlots({ sessionDuration: duration });
  };

  const updateBufferTime = (buffer: number) => {
    setBufferTime(buffer);
    regenerateTimeSlots({ bufferTime: buffer });
  };

  const weekDays = getWeekDays();
  const previewData = weekDays.map(day => {
    const dateKey = dayjs(day.fullDate).format("YYYY-MM-DD");
    const daySlots = availability[dateKey] || [];
    const futureSlots = daySlots.filter(slot => !slot.isPast);
    const availableSlots = futureSlots.filter(slot => slot.available && !slot.booked);
    const bookedSlots = futureSlots.filter(slot => slot.booked);

    return {
      day: day.shortDay,
      date: day.date,
      fullDate: day.fullDate,
      availableSlots,
      bookedSlots,
      allVisibleSlots: [...availableSlots, ...bookedSlots].sort((a, b) => a.startTime.localeCompare(b.startTime))
    };
  });

  return (
    <div className="bg-gray-800 rounded-lg overflow-hidden shadow-lg p-6">
      {/* Header */}
      <div className="flex flex-wrap justify-between items-center mb-6 gap-4">
        <h1 className="text-2xl font-semibold">Manage Your Availability</h1>
        <Button
          type="primary"
          icon={<SaveOutlined />}
          onClick={saveChanges}
          loading={isSaving}
          disabled={isLoading}
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

      {/* Loading State */}
      {isLoading ? (
        <div className="flex justify-center items-center min-h-[400px]">
          <Spin size="large" />
        </div>
      ) : (

        <div className="grid grid-cols-1 lg:grid-cols-12 gap-4">
          {/* Left Sidebar */}
          <div className="flex flex-col lg:col-span-3 gap-4">

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
          <div className="flex flex-col lg:col-span-9 gap-4">
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
                    timeBlocks={currentSlots}
                    onToggleBlock={toggleSlotAvailability}
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
                        {/* TODO: Clarify with team */}
                        <div className="space-y-1 max-h-48 overflow-y-auto">
                          {day.allVisibleSlots.length > 0 ? (
                            day.allVisibleSlots.map((slot, slotIndex) => (
                              <div
                                key={slotIndex}
                                className={`text-white text-xs px-2 py-1 rounded text-center truncate ${slot.booked
                                  ? 'bg-slate-500'
                                  : 'bg-orange-500'
                                  }`}
                                title={`${slot.time}${slot.booked ? ' (Booked)' : ' (Available)'}`}
                              >
                                {slot.time}
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
      )}
    </div>
  );
}