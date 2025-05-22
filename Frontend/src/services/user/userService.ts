import type {
  EditUserRequest,
  GetUserResponse,
  UserFilterPagedRequest,
} from "../../types/UserTypes";
import { axiosClient } from "../apiClient";
import type { PaginatedList } from "../../types/Pagination";

export const userService = {
  getUserById: async (userId: string): Promise<GetUserResponse> => {
    return await axiosClient
      .get(`Users/${userId}`)
      .then((response) => {
        return response.data.value;
      })
      .catch((error) => {
        throw error;
      });
  },

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

  forgotPassword: async (email: string) => {
    const encodedEmail = encodeURIComponent(email);
    const response = await axiosClient.post(
      `/Users/request-forgot-password/${encodedEmail}`,
    );
    return response.data.value;
  },
  updateUserDetails: async (userId: string) => {
    const response = await axiosClient.put(`Users/${userId}/detail`);
    return response.data.value;
  }
};
