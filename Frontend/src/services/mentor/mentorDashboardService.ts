import { axiosClient } from "../apiClient";

export interface UpcomingSessionResponse {
  learnerProfilePhotoUrl: string;
  sessionId: string;
  learnerName: string;
  scheduledDate: string;
  timeRange: string;
  type: string;
}

export interface MentorDashboardResponse {
  totalPendingSessions: number;
  totalLearners: number;
  totalCourses: number;
  upcomingSessions: number;
  completedSessions: number;
  upcomingSessionsList: UpcomingSessionResponse[];
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
