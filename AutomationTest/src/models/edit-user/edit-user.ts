export interface User {
  username: string;
  email: string;
  role: string;
  status: string;
}

export interface CreateUser {
  email: string;
  password: string;
  confirmPassword: string;
  role: number;
}
