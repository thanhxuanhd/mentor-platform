export const ApplicationStatus = {
  Submitted: 0,
  WaitingInfo: 1,
  Approved: 2,
  Rejected: 3,
} as const;

export type ApplicationStatus =
  (typeof ApplicationStatus)[keyof typeof ApplicationStatus];
