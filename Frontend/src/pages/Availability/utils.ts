import dayjs from 'dayjs';
import utc from 'dayjs/plugin/utc';
import timezone from 'dayjs/plugin/timezone';
import { v4 as uuidv4 } from 'uuid';
import type { TimeBlock, DayAvailability } from './types';
import type { ScheduleSettingsResponse } from '../../services/availability/availabilityService';
import { convertUTCTimeSlotsToLocal, convertLocalTimeSlotsToUTC } from '../../utils/timezoneUtils';

dayjs.extend(utc);
dayjs.extend(timezone);

export const convertApiScheduleToAvailability = (
  apiSettings: ScheduleSettingsResponse, 
  userTimezone: string
): DayAvailability => {
  const availability: DayAvailability = {};
  
  const localTimeSlots = convertUTCTimeSlotsToLocal(apiSettings.availableTimeSlots, userTimezone);
  
  Object.entries(localTimeSlots).forEach(([localDate, slots]) => {
    const timeBlocks: TimeBlock[] = slots.map(slot => ({
      id: slot.id,
      time: `${slot.startTime} - ${slot.endTime}`,
      startTime: slot.startTime,
      endTime: slot.endTime,
      available: slot.isAvailable,
      booked: slot.isBooked,
      originalDate: slot.originalDate,
      originalStartTime: slot.originalStartTime,
      originalEndTime: slot.originalEndTime
    }));
    
    if (timeBlocks.length > 0) {
      availability[localDate] = timeBlocks;
    }
  });
  
  return availability;
};

export const convertAvailabilityToApiFormat = (
  availability: DayAvailability,
  settings: {
    weekStartDate: string;
    weekEndDate: string;
    startTime: string;
    endTime: string;
    sessionDuration: number;
    bufferTime: number;
  },
  userTimezone: string
) => {
  // Filter only available (selected) slots
  const localAvailableSlots: Record<string, Array<{
    id?: string;
    startTime: string;
    endTime: string;
    isAvailable: boolean;
    isBooked: boolean;
    originalDate?: string;
    originalStartTime?: string;
    originalEndTime?: string;
  }>> = {};

  Object.entries(availability).forEach(([localDate, timeBlocks]) => {
    const availableSlots = timeBlocks.filter(slot => 
      slot.available && !slot.booked
    );
    
    if (availableSlots.length > 0) {
      localAvailableSlots[localDate] = availableSlots.map(slot => ({
        id: slot.id,
        startTime: slot.startTime,
        endTime: slot.endTime,
        isAvailable: slot.available,
        isBooked: slot.booked,
        originalDate: slot.originalDate,
        originalStartTime: slot.originalStartTime,
        originalEndTime: slot.originalEndTime
      }));
    }
  });

  const utcTimeSlots = convertLocalTimeSlotsToUTC(localAvailableSlots, userTimezone);

  return {
    weekStartDate: settings.weekStartDate,
    weekEndDate: settings.weekEndDate,
    startTime: settings.startTime,
    endTime: settings.endTime,
    sessionDuration: settings.sessionDuration,
    bufferTime: settings.bufferTime,
    availableTimeSlots: utcTimeSlots
  };
};

export const generateTimeSlotsForDay = (
  date: string, // YYYY-MM-DD format
  startTime: string, // HH:mm format
  endTime: string, // HH:mm format
  sessionDuration: number, // minutes
  bufferTime: number, // minutes
  existingBookedSlots: TimeBlock[] = []
): TimeBlock[] => {
  const slots: TimeBlock[] = [];
  
  // Parse start and end times
  const dayStart = dayjs(`${date} ${startTime}`);
  const dayEnd = dayjs(`${date} ${endTime}`);
  
  // Create a map of existing booked slots for quick lookup
  const bookedSlotsMap = new Map<string, TimeBlock>();
  existingBookedSlots.forEach(slot => {
    if (slot.booked) {
      const key = `${slot.startTime}-${slot.endTime}`;
      bookedSlotsMap.set(key, slot);
    }
  });  

  let currentTime = dayStart;
  
  // Generate slots only if they can completely fit within work hours and are in the future
  while (currentTime.add(sessionDuration, 'minute').isSameOrBefore(dayEnd)) {
    const slotStart = currentTime;
    const slotEnd = currentTime.add(sessionDuration, 'minute');
    
    const startTimeStr = slotStart.format('HH:mm');
    const endTimeStr = slotEnd.format('HH:mm');
    const key = `${startTimeStr}-${endTimeStr}`;
    
    // Check if this slot is in the past or currently happening
    const isPast = slotStart.isSameOrBefore(dayjs());
    
    // Skip generating past time slots entirely
    if (isPast) {
      currentTime = currentTime.add(sessionDuration + bufferTime, 'minute');
      continue;
    }
    
    const existingBookedSlot = bookedSlotsMap.get(key);
    
    if (existingBookedSlot && !isPast) {      
      slots.push({
        ...existingBookedSlot
      });
    } else if (!isPast) {
      slots.push({
        id: uuidv4(),
        time: `${startTimeStr} - ${endTimeStr}`,
        startTime: startTimeStr,
        endTime: endTimeStr,
        available: false, 
        booked: false
      });
    }
    
    // Move to next slot time (session duration + buffer time)
    currentTime = currentTime.add(sessionDuration + bufferTime, 'minute');
  }
  
  return slots;
};

export const generateTimeSlotsForWeek = (
  weekStartDate: string, // YYYY-MM-DD format
  startTime: string, // HH:mm format
  endTime: string, // HH:mm format
  sessionDuration: number, // minutes
  bufferTime: number, // minutes
  existingAvailability: DayAvailability = {}
): DayAvailability => {
  const weekAvailability: DayAvailability = {};
  
  // Generate slots for each day of the week
  for (let i = 0; i < 7; i++) {
    const currentDate = dayjs(weekStartDate).add(i, 'day');
    const dateKey = currentDate.format('YYYY-MM-DD');
    
    // Get existing booked slots for this day
    const existingBookedSlots = existingAvailability[dateKey]?.filter(slot => slot.booked) || [];
    
    // Generate new slots for this day
    weekAvailability[dateKey] = generateTimeSlotsForDay(
      dateKey,
      startTime,
      endTime,
      sessionDuration,
      bufferTime,
      existingBookedSlots
    );
  }
  
  return weekAvailability;
};
