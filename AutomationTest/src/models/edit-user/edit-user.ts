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

export interface EditUserProfileInterface {
  fullname: string;
  phoneNumber: string;
  bio?: string;
  expertise?: string[];
  skills?: string;
  experience?: string;
  availbility: number[];
  teaching?: string[];
  category?: string[];
  communication_method?: string;
  url_image?: string;
  objective?: string;
  expectedMessage: string;
}
