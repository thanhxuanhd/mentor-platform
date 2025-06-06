import { APIRequestContext, expect } from '@playwright/test';
import { API_ENDPOINTS } from '../constants/api-endpoint-url';

export async function requestNewPasswordFromEmail(
    request: APIRequestContext,
    email: string
): Promise<string> {
    const response = await request.post(`${API_ENDPOINTS.SEND_NEW_PASSWORD}/${email}`);
    const body = await response.json();
    return body.value.newPassword;
}
