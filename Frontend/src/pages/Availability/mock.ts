import type { DayAvailability, SaveAvailabilityResponse } from './types';
import { v4 as uuidv4 } from 'uuid';
import dayjs from 'dayjs';

// Generate initial mock availability for the current day
export const generateInitialAvailability = (): DayAvailability => {
  const today = dayjs().format("YYYY-MM-DD");
  return {
    [today]: [
      { 
        id: uuidv4(), 
        time: "09:00 - 10:00", 
        startTime: "09:00", 
        endTime: "10:00", 
        available: false,
        booked: false 
      },
      { 
        id: uuidv4(), 
        time: "10:15 - 11:15", 
        startTime: "10:15", 
        endTime: "11:15", 
        available: true,
        booked: false 
      },
      { 
        id: uuidv4(), 
        time: "11:30 - 12:30", 
        startTime: "11:30", 
        endTime: "12:30", 
        available: true,
        booked: false 
      },
      { 
        id: uuidv4(), 
        time: "12:45 - 13:45", 
        startTime: "12:45", 
        endTime: "13:45", 
        available: true,
        booked: true 
      },
      { 
        id: uuidv4(), 
        time: "14:00 - 15:00", 
        startTime: "14:00", 
        endTime: "15:00", 
        available: false,
        booked: false 
      },
      { 
        id: uuidv4(), 
        time: "15:15 - 16:15", 
        startTime: "15:15", 
        endTime: "16:15", 
        available: false,
        booked: false 
      },
    ]
  };
};

// Mock function for simulating API save operations
export const mockSaveAvailability = (data: DayAvailability): Promise<SaveAvailabilityResponse> => {
  return new Promise((resolve) => {
    // Simulate network delay
    setTimeout(() => {
      console.log("Saved availability:", data);
      // 95% success rate to simulate occasional failures
      const success = Math.random() < 0.95;
      resolve({
        success,
        message: success ? "Availability saved successfully!" : "Failed to save availability. Please try again."
      });
    }, 800);
  });
};
