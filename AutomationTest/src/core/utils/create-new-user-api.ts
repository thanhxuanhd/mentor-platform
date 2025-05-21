import { APIRequestContext, expect } from "@playwright/test";
import { API_ENDPOINTS } from "../constants/api-endpoint-url";
import {
  generateRandomRole,
  generateUniqueEmail,
} from "./generate-unique-data";

export async function requestCreateNewUser(request: APIRequestContext) {
  const randomRole = generateRandomRole();
  const randomEmail = generateUniqueEmail();

  for (let i = 0; i < 30; i++) {
    await request.post(`${API_ENDPOINTS.CREATE_USER}`, {
      data: {
        password: "stringss",
        confirmPassword: "stringss",
        email: randomEmail,
        roleId: randomRole,
      },
    });
  }
}
