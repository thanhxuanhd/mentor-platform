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

