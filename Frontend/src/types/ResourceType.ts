import type { FileType } from "./enums/FileType";

export interface CourseResourceResponse {
  id: string;
  title: string;
  description: string;
  resourceType: FileType;
  resourceUrl: string;
  courseId: string;
  courseTitle: string;
}

export interface FilterResourceRequest {
  pageIndex: number;
  pageSize: number;
  keyWord?: string | null;
  resourceType?: FileType | null;
}

export interface CourseResourceRequest {
  id: string;
  title: string;
  description: string;
  resource: File;
}
