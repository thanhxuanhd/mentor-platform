import type { CommunicationMethod } from "./enums/CommunicationMethod";
import type { LearningStyle } from "./enums/LearningStyle";
import type { SessionFrequency } from "./enums/SessionFrequency";

export interface GetUserResponse {
  id: string;
  fullName: string;
  email: string;
  role: "Admin" | "Mentor" | "Learner";
  joinedDate: string;
  status: "Pending" | "Active" | "Deactivated";
  lastActive: string;
}

export interface EditUserRequest {
  id: string;
  fullName: string;
  email: string;
  role: string;
}

export interface UserFilterPagedRequest {
  pageIndex: number;
  pageSize: number;
  roleName: string | null;
  fullName: string | null;
}

export interface UserProfile {
  id: string;
  fullName: string;
  roleId: number;
  bio: string | null;
  profilePhotoUrl: string | null;
  phoneNumber: string;
  skills: string | null;
  experiences: string | null;
  preferredCommunicationMethod: number;
  goal: string | null;
  preferredSessionFrequency: number;
  preferredSessionDuration: number;
  preferredLearningStyle: number;
  isPrivate: boolean;
  isAllowedMessage: boolean;
  isReceiveNotification: boolean;
  availabilityIds: string[];
  expertiseIds: string[];
  teachingApproachIds: string[];
  categoryIds: string[];
}

export interface UpdateProfileRequest {
  fullName?: string;
  roleId?: number;
  bio?: string;
  profilePhotoUrl?: string;
  phoneNumber?: string;
  skills?: string;
  experiences?: string;
  preferredCommunicationMethod?: number;
  goal?: string;
  preferredSessionFrequency?: number;
  preferredSessionDuration?: number;
  preferredLearningStyle?: number;
  isPrivate?: boolean;
  isAllowedMessage?: boolean;
  isReceiveNotification?: boolean;
  availabilityIds: string[];
  expertiseIds: string[];
  teachingApproachIds: string[];
  categoryIds: string[];
}

export interface UserDetail {
  fullName: string;
  roleId: number;
  bio: string;
  profilePhotoUrl: string;
  phoneNumber: string;
  skills: string;
  experiences: string;
  goal: string;
  preferredCommunicationMethod: CommunicationMethod;
  preferredSessionFrequency: SessionFrequency;
  preferredSessionDuration: number;
  preferredLearningStyle: LearningStyle;
  isPrivate: boolean;
  isAllowedMessage: boolean;
  isReceiveNotification: boolean;
  availabilityIds: string[];
  expertiseIds: string[];
  teachingApproachIds: string[];
  categoryIds: string[];
}

export interface TeachingApproach {
  id: string;
  name: string;
}
