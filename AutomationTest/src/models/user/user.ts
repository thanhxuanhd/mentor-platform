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

export interface SignUpUser {
  email: string;
  password: string;
  confirmPassword: string;
  isTermCheck: boolean;
  expectedMessage: string;
}

export interface UserProfileCreation {
  fullname: string;
  phoneNumber: string;
  bio?: string;
  role?: string;
  expertise?: string[];
  skills?: string;
  experience?: string;
  availbility: number[];
  communication_method?: string;
  url_image?: string;
  objective?: string;
  expectedMessage: string;
}
