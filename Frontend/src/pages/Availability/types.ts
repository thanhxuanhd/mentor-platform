// Types for Mentor Availability Management

export interface TimeSlot {
  id: string;
  time: string;
  startTime: string;
  endTime: string;
  available: boolean;
  booked: boolean;
}

export interface DayAvailability {
  [date: string]: TimeSlot[];
}

export interface WeekDay {
  day: string;
  shortDay: string;
  date: string;
  fullDate: string;
  isToday: boolean;
  active: boolean;
  hasAvailability: boolean;
}

export interface AvailabilitySettings {
  startTime: string;
  endTime: string;
  sessionDuration: number;
  bufferTime: number;
}

export interface SaveAvailabilityResponse {
  success: boolean;
  message: string;
}
