import type { Category, Course, CourseFormDataOptions } from "./types.tsx";

export type CourseStates = "draft" | "published" | "archived";
export const CourseStatesEnumMember: Record<CourseStates, string> = {
  draft: "Draft",
  published: "Published",
  archived: "Archived",
};

export type CourseLevel = "beginner" | "intermediate" | "advanced";
export const CourseLevelEnumMember: Record<CourseLevel, string> = {
  beginner: "Beginner",
  intermediate: "Intermediate",
  advanced: "Advanced",
};

export type CourseMimeType = "application/pdf" | "application/octet-stream";
export const CourseMediaTypeEnumMember: Record<CourseMimeType, string> = {
  "application/pdf": "pdf",
  "application/octet-stream": "binary",
};

export const initialFormData: CourseFormDataOptions = {
  title: "",
  description: "",
  categoryId: "",
  status: "draft",
  dueDate: "",
  difficulty: "beginner",
  tags: [],
};

export const mockCourses: Course[] = [
  {
    id: "1",
    title: "Introduction to Leadership",
    description:
      "Learn fundamental leadership principles and develop your leadership style",
    categoryId: "1",
    categoryName: "Leadership Coaching",
    createdAt: "2024-09-15",
    updatedAt: "2024-11-02",
    status: "published",
    enrolledStudents: 24,
    completionRate: 78,
    dueDate: "6 weeks",
    difficulty: "beginner",
    tags: ["leadership", "beginner"],
    materials: [
      {
        id: "101",
        name: "Leadership Basics.pdf",
        mediaType: "pdf",
        url: "/resources/leadership-basics.pdf",
        uploadedAt: "2024-09-15",
      },
      {
        id: "102",
        name: "Communication Skills.pdf",
        mediaType: "pdf",
        url: "/resources/communication-skills.pdf",
        uploadedAt: "2024-09-20",
      },
    ],
    mentorId: "101",
    feedback: [],
  },
  {
    id: "2",
    title: "Advanced Leadership Strategies",
    description:
      "Master advanced leadership techniques for managing complex teams",
    categoryId: "1",
    categoryName: "Leadership Coaching",
    createdAt: "2024-10-05",
    updatedAt: "2024-10-28",
    status: "published",
    enrolledStudents: 18,
    completionRate: 62,
    dueDate: "8 weeks",
    difficulty: "advanced",
    tags: ["leadership", "advanced"],
    materials: [],
    mentorId: "102",
    feedback: [],
  },
  {
    id: "3",
    title: "Resume Building Workshop",
    description: "Create a standout resume that gets you noticed by employers",
    categoryId: "2",
    categoryName: "Career Development",
    createdAt: "2024-08-22",
    updatedAt: "2024-10-15",
    status: "published",
    enrolledStudents: 35,
    completionRate: 91,
    dueDate: "3 weeks",
    difficulty: "beginner",
    tags: ["resume", "career"],
    materials: [],
    mentorId: "103",
    feedback: [],
  },
];

export const mockCategories: Category[] = [
  { id: "1", name: "Leadership Coaching" },
  { id: "2", name: "Career Development" },
  { id: "3", name: "Project Management" },
  { id: "4", name: "Public Speaking" },
  { id: "5", name: "Time Management" },
  { id: "6", name: "Conflict Resolution" },
];
