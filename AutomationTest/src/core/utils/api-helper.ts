import { APIRequestContext } from "@playwright/test";
import { API_ENDPOINTS } from "../constants/api-endpoint-url";
import {
  endWithTimestamp,
  generateRandomRole,
  generateUniqueEmail,
} from "./generate-unique-data";
import path from "path";
import fs from "fs";

const admin = {
  email: process.env.ADMIN_USER_NAME,
  password: process.env.ADMIN_PASSWORD,
};
const learner = {
  email: process.env.LEARNER_USER_NAME,
  password: process.env.LEARNER_PASSWORD,
};
const mentor = {
  email: process.env.MENTOR_USER_NAME,
  password: process.env.MENTOR_PASSWORD,
};

export async function getAuthToken(
  request: APIRequestContext,
  user: any
): Promise<string> {
  const requestBody = {
    email: user.email,
    password: user.password,
  };
  const response = await request.post(API_ENDPOINTS.SIGN_IN, {
    data: requestBody,
  });
  const responseBody = await response.json();
  return responseBody.value.token;
};

//category
export async function createTestCategory(
  request: APIRequestContext
): Promise<any> {
  const requestBody = {
    name: endWithTimestamp("A new Cate"),
    status: true
  };
  const token = await getAuthToken(request, admin);
  const response = await request.post(API_ENDPOINTS.CATEGORY, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
    data: requestBody,
  });
  const responseBody = await response.json();
  return responseBody.value;
}

export async function getLatestCategory(
  request: APIRequestContext
): Promise<string> {
  const token = await getAuthToken(request, admin);
  const response = await request.get(API_ENDPOINTS.CATEGORY, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
    params: {
      status: true,
    },
  });
  const responseBody = await response.json();
  return responseBody.value?.items?.[0]?.id ?? null;
}

export async function deleteTestCategory(
  request: APIRequestContext,
  categoryId: string
): Promise<void> {
  const token = await getAuthToken(request, admin);
  await request.delete(`${API_ENDPOINTS.CATEGORY}/${categoryId}`, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
}

//course
export async function createTestCourse(
  request: APIRequestContext,
  categoryId: any
): Promise<any> {
  const requestBody = {
    title: endWithTimestamp("A New Course"),
    description: "Description",
    categoryId: categoryId,
    difficulty: "Beginner",
    tags: [],
    dueDate: "2025-06-25T10:28:04.881Z",
  };
  const token = await getAuthToken(request, mentor);
  const response = await request.post(API_ENDPOINTS.COURSE, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
    data: requestBody,
  });
  const responseBody = await response.json();
  return responseBody.value;
}

export async function getLatestCourse(
  request: APIRequestContext
): Promise<string> {
  const token = await getAuthToken(request, mentor);
  const response = await request.get(API_ENDPOINTS.COURSE, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
    params: {
      PageIndex: 1,
      PageSize: 5,
    },
  });
  const responseBody = await response.json();
  return responseBody.value?.items?.[0]?.id ?? null;
}

export async function deleteTestCourse(
  request: APIRequestContext,
  courseId: string
): Promise<void> {
  const token = await getAuthToken(request, mentor);
  await request.delete(`${API_ENDPOINTS.COURSE}/${courseId}`, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
}

export async function requestCreateNewUser(request: APIRequestContext) {
  const randomRole = generateRandomRole();
  const randomEmail = generateUniqueEmail();

  for (let i = 0; i < 30; i++) {
    await request.post(`${API_ENDPOINTS.CREATE_USER}`, {
      data: {
        password: "Testing@123",
        confirmPassword: "Testing@123",
        email: randomEmail,
        roleId: randomRole,
      },
    });
  }
}

export async function requestNewPasswordFromEmail(
  request: APIRequestContext,
  email: string
): Promise<string> {
  const response = await request.post(
    `${API_ENDPOINTS.SEND_NEW_PASSWORD}/${email}`
  );
  const body = await response.json();
  return body.value.newPassword;
}

//Application
export async function requestCreateNewApplication(request: APIRequestContext) {
  const filePath = path.resolve(
    __dirname,
    "../../tests/test-data/mentor-application/img/valid_image.png"
  );
  const fileBuffer = fs.readFileSync(filePath);

  const token = await getAuthToken(request, mentor);
  const response = await request.post(API_ENDPOINTS.MENTOR_SUBMISSION, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
    multipart: {
      education: "FPT University",
      experiences: "5 years in .NET",
      certifications: "IELTS 8.0",
      statement: "This is a test application",
      documents: {
        name: "image.png",
        mimeType: "image/png",
        buffer: fileBuffer,
      },
    },
  });

  return response;
}

export async function createTestLoginUser(request: APIRequestContext): Promise<any> {
  const randomEmail = generateUniqueEmail();
  const testData = {
    password: "Testing@123",
    confirmPassword: "Testing@123",
    email: randomEmail,
    roleId: 3
  }
  await request.post(`${API_ENDPOINTS.CREATE_USER}`, {
    data: testData
  });
  return await testData;
}
