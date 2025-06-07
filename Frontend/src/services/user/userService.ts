import type {
  EditUserRequest,
  GetUserResponse,
  UserDetail,
  UserFilterPagedRequest,
} from "../../types/UserTypes";
import { axiosClient } from "../apiClient";
import type { PaginatedList } from "../../types/Pagination";
import type { UserProfile, UpdateProfileRequest } from "../../types/UserTypes";

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

  updateUserDetail: async (userId: string, userDetail: UserDetail) => {
    await axiosClient
      .put(`Users/${userId}/detail`, userDetail)
      .then((response) => {
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

  removeAvatar: async (imageUrl: string) => {
    await axiosClient
      .delete(`Users/avatar`, { params: { imageUrl } })
      .then((response) => {
        return response.data.value;
      });
  },
  getUserProfile: async (userId: string): Promise<UserProfile> => {
    return await axiosClient
      .get(`Users/${userId}/detail`)
      .then((response) => response.data.value)
      .catch((error) => {
        throw error;
      });
  },

  getUserDetail: async (userId: string): Promise<UserDetail> => {
    return await axiosClient
      .get(`Users/${userId}/detail`)
      .then((response) => {
        return response.data.value;
      })
      .catch((error) => {
        throw error;
      });
  },

  updateUserProfile: async (
    userId: string,
    profileData: UpdateProfileRequest,
  ): Promise<UserProfile> => {
    return await axiosClient
      .put(`Users/${userId}/detail`, profileData)
      .then((response) => response.data.value)
      .catch((error) => {
        throw error;
      });
  },

  uploadProfilePhoto: async (userId: string, file: File): Promise<string> => {
    const formData = new FormData();
    formData.append("file", file);
    return await axiosClient
      .post(`Users/${userId}`, formData, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((response) => response.data.url)
      .catch((error) => {
        throw error;
      });
  },
  getAvailabilities: async (token: string) => {
    const response = await axiosClient.get("/Availabilities", {
      headers: { Authorization: `Bearer ${token}` },
    });
    return response.data.value as { id: string; name: string }[];
  },

  getExpertises: async (token: string) => {
    const response = await axiosClient.get("/Expertises", {
      headers: { Authorization: `Bearer ${token}` },
    });
    return response.data.value as { id: string; name: string }[];
  },

  getTeachingApproaches: async (token: string) => {
    const response = await axiosClient.get("/TeachingApproaches", {
      headers: { Authorization: `Bearer ${token}` },
    });
    return response.data.value as { id: string; name: string }[];
  },
};

export const getAllTeachingApproaches = async () => {
  const response = await axiosClient.get(`TeachingApproaches`);
  return response.data.value;
};
