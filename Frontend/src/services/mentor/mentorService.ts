import { axiosClient } from "../apiClient";
import type { Mentor } from "../../pages/Courses/types";

interface MentorListParams {
  fullName?: string;
  pageIndex?: number;
  pageSize?: number;
}


interface MentorListResponse {
  items: Mentor[];
  pageSize: number;
  pageIndex: number;
  totalPages: number;
  totalCount: number;
}

export const mentorService = {
  /**
   * Get a list of mentors with filtering
   */
  list: async (params?: MentorListParams): Promise<MentorListResponse> => {
    const response = await axiosClient.get(`Users/filter`, {
      params: {
        ...params,
        roleName: "Mentor",
      },
    });

    const data = response.data.value;
    const mentors: Mentor[] = data.items;

    return {
      items: mentors,
      pageSize: data.pageSize,
      pageIndex: data.pageIndex,
      totalPages: data.totalPages,
      totalCount: data.totalCount,
    };
  },
};

export default mentorService;
