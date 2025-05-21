export interface User {
  username: string;
  email: string;
  role: string;
  joinDate: string;
  status: string;
}

export interface CreateUser {
  email: string;
  password: string;
  confirmPassword: string;
  role: number;
}
