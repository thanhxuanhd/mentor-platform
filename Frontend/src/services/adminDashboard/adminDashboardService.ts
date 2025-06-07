import type { FileType } from "../../types/enums/FileType";
import { downloadBlobFile, getFileNameFromDisposition } from "../../utils/File";
import { axiosClient } from "../apiClient";

interface DashboardResponse {
  totalUsers: number;
  totalMentors: number;
  totalLearners: number;
  totalResources: number;
  sessionsThisWeek: number;
  pendingApplications: number;
  resourceTypeCounts: {
    resourceType: keyof typeof FileType;
    count: number;
  }[];
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

  generateMentorActivityReport: async () => {
    try {
      const response = await axiosClient.get(
        "/dashboards/admin/report/mentor-activity",
        {
          responseType: "blob",
        },
      );
      const blob = new Blob([response.data], { type: response.data.type });
      const contentDisposition = response.headers["content-disposition"];
      const fileName = getFileNameFromDisposition(contentDisposition);
      downloadBlobFile(blob, fileName || "", blob.type);
    } catch (error) {
      throw new Error(
        `Failed to generate mentor activity report: ${(error as Error).message}`,
      );
    }
  },

  generateMonthlyMentorApplicationReport: async () => {
    try {
      const response = await axiosClient.get(
        "/dashboards/admin/report/monthly-mentor-application",
        {
          responseType: "blob",
        },
      );
      const blob = new Blob([response.data], { type: response.data.type });
      const contentDisposition = response.headers["content-disposition"];
      const fileName = getFileNameFromDisposition(contentDisposition);
      console.log(fileName);
      downloadBlobFile(blob, fileName || "", blob.type);
    } catch (error) {
      throw new Error(
        `Failed to generate monthly mentor application report: ${(error as Error).message}`,
      );
    }
  },
};
