import dayjs from 'dayjs';
import type { TimeBlock, DayAvailability } from './types';
import type { ScheduleSettingsResponse, TimeSlotResponse } from '../../services/availability/availabilityService';

/**
 * Convert API time slot to frontend TimeBlock format
 */
export const convertApiTimeSlotToTimeBlock = (apiSlot: TimeSlotResponse): TimeBlock => {
  const startTime = dayjs(`2024-01-01 ${apiSlot.startTime}`);
  const endTime = dayjs(`2024-01-01 ${apiSlot.endTime}`);
  
  return {
    id: apiSlot.id,
    time: `${startTime.format("HH:mm")} - ${endTime.format("HH:mm")}`,
    startTime: apiSlot.startTime,
    endTime: apiSlot.endTime,
    available: apiSlot.isAvailable,
    booked: apiSlot.isBooked
  };
};

/**
 * Convert API schedule settings to frontend availability format
 */
export const convertApiScheduleToAvailability = (apiSettings: ScheduleSettingsResponse): DayAvailability => {
  const availability: DayAvailability = {};
  
  Object.entries(apiSettings.availableTimeSlots).forEach(([date, slots]) => {
    availability[date] = slots.map(convertApiTimeSlotToTimeBlock);
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
    availableTimeSlots[date] = timeBlocks.map(convertTimeBlockToApiSlot);
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
