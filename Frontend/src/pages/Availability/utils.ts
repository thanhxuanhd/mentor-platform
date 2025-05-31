import dayjs from 'dayjs';
import { v4 as uuidv4 } from 'uuid';
import type { TimeBlock, DayAvailability } from './types';
import type { ScheduleSettingsResponse, TimeSlotResponse } from '../../services/availability/availabilityService';

/**
 * Convert API time slot to frontend TimeBlock format
 */
export const convertApiTimeSlotToTimeBlock = (apiSlot: TimeSlotResponse, date?: string): TimeBlock => {
  const startTime = dayjs(`2024-01-01 ${apiSlot.startTime}`);
  const endTime = dayjs(`2024-01-01 ${apiSlot.endTime}`);
  
  // Calculate if this slot is in the past
  let isPast = false;
  if (date) {
    const slotDateTime = dayjs(`${date} ${apiSlot.startTime}`);
    isPast = slotDateTime.isSameOrBefore(dayjs());
  }
  
  return {
    id: apiSlot.id,
    time: `${startTime.format("HH:mm")} - ${endTime.format("HH:mm")}`,
    startTime: apiSlot.startTime,
    endTime: apiSlot.endTime,
    available: apiSlot.isAvailable,
    booked: apiSlot.isBooked,
    isPast
  };
};

/**
 * Convert API schedule settings to frontend availability format
 */
export const convertApiScheduleToAvailability = (apiSettings: ScheduleSettingsResponse): DayAvailability => {
  const availability: DayAvailability = {};
  
  Object.entries(apiSettings.availableTimeSlots).forEach(([date, slots]) => {
    availability[date] = slots.map(slot => convertApiTimeSlotToTimeBlock(slot, date));
  });
  
  return availability;
};

/**
 * Convert frontend TimeBlock to API format
 */
export const convertTimeBlockToApiSlot = (timeBlock: TimeBlock): {
  id?: string;
  startTime: string;
  endTime: string;
  isAvailable: boolean;
  isBooked: boolean;
} => {
  return {
    id: timeBlock.id,
    startTime: timeBlock.startTime,
    endTime: timeBlock.endTime,
    isAvailable: timeBlock.available,
    isBooked: timeBlock.booked
  };
};

/**
 * Convert frontend availability to API format for saving
 */
export const convertAvailabilityToApiFormat = (
  availability: DayAvailability,
  settings: {
    weekStartDate: string;
    weekEndDate: string;
    startTime: string;
    endTime: string;
    sessionDuration: number;
    bufferTime: number;
  }
) => {
  const availableTimeSlots: Record<string, Array<{
    id?: string;
    startTime: string;
    endTime: string;
    isAvailable: boolean;
    isBooked: boolean;
  }>> = {};
  Object.entries(availability).forEach(([date, timeBlocks]) => {
    // Only include time slots that are available (selected by the user)
    // Exclude booked, unavailable, and past slots
    const availableSlots = timeBlocks.filter(slot => 
      slot.available && !slot.booked && !slot.isPast
    );
    if (availableSlots.length > 0) {
      availableTimeSlots[date] = availableSlots.map(convertTimeBlockToApiSlot);
    }
  });
  
  return {
    weekStartDate: settings.weekStartDate,
    weekEndDate: settings.weekEndDate,
    startTime: settings.startTime,
    endTime: settings.endTime,
    sessionDuration: settings.sessionDuration,
    bufferTime: settings.bufferTime,
    availableTimeSlots
  };
};

/**
 * Generate time slots for a single day based on work hours and session settings
 */
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
  // Generate slots only if they can completely fit within work hours (matching backend logic)
  while (currentTime.add(sessionDuration, 'minute').isSameOrBefore(dayEnd)) {
    const slotStart = currentTime;
    const slotEnd = currentTime.add(sessionDuration, 'minute');
    
    const startTimeStr = slotStart.format('HH:mm');
    const endTimeStr = slotEnd.format('HH:mm');
    const key = `${startTimeStr}-${endTimeStr}`;
    
    // Check if this slot is in the past (including current time slot)
    const isPast = slotStart.isSameOrBefore(dayjs());
    
    // Check if this slot already exists and is booked
    const existingBookedSlot = bookedSlotsMap.get(key);
    
    if (existingBookedSlot) {
      // Preserve the existing booked slot but update isPast
      slots.push({
        ...existingBookedSlot,
        isPast
      });
    } else {
      // Create a new slot (unselected by default)
      slots.push({
        id: uuidv4(),
        time: `${startTimeStr} - ${endTimeStr}`,
        startTime: startTimeStr,
        endTime: endTimeStr,
        available: false, // New slots are unselected by default
        booked: false,
        isPast
      });
    }
    
    // Move to next slot time (session duration + buffer time)
    currentTime = currentTime.add(sessionDuration + bufferTime, 'minute');
  }
  
  return slots;
};

/**
 * Generate time slots for a week based on work hours and session settings
 */
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
