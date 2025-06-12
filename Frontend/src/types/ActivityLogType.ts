export interface ActivityLogRequest {
  startDateTime?: Date;
  endDateTime?: Date;
  keyword?: string;
  pageSize?: number;
  pageIndex?: number;
}

export interface ActivityLogResponse {
  id?: string;
  action?: string;
  timestamp?: string;
}
