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
  duration: number;
  difficulty: string;
  tags: string[];
  mentorId: string;
}

interface CourseUpdateParams {
  title: string;
  description: string;
  categoryId: string;
  status: string;
  duration: number;
  difficulty: string;
  tags: string[];
  mentorId: string;
}

function getRandomIntInclusive(min: number, max: number) {
  min = Math.ceil(min);
  max = Math.floor(max);
  return Math.floor(Math.random() * (max - min + 1)) + min;
}

function delay(ms: number) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

export async function list(params: CourseListParams) {
  await delay(1000);
  const page = getRandomIntInclusive(1, 2);
  return mockCourses.slice(0, page);
}

export async function update(params: CourseUpdateParams) {}

export async function create(params: CourseCreateParams) {}

export async function del(id: string) {}
