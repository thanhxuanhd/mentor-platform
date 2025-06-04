import { axiosClient } from "../apiClient";
import type { SessionStatus } from "../../types/enums/SessionStatus";

export interface AvailableMentorForBookingResponse {
  mentorId: string;
  mentorName: string;
  mentorExpertise?: string[];
  mentorAvatarUrl?: string | null;
  workingStartTime: string;
  workingEndTime: string;
}

export interface AvailableTimeSlotResponse {
  id: string;
  startTime: string; // Formatted as HH:mm:ss
  endTime: string;   // Formatted as HH:mm:ss
  date: string;      // ISO date (e.g., "2025-06-04")
  isBooked: boolean;
  mentorId: string;
  mentorName: string;
}

interface AvailableTimeSlotRequest {
  date: string; // ISO date format (e.g., "2025-06-04")
}

interface CreateSessionBookingRequest {
  timeSlotId: string;
  sessionType: string; // Assuming SessionType is serialized as string (e.g., "Virtual")
}

interface SessionSlotStatusResponse {
  sessionId: string;
  slotId: string;
  mentorId: string;
  day: string; // ISO date (e.g., "2025-06-04")
  startTime: string; // HH:mm:ss
  endTime: string; // HH:mm:ss
  bookingStatus: SessionStatus;
}

export const getAvailableMentors = async () => {
  const response = await axiosClient.get('sessionbooking/available-mentors');
  console.log("Available Mentors Response:", response.data.value);
  return response.data.value;
}

export const getAvailableTimeSlots = async (mentorId: string, request: AvailableTimeSlotRequest) => {
  try {
    const response = await axiosClient.get(`sessionbooking/available-mentors/timeslots/${mentorId}`, {
      params: {
        Date: request.date,
      },
    });
    console.log("Raw API Response (Time Slots):", response.data);

    const data = response.data.value || response.data;
    if (!Array.isArray(data)) {
      throw new Error("API response does not contain a valid time slots array");
    }
    return data.map((slot: any) => ({
      id: slot.id,
      startTime: slot.startTime, // Already in HH:mm:ss
      endTime: slot.endTime,
      date: slot.date,
      isBooked: slot.isBooked,
      mentorId: slot.mentorId,
      mentorName: slot.mentorName,
    })) as AvailableTimeSlotResponse[];
  } catch (error) {
    console.error("API Error (Time Slots):", error);
    throw error;
  }
};

export const requestBooking = async (request: CreateSessionBookingRequest) => {
  try {
    const response = await axiosClient.post('sessionbooking/request', request);
    console.log("Booking Response:", response.data);

    const data = response.data.value || response.data;
    if (!data || !data.sessionId) {
      throw new Error("Invalid booking response");
    }
    return data as SessionSlotStatusResponse;
  } catch (error) {
    console.error("Booking Error:", error);
    throw error;
  }
};

export const getBookingRequestsByLearner = async () => {
  const response = await axiosClient.get('sessionbooking/timeslots/requests/me');
  console.log("Booking Requests Response:", response);

  return response.data.value;
};

export const cancelBooking = async (bookingId: string) => {
  try {
    const response = await axiosClient.post(`sessionbooking/request/${bookingId}/cancel`);
    console.log("Cancel Booking Response:", response);
    return response.data;
  } catch (error) {
    console.error("Error cancelling booking:", error);
    throw error;
  }
};