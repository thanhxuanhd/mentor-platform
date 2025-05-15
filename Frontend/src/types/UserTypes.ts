export interface GetUserResponse {
  id: string;
  fullName: string;
  email: string;
  roleId: 1 | 2 | 3;
  joinedDate: string;
  status: 0 | 1 | 2;
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
