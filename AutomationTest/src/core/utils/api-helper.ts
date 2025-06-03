import { APIRequestContext } from '@playwright/test';
import { API_ENDPOINTS } from '../constants/api-endpoint-url';
import { endWithTimestamp, generateRandomRole, generateUniqueEmail } from "./generate-unique-data";
import categoryData from '../../tests/test-data/category-data.json'
import courseData from '../../tests/test-data/course-data.json'

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
export async function createTestCategory(request: APIRequestContext): Promise<any> {
    const testData = categoryData.create_valid_category;
    const requestBody = {
        "name": endWithTimestamp(testData.name),
        "status": true
    }
    const token = await getAuthToken(request, admin);
    const response = await request.post(API_ENDPOINTS.CATEGORY, {
        headers: {
            Authorization: `Bearer ${token}`
        },
        data: requestBody
    });
    const responseBody = await response.json();
    return await responseBody.value;
}

export async function getCategoryNameById(request: APIRequestContext, categoryId: string): Promise<string> {
    const token = await getAuthToken(request, admin);
    const response = await request.get(`${API_ENDPOINTS.CATEGORY}/${categoryId}`, {
        headers: {
            Authorization: `Bearer ${token}`
        }
    });
    const responseBody = await response.json();
    return responseBody.value.name;
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
export async function createTestCourse(request: APIRequestContext, categoryId: string): Promise<any> {
    const testData = courseData.create_valid_course;
    const requestBody = {
        "title": endWithTimestamp(testData.title),
        "description": testData.description,
        "categoryId": categoryId,
        "difficulty": testData.difficulty,
        "tags": testData.tags,
        "dueDate": testData.dueDate
    }
    const token = await getAuthToken(request, mentor);
    const response = await request.post(API_ENDPOINTS.COURSE, {
        headers: {
            Authorization: `Bearer ${token}`
        },
        data: requestBody
    });
    const responseBody = await response.json();
    return await responseBody.value;
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

export async function getCourseTitleById(request: APIRequestContext, courseId: string): Promise<string> {
    const token = await getAuthToken(request, mentor);
    const response = await request.get(`${API_ENDPOINTS.COURSE}/${courseId}`, {
        headers: {
            Authorization: `Bearer ${token}`
        }
    });
    const responseBody = await response.json();
    return responseBody.value.title;
}

export async function deleteTestCourse(request: APIRequestContext, courseId: string): Promise<void> {
    const token = await getAuthToken(request, mentor);
    await request.delete(`${API_ENDPOINTS.COURSE}/${courseId}`, {
        headers: {
            Authorization: `Bearer ${token}`,
            Accept: '*/*'
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
    await request.delete(API_ENDPOINTS.USER, {
        headers: {
            Authorization: `Bearer ${token}`,
            Accept: '*/*'
        },
        params: {
            email: email
        }
    });
}

export async function getUserNameById(request: APIRequestContext, userId: string): Promise<string> {
    const token = await getAuthToken(request, admin);
    const response = await request.get(`${API_ENDPOINTS.USER}/${userId}`, {
        headers: {
            Authorization: `Bearer ${token}`,
            Accept: '*/*'
        },
    });
    const responseBody = await response.json();
    return responseBody.value.fullName;
}
