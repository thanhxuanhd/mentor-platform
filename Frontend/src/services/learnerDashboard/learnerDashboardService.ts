import { axiosClient } from "../apiClient";

export interface LearnerUpcomingSessionResponse {
  sessionId: string;
  mentorName: string;
  mentorProfilePictureUrl?: string;
  scheduledDate: string;
  timeRange: string;
  type: string;
}

export interface GetLearnerDashboardResponse {
  upcomingSessions: LearnerUpcomingSessionResponse[];
}

export const learnerDashboardService = {
  getLearnerDashboard: async (): Promise<GetLearnerDashboardResponse> => {
    const response = await axiosClient.get(`dashboards/learner`);
    return response.data.value;
  },
};