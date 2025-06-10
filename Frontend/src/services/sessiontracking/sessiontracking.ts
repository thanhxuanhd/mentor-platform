import { axiosClient } from "../apiClient";
import type { SessionBookingRequest } from "../../types/SessionBookingTypes";

export interface TimeSlot {
  id: string;
  startTime: string;
  endTime: string;
  date: string;
  mentorId: string;
  mentorName: string;
  isBooked: boolean;
}

export interface AvailableTimeslotsResponse {
  value: {
    items: TimeSlot[];
  };
}

export const sessionBookingService = {
  getSessionBookings: async (): Promise<SessionBookingRequest[]> => {
    try {
      const response = await axiosClient.get(`SessionBooking/request/get`);
      return response.data.value || response.data;
    } catch (error) {
      console.error("Error fetching session bookings:", error);
      throw error;
    }
  },

  getSessionDetails: async (sessionId: string): Promise<SessionBookingRequest> => {
    try {
      const response = await axiosClient.get(`SessionBooking/request/get/${sessionId}`);
      return response.data.value || response.data;
    } catch (error) {
      console.error("Error fetching session details:", error);
      throw error;
    }
  },

  getAvailableTimeslots: async (mentorId: string): Promise<TimeSlot[]> => {
    try {
      const response = await axiosClient.get(`SessionBooking/available-timeslots/${mentorId}`);
      const data = response.data.value || response.data;
      return data.items || data || [];
    } catch (error) {
      console.error("Error fetching available timeslots:", error);
      throw error;
    }
  },

  getAvailableTimeslotsByDate: async (mentorId: string, date?: string): Promise<TimeSlot[]> => {
    const formattedDate = date?.split("T")[0];
    const url = formattedDate
      ? `SessionBooking/available-mentors/timeslots/get/${mentorId}?date=${formattedDate}`
      : `SessionBooking/available-mentors/timeslots/get/${mentorId}`;

    try {
      const response = await axiosClient.get(url);
      let data = response.data.value || response.data;
      console.log(data)
      if (!data) {
        console.warn("No data returned from API");
        return [];
      }

      data = data.items || data;

      if (!Array.isArray(data)) {
        console.warn("Data is not an array:", data);
        return [];
      }

      return data.map((slot: TimeSlot) => ({
        id: slot.id || "",
        startTime: slot.startTime || "00:00",
        endTime: slot.endTime || "00:00",
        date: typeof slot.date === "string" ? slot.date.split("T")[0] : formattedDate || "",
        mentorId: slot.mentorId || mentorId,
        mentorName: slot.mentorName || "",
        isBooked: slot.isBooked || false,
      }));
    } catch (error) {
      console.error("Error fetching available timeslots by date:", error);
      throw error;
    }
  },

  updateSessionStatus: async (id: string, status: string): Promise<void> => {
    const statusMap: { [key: string]: number } = {
      Pending: 0,
      Approved: 1,
      Completed: 2,
      Canceled: 3,
      Rescheduled: 4,
    };

    const numericStatus = statusMap[status];
    if (numericStatus === undefined) {
      throw new Error(`Invalid status: ${status}`);
    }

    try {
      const response = await axiosClient.put(`SessionBooking/${id}`, {
        status: numericStatus,
      });
      return response.data.value;
    } catch (error) {
      console.error("Error updating session status:", error);
      throw error;
    }
  },

  updateSessionBooking: async (
    id: string,
    updateData: Partial<SessionBookingRequest>,
  ): Promise<SessionBookingRequest> => {
    const statusMap: { [key: string]: number } = {
      Pending: 0,
      Approved: 1,
      Completed: 2,
      Canceled: 3,
      Rescheduled: 4,
    };

    const requestData: Partial<SessionBookingRequest> = { ...updateData };

    if (typeof updateData.status === "string") {
      const numericStatus = statusMap[updateData.status];
      if (numericStatus !== undefined) {
        requestData.status = numericStatus;
      } else {
        throw new Error(`Invalid status: ${updateData.status}`);
      }
    }

    try {
      const response = await axiosClient.put(`SessionBooking/${id}`, requestData);
      return response.data.value;
    } catch (error) {
      console.error("Error updating session booking:", error);
      throw error;
    }
  },

  rescheduleSession: async (
    id: string,
    rescheduleData: {
      timeslotId: string;
      reason: string;
    },
  ): Promise<void> => {
    try {
      const response = await axiosClient.put(`SessionBooking/update/${id}`, rescheduleData);
      return response.data.value;
    } catch (error) {
      console.error("Error rescheduling session:", error);
      throw error;
    }
  },
};