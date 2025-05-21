export interface LoginUser {
  email: string;
  password: string;
}

export interface ResetPasswordUser {
  email: string;
  currentPassword: string;
  newPassword: string;
  expectedMessage: string;
}

export interface User_Registration_Step_1 {
  email: string;
  password: string;
  confirmPassword: string;
  isTermCheck: boolean;
  expectedMessage: string;
}

