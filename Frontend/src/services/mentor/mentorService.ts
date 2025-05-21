import { axiosClient } from "../apiClient";
import type { Mentor } from "../../pages/Courses/types";

interface MentorListResponse {
  items: Mentor[];
  pageSize: number | null;
  pageIndex: number | null;
  totalPages: number | null;
  totalCount: number | null;
}

export const mentorService = {
  /**
   * Get a list of mentors
   */
  list: async (): Promise<MentorListResponse> => {
    // This is a placeholder implementation that mimics what's in courseClient.tsx
    // TODO: Replace with actual API call once implemented
    function delay(ms: number) {
      return new Promise((resolve) => setTimeout(resolve, ms));
    }

    await delay(100);

    const mentors: Mentor[] = [
      {
        id: "BC7CB279-B292-4CA3-A994-9EE579770DBE",
        name: "MySuperKawawiiMentorXxX@at.local",
      },
      {
        id: "B5095B17-D0FE-47CC-95B8-FD7E560926F8",
        name: "DuongSenpai@at.local",
      },
      {
        id: "01047F62-6E87-442B-B1E8-2A54C9E17D7C",
        name: "AnhDoSkibidi@at.local",
      },
    ];

    return {
      items: mentors,
      pageSize: null,
      pageIndex: null,
      totalPages: null,
      totalCount: null,
    };
  },
};

export default mentorService;
