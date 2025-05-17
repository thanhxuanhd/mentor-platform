import type {
  EditUserRequest,
  GetUserResponse,
  UserFilterPagedRequest,
} from "../../types/UserTypes";
import { axiosClient } from "../apiClient";
import type { PaginatedList } from "../../types/Pagination";

export const userService = {
  getUsers: async (
    request: UserFilterPagedRequest,
  ): Promise<PaginatedList<GetUserResponse>> => {
    return await axiosClient
      .get(`Users/filter`, { params: request })
      .then((response) => {
        return response.data.value;
      })
      .catch((error) => {
        throw error;
      });
  },

  updateUser: async (userId: string, userData: EditUserRequest) => {
    await axiosClient.put(`Users/${userId}`, userData).then((response) => {
      return response.data.value;
    });
  },

  changeUserStatus: async (userId: string) => {
    await axiosClient.put(`Users/status/${userId}`).then((response) => {
      return response.data.value;
    });
  },
};
