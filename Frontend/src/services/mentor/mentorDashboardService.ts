import { axiosClient } from "../apiClient";

export interface MentorDashboardResponse {
  totalLearners: number;
  totalCourses: number;
  upcomingSessions: number;
  completedSessions: number;
  upcomingSessionsList: {
    sessionId: string;
    learnerName: string;
    scheduledDate: string;
    timeRange: string;
    type: string;
  }[];
}

export const mentorDashboardService = {
  getDashboardData: async (
    mentorId: string,
  ): Promise<MentorDashboardResponse> => {
    const response = await axiosClient.get(`dashboards/mentor/${mentorId}`);

    return response.data.value;
  },
};

export default mentorDashboardService;
