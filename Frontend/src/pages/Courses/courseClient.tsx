import { mockCourses } from "./initial-values.tsx";

interface CourseListParams {
  keyword?: string;
  state?: string;
  categoryId?: string;
  pageIndex?: number;
  pageSize?: number;
}

interface CourseCreateParams {
  title: string;
  description: string;
  categoryId: string;
  status: string;
  dueDate: string;
  difficulty: string;
  tags: string[];
  mentorId: string;
}

interface CourseUpdateParams {
  title: string;
  description: string;
  categoryId: string;
  status: string;
  difficulty: string;
  dueDate: string;
  tags: string[];
  mentorId: string;
}

function delay(ms: number) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

export async function list(params: CourseListParams) {
  await delay(250);

  let filteredCourses = [...mockCourses];

  if (params.keyword) {
    filteredCourses = filteredCourses.filter(
      (course) =>
        course.title.toLowerCase().includes(params.keyword!.toLowerCase()) ||
        course.description
          .toLowerCase()
          .includes(params.keyword!.toLowerCase()),
    );
  }

  if (params.state) {
    filteredCourses = filteredCourses.filter(
      (course) => course.status === params.state,
    );
  }

  if (params.categoryId) {
    filteredCourses = filteredCourses.filter(
      (course) => course.categoryId === params.categoryId,
    );
  }

  const startIndex = (params.pageIndex || 0) * (params.pageSize || 10);
  const endIndex = startIndex + (params.pageSize || 10);

  return {
    total: filteredCourses.length,
    data: filteredCourses.slice(startIndex, endIndex),
  };
}

export async function update(id: string, params: CourseUpdateParams) {
  await delay(250);

  const courseIndex = mockCourses.findIndex((c) => c.id === id);
  if (courseIndex === -1) {
    throw new Error("Course not found");
  }

  mockCourses[courseIndex] = {
    ...mockCourses[courseIndex],
    ...params,
    updatedAt: new Date().toISOString(),
  };

  return mockCourses[courseIndex];
}

export async function create(params: CourseCreateParams) {
  await delay(250);

  const newCourse = {
    id: String(mockCourses.length + 1),
    ...params,
    categoryName:
      mockCourses.find((c) => c.categoryId === params.categoryId)
        ?.categoryName || "Unknown",
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
    enrolledStudents: 0,
    completionRate: 0,
    materials: [],
    feedback: [],
  };

  mockCourses.push(newCourse);
  return newCourse;
}

export async function del(id: string) {
  await delay(250);

  const courseIndex = mockCourses.findIndex((c) => c.id === id);
  if (courseIndex === -1) {
    throw new Error("Course not found");
  }

  mockCourses.splice(courseIndex, 1);
  return true;
}
