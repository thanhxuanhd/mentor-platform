import { axiosClient } from "../apiClient";
import type { Mentor } from "../../pages/Courses/types";

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
  list: async (keyword: string = ""): Promise<MentorListResponse> => {
    const response = await axiosClient.get(`Users/filter`, {
      params: {
        pageIndex: 1,
        pageSize: 100, // Get all mentors since we want to show all in dropdown
        roleName: "Mentor",
        fullName: keyword || null,
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
