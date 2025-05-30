import { APIRequestContext } from '@playwright/test';
import { API_ENDPOINTS } from '../constants/api-endpoint-url';
import { generateRandomRole, generateUniqueEmail } from "./generate-unique-data";

const admin = {
    email: process.env.ADMIN_USER_NAME,
    password: process.env.ADMIN_PASSWORD
}
const learner = {
    email: process.env.LEARNER_USER_NAME,
    password: process.env.LEARNER_PASSWORD
}
const mentor = {
    email: process.env.MENTOR_USER_NAME,
    password: process.env.MENTOR_PASSWORD
}

export async function getAuthToken(request: APIRequestContext, user: any): Promise<string> {
    const requestBody = {
        "email": user.email,
        "password": user.password
    }
    const response = await request.post(API_ENDPOINTS.SIGN_IN, {
        data: requestBody
    });
    const responseBody = await response.json();
    return responseBody.value.token;
}

//category
export async function createTestCategory(request: APIRequestContext, category: any): Promise<string> {
    const requestBody = {
        "name": category.name,
        "description": category.description
    }
    const token = await getAuthToken(request, admin);
    const response = await request.post(API_ENDPOINTS.CATEGORY, {
        headers: {
            Authorization: `Bearer ${token}`
        },
        data: requestBody
    });
    const responseBody = await response.json();
    return responseBody.value.id;
}

export async function getLatestCategory(request: APIRequestContext): Promise<string> {
    const token = await getAuthToken(request, admin);
    const response = await request.get(API_ENDPOINTS.CATEGORY, {
        headers: {
            Authorization: `Bearer ${token}`
        },
        params: {
            status: true
        }
    });
    const responseBody = await response.json();
    return responseBody.value?.items?.[0]?.id ?? null;
}

export async function deleteTestCategory(request: APIRequestContext, categoryId: string): Promise<void> {
    const token = await getAuthToken(request, admin);
    await request.delete(`${API_ENDPOINTS.CATEGORY}/${categoryId}`, {
        headers: {
            Authorization: `Bearer ${token}`
        }
    });
}

//course
export async function createTestCourse(request: APIRequestContext, course: any): Promise<string> {
    const requestBody = {
        "title": course.title,
        "description": course.description,
        "categoryId": await getLatestCategory(request),
        "difficulty": course.difficulty,
        "tags": course.tags,
        "dueDate": course.dueDate
    }
    const token = await getAuthToken(request, mentor);
    const response = await request.post(API_ENDPOINTS.COURSE, {
        headers: {
            Authorization: `Bearer ${token}`
        },
        data: requestBody
    });
    const responseBody = await response.json();
    return responseBody.value.id;
}

export async function getLatestCourse(request: APIRequestContext): Promise<string> {
    const token = await getAuthToken(request, mentor);
    const response = await request.get(API_ENDPOINTS.COURSE, {
        headers: {
            Authorization: `Bearer ${token}`
        },
        params: {
            PageIndex: 1,
            PageSize: 5
        }
    });
    const responseBody = await response.json();
    return responseBody.value?.items?.[0]?.id ?? null;
}

export async function deleteTestCourse(request: APIRequestContext, courseId: string): Promise<void> {
    const token = await getAuthToken(request, mentor);
    await request.delete(`${API_ENDPOINTS.COURSE}/${courseId}`, {
        headers: {
            Authorization: `Bearer ${token}`
        }
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
    const response = await request.post(`${API_ENDPOINTS.SEND_NEW_PASSWORD}/${email}`);
    const body = await response.json();
    return body.value.newPassword;
}

//user
export async function createTestUser(request: APIRequestContext, user: any): Promise<string> {
    const requestBody = {
        "email": user.email,
        "password": user.password,
        "roleId": user.roleId
    }
    await request.post(API_ENDPOINTS.SIGN_UP, {
        params: requestBody
    });
    return requestBody.email;
}

export async function deleteTestUser(request: APIRequestContext, email: string): Promise<void> {
    const token = await getAuthToken(request, admin);
    await request.delete(API_ENDPOINTS.DELETE_USER, {
        headers: {
            Authorization: `Bearer ${token}`,
            Accept: '*/*'
        },
        params: {
            email: email
        }
    });
}
