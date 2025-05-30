import type { ApplicationStatus } from "../../types/enums/ApplicationStatus";
import type {
  MentorApplicationFilterProp,
  MentorApplicationListItemProp,
} from "../../types/MentorApplicationType";
import type { PaginatedList } from "../../types/Pagination";
import { axiosClient } from "../apiClient";

export const mentorApplicationService = {
  getMentorApplications: async (
    request: MentorApplicationFilterProp,
  ): Promise<PaginatedList<MentorApplicationListItemProp>> => {
    const response = await axiosClient.get(`mentor-applications`, {
      params: request,
    });
    return response.data.value;
  },

  getMentorApplicationById: async (id: string) => {
    const response = await axiosClient.get(`mentor-applications/${id}`);
    return response.data.value;
  },

  updateMentorApplicationStatus: async (
    id: string,
    status: ApplicationStatus,
    note?: string,
  ) => {
    const response = await axiosClient.put(`mentor-applications/${id}/status`, {
      status,
      note,
    });
    return response.data.value;
  },

  requestMentorApplicationInfo: async (
    id: string,
    note: string,
  ): Promise<void> => {
    await axiosClient.put(`mentor-applications/${id}/request-info`, {
      note,
    });
  },

  getMentorApplicationByMentorId: async (id: string) => {
    const response = await axiosClient.get(`mentor-applications/${id}/mentor`);
    return response.data.value;
  },

  postMentorSubmission: async (data: FormData) => {
    const response = await axiosClient.post(`mentor-applications`, data, {
      headers: { "Content-Type": "multipart/form-data" },
    });
    return response.data.value;
  },

  editMentorApplication: async (applicationId: string, data: FormData) => {
    const response = await axiosClient.put(
      `mentor-applications/${applicationId}`,
      data,
      {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      },
    );

    return response.data;
  },
};
