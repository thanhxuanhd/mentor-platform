import type {
  ActivityLogRequest,
  ActivityLogResponse,
} from "../../types/ActivityLogType";
import type { PaginatedList } from "../../types/Pagination";
import { axiosClient } from "../apiClient";

export const activityLogService = {
  getActivityLogs: async (
    params: ActivityLogRequest,
  ): Promise<PaginatedList<ActivityLogResponse>> => {
    try {
      const response = await axiosClient.get("/activity-logs", {
        params: {
          startDateTime: params.startDateTime?.toISOString() || undefined,
          endDateTime: params.endDateTime?.toISOString() || undefined,
          keyword: params.keyword || undefined,
          pageSize: params.pageSize ?? 5,
          pageIndex: params.pageIndex ?? 1,
        },
      });
      return response.data.value;
    } catch (error) {
      throw new Error(
        "Failed to fetch activity logs: " + (error as Error).message,
      );
    }
  },
};
