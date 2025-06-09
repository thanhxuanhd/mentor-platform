const baseAPIUrl = process.env.BASE_BE_URL;

export const API_ENDPOINTS = {
  SEND_NEW_PASSWORD: baseAPIUrl + "/api/Users/request-forgot-password",
  CREATE_USER: baseAPIUrl + "/api/Auth/sign-up",
  CATEGORY: baseAPIUrl + "/api/Categories",
  SIGN_IN: baseAPIUrl + "/api/Auth/sign-in",
  COURSE: baseAPIUrl + "/api/Courses",
  MENTOR_SUBMISSION: baseAPIUrl + "/api/mentor-applications",
  RESOURCE: baseAPIUrl +"/api/Resources",
  SIGN_UP: baseAPIUrl + "/api/Users/test",
  USER: baseAPIUrl + "/api/Users",
};
