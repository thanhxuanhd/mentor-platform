import { axiosClient } from "../../services/apiClient";
import type { Course } from "./types.tsx";

interface CourseListParams {
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
  id: string;
  title: string;
  description: string;
  categoryId: string;
}

export const categoryList = async (
  pageIndex: number = 1,
  pageSize: number = 5,
  keyword: string = "",
) => {
  const response = await axiosClient.get("/Categories", {
    params: {
      pageIndex,
      pageSize,
      keyword: keyword.trim(),
    },
  });
  return response.data.value;
};


export async function list(
  params: CourseListParams,
): Promise<CourseListResponse> {
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
}

export async function update(id: string, params: CourseUpdateParams) {
  const response = await axiosClient.put(`/Course/${id}`, {
    ...params,
  });
  return response.data;
}

export async function create(params: CourseCreateParams) {
  const response = await axiosClient.post("/Course", {
    ...params,
    tags: params.tags || []
  });
  return response.data;
}

export async function del(id: string) {
  await axiosClient.delete(`/Course/${id}`);
  return true;
}
