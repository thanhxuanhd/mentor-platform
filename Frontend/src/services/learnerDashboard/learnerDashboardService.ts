import { axiosClient } from "../apiClient";

export interface LearnerUpcomingSessionResponse {
  sessionId: string;
  mentorName: string;
  mentorProfilePictureUrl?: string;
  scheduledDate: string;
  timeRange: string;
  type: string;
  status: string
}

export interface GetLearnerDashboardResponse {
  upcomingSessions: LearnerUpcomingSessionResponse[];
}

export const learnerDashboardService = {
  getLearnerDashboard: async (): Promise<GetLearnerDashboardResponse> => {
    const response = await axiosClient.get(`dashboards/learner`);
    return response.data.value;
  },

  cancelSessionBooking: async (sessionBookingId: string) => {
    const response = await axiosClient.post(`dashboards/learner/cancel/${sessionBookingId}`);
    return response.data.value;
  },

  acceptSessionBooking: async (sessionBookingId: string) => {
    const response = await axiosClient.post(`dashboards/learner/accept/${sessionBookingId}`);
    return response.data.value;
  },
};