import type { CourseFormDataOptions } from "./types.tsx";
import dayjs from "dayjs";

export type CourseStatus = "draft" | "published" | "archived" | string;
export const CourseStatesEnumMember: Record<CourseStatus, string> = {
  draft: "Draft",
  published: "Published",
  archived: "Archived",
};

export type CourseDifficulty = "beginner" | "intermediate" | "advanced";

export const CourseDifficultyEnumMember: Record<CourseDifficulty, string> = {
  beginner: "Beginner",
  intermediate: "Intermediate",
  advanced: "Advanced",
};

export type CourseMimeType = "application/pdf" | "application/octet-stream";
export const CourseResourceTypeEnumMember: Record<CourseMimeType, string> = {
  "application/pdf": "pdf",
  "application/octet-stream": "binary",
};

export const initialFormData: CourseFormDataOptions = {
  id: undefined,
  title: "",
  description: "",
  categoryId: undefined,
  status: "draft",
  dueDate: dayjs().add(1, "day").toString(),
  difficulty: "beginner",
  tags: [],
};
