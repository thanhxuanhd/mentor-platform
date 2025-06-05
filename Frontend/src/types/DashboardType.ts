import type { FileType } from "./enums/FileType";

export interface DashboardMetrics {
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
