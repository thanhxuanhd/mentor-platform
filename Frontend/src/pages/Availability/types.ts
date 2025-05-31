// Types for Mentor Availability Management

export interface TimeBlock {
  id: string;
  time: string;
  startTime: string;
  endTime: string;
  available: boolean;
  booked: boolean;
  isPast?: boolean;
}

export interface DayAvailability {
  [date: string]: TimeBlock[];
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
