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
  roleId: number;
}

export interface UserFilterPagedRequest {
  pageIndex: number;
  pageSize: number;
  roleName: string | null;
  fullName: string | null;
}
