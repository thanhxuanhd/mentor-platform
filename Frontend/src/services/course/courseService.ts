import { axiosClient } from "../apiClient";
import type { Course } from "../../pages/Courses/types";
import type { CourseStatus } from "../../pages/Courses/initial-values";

interface CourseListParams {
  status?: CourseStatus;
  keyword?: string;
  difficulty?: string;
  categoryId?: string;
  mentorId?: string;
  pageIndex?: number;
  pageSize?: number;
}

interface CourseListResponse {
  items: Course[];
  pageSize: number;
  pageIndex: number;
  totalPages: number;
  totalCount: number;
}

interface CourseCreateParams {
  title: string;
  description: string;
  categoryId: string;
  mentorId: string;
  dueDate: string;
  difficulty: string;
  tags?: string[];
}

interface CourseUpdateParams {
  title: string;
  description: string;
  categoryId: string;
  mentorId: string;
  dueDate: string;
  difficulty: string;
  tags?: string[];
}

interface CourseItemCreateParams {
  title: string;
  description: string;
  mediaType: string;
  webAddress: string;
}

interface CourseItemUpdateParams {
  title: string;
  description: string;
  mediaType: string;
  webAddress: string;
}

export const courseService = {
  /**
   * Get a list of courses based on filter parameters
   */
  list: async (params: CourseListParams): Promise<CourseListResponse> => {
    const pageIndex = params.pageIndex ?? 1;
    params.pageIndex = pageIndex + 1;
    const response = await axiosClient.get("/Course", { params });
    const responseData = response.data.value;
    return {
      items: responseData.items,
      pageSize: responseData.pageSize,
      pageIndex: responseData.pageIndex,
      totalPages: responseData.totalCount,
      totalCount: responseData.totalCount,
    };
  },

  get: async (id: string): Promise<Course> => {
    const response = await axiosClient.get(`/Course/${id}`);
    return response.data.value as Course;
  },

  /**
   * Create a new course
   */
  create: async (params: CourseCreateParams) => {
    const response = await axiosClient.post("/Course", {
      ...params,
      tags: params.tags || [],
    });
    return response.data;
  },
  /**
   * Update an existing course
   */
  update: async (id: string, params: CourseUpdateParams) => {
    const response = await axiosClient.put(`/Course/${id}`, {
      ...params,
      tags: params.tags || [],
    });
    return response.data;
  },

  /**
   * Delete a course
   */
  delete: async (id: string) => {
    await axiosClient.delete(`/Course/${id}`);
    return true;
  },

  /**
   * Create a new course resource
   */
  createResource: async (courseId: string, params: CourseItemCreateParams) => {
    const response = await axiosClient.post(
      `/Course/${courseId}/resource`,
      params,
    );
    return response.data;
  },

  /**
   * Update a course resource
   */
  updateResource: async (
    courseId: string,
    resourceId: string,
    params: CourseItemUpdateParams,
  ) => {
    const response = await axiosClient.put(
      `/Course/${courseId}/resource/${resourceId}`,
      params,
    );
    return response.data;
  },
  /**
   * Delete a course resource
   * @returns void - API returns 204 No Content on success
   */
  deleteResource: async (
    courseId: string,
    resourceId: string,
  ): Promise<void> => {
    await axiosClient.delete(`/Course/${courseId}/resource/${resourceId}`);
  },
};

export default courseService;
