import type { ApplicationStatus } from "./enums/ApplicationStatus";

export interface MentorApplicationListItemProp {
  mentorApplicationId: string;
  mentorName: string;
  email?: string;
  profilePhotoUrl?: string;
  expertises: string[];
  status: string;
  submittedAt: string;
}

export interface MentorApplicationFilterProp {
  pageSize?: number;
  pageIndex?: number;
  status?: ApplicationStatus;
  keyword?: string;
}

export interface MentorApplicationDetailItemProp {
  mentorApplicationId: string;
  mentorName: string;
  email?: string;
  bio?: string;
  profilePhotoUrl?: string;
  expertises: string[];
  experiences?: string;
  applicationStatus: string;
  education: string;
  certifications: string;
  statement: string;
  submittedAt: string;
  reviewedAt: string;
  reviewBy: string;
  note?: string;
  documents?: ApplicationDocumentProp[];
}

export interface ApplicationDocumentProp {
  documentId: string;
  documentUrl: string;
  documentType: "pdf" | "jpg";
}
