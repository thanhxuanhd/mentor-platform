export interface CreateUserReq {
  firstName: string;
  lastName: string;
  dateOfBirth: string;
  joinedDate: string;
  gender: number;
  role: number;
  location: number;
}

export interface SignUpReq{
  email: string;
  password: string;
  confirmpassword: string;
  roleId: number;
}
export interface LoginReq {
  email: string;
  password: string;
}

export interface ResetPasswordReq {
  email: string;
  oldPassword: string;
  newPassword: string;
}