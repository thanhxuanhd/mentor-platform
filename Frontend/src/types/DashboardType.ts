import type { FileType } from "./enums/FileType";

export interface AdminDashboardMetrics {
  totalUsers: number;
  totalMentors: number;
  totalLearners: number;
  totalResources: number;
  sessionsThisWeek: number;
  pendingApplications: number;
  resourceTypeCounts: {
    resourceType: keyof typeof FileType;
    count: number;
  }[];
}

export interface MentorDashboardMetrics {
  totalLearners: number;
  totalCourses: number;
  upcomingSessions: number;
  completedSessions: number;
}
