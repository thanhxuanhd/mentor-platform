import { axiosClient } from "../apiClient";

// API response types
interface TimeSlotResponse {
  id: string;
  startTime: string;
  endTime: string;
  isAvailable: boolean;
  isBooked: boolean;
}

interface ScheduleSettingsResponse {
  weekStartDate: string;
  weekEndDate: string;
  startTime: string;
  endTime: string;
  sessionDuration: number;
  bufferTime: number;
  isLocked: boolean;
  availableTimeSlots: Record<string, TimeSlotResponse[]>;
}

interface ApiResponse<T> {
  value: T;
  statusCode: number;
  isSuccess: boolean;
  error: string | null;
}

// Request types
interface GetScheduleSettingsRequest {
  weekStartDate?: string; // YYYY-MM-DD format
  weekEndDate?: string;   // YYYY-MM-DD format
}

interface SaveScheduleSettingsRequest {
  weekStartDate: string;
  weekEndDate: string;
  startTime: string;
  endTime: string;
  sessionDuration: number;
  bufferTime: number;
  availableTimeSlots: Record<string, Array<{
    id?: string;
    startTime: string;
    endTime: string;
    isAvailable: boolean;
    isBooked: boolean;
  }>>;
}

export const availabilityService = {
  /**
   * Get schedule settings and available time slots for a mentor
   */
  getScheduleSettings: async (
    mentorId: string,
    params?: GetScheduleSettingsRequest
  ): Promise<ScheduleSettingsResponse> => {
    const queryParams = new URLSearchParams();
    
    if (params?.weekStartDate) {
      queryParams.append('WeekStartDate', params.weekStartDate);
    }
    if (params?.weekEndDate) {
      queryParams.append('WeekEndDate', params.weekEndDate);
    }

    const queryString = queryParams.toString();
    const url = `schedules/${mentorId}/settings${queryString ? `?${queryString}` : ''}`;
    
    const response = await axiosClient.get<ApiResponse<ScheduleSettingsResponse>>(url);
    
    if (!response.data.isSuccess) {
      throw new Error(response.data.error || 'Failed to fetch schedule settings');
    }
    
    return response.data.value;
  },

  /**
   * Save schedule settings and available time slots for a mentor
   */
  saveScheduleSettings: async (
    mentorId: string,
    request: SaveScheduleSettingsRequest
  ): Promise<{ success: boolean; message: string }> => {
    const response = await axiosClient.post<ApiResponse<{ message: string }>>(
      `schedules/${mentorId}/settings`,
      request
    );
    
    if (!response.data.isSuccess) {
      throw new Error(response.data.error || 'Failed to save schedule settings');
    }
    
    return {
      success: true,
      message: response.data.value?.message || 'Schedule settings saved successfully'
    };
  }
};

export type { ScheduleSettingsResponse, TimeSlotResponse, SaveScheduleSettingsRequest };
