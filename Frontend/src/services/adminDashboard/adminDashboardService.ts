import { axiosClient } from "../apiClient";

interface DashboardResponse {
  totalUsers: number;
  totalMentors: number;
  totalLearners: number;
  totalResources: number;
  sessionsThisWeek: number;
  pendingApplications: number;
}

export const dashboardService = {
  getAdminDashboard: async (): Promise<DashboardResponse> => {
    try {
      const response = await axiosClient.get("/dashboards/admin");
      return response.data.value;
    } catch (error) {
      throw new Error(
        "Failed to fetch admin dashboard: " + (error as Error).message,
      );
    }
  },
};
