import type { PaginatedList } from "../../types/Pagination";
import type {
  EditUserRequest,
  GetUserResponse,
  UserFilterPagedRequest,
} from "../../types/UserTypes";
import { axiosClient } from "../apiClient";

export const userService = {
  getUsers: async (
    request: UserFilterPagedRequest,
  ): Promise<PaginatedList<GetUserResponse>> => {
    const response = await axiosClient.get(`Users/filter`, { params: request });
    return response.data.value;
  },

  updateUser: async (userId: string, userData: EditUserRequest) => {
    await axiosClient.put(`Users/${userId}`, userData).then((response) => {
      return response.data.value;
    });
  },

  changeUserStatus: async (userId: string) => {
    const response = await axiosClient.put(`Users/status/${userId}`);
    return response.data.value;
  },
  forgotPassword: async (email: string) => {
    const encodedEmail = encodeURIComponent(email);
    const response = await axiosClient.post(`/Users/request-forgot-password/${encodedEmail}`);
    return response.data.value;
  },
};
