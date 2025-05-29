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
    try {
      const response = await axiosClient.get(`mentor-applications`, {
        params: request,
      });
      return response.data.value;
    } catch (error) {
      throw error;
    }
  },

  getMentorApplicationById: async (id: string) => {
    try {
      const response = await axiosClient.get(`mentor-applications/${id}`);
      return response.data.value;
    } catch (error) {
      throw error;
    }
  },

  updateMentorApplicationStatus: async (
    id: string,
    status: ApplicationStatus,
    note?: string,
  ) => {
    try {
      const response = await axiosClient.put(
        `mentor-applications/${id}/status`,
        {
          status,
          note,
        },
      );
      return response.data.value;
    } catch (error) {
      throw error;
    }
  },

  requestMentorApplicationInfo: async (
    id: string,
    note: string,
  ): Promise<void> => {
    try {
      await axiosClient.put(`mentor-applications/${id}/request-info`, {
        note,
      });
    } catch (error) {
      throw error;
    }
  },

  getMentorApplicationByMentorId: async (id: string) => {
    try {
      const response = await axiosClient.get(
        `mentor-applications/${id}/mentor`,
      );
      return response.data.value;
    } catch (error) {
      throw error;
    }
  },

  postMentorSubmission: async (data: FormData) => {
    const response = await axiosClient.post(
      `mentor-applications/mentor-submission`,
      data,
      {
        headers: { "Content-Type": "multipart/form-data" },
      },
    );
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
