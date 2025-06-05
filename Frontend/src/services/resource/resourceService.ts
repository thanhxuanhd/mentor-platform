import type { PaginatedList } from "../../types/Pagination";
import type {
  CourseResourceResponse,
  FilterResourceRequest,
} from "../../types/ResourceType";
import { axiosClient } from "../apiClient";

export const resourceService = {
  getResources: async (
    request: FilterResourceRequest,
  ): Promise<PaginatedList<CourseResourceResponse>> => {
    return await axiosClient
      .get(`Resources`, { params: request })
      .then((response) => {
        return response.data.value;
      })
      .catch((error) => {
        throw error;
      });
  },

  createResource: async (
    resource: FormData,
  ): Promise<CourseResourceResponse> => {
    return await axiosClient
      .post(`Resources`, resource, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((response) => {
        return response.data.value;
      })
      .catch((error) => {
        throw error;
      });
  },

  editResource: async (
    resourceId: string,
    resource: FormData,
  ): Promise<CourseResourceResponse> => {
    return await axiosClient
      .put(`Resources/${resourceId}`, resource, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((response) => {
        return response.data.value;
      })
      .catch((error) => {
        throw error;
      });
  },

  deleteResource: async (resourceId: string): Promise<void> => {
    return await axiosClient
      .delete(`Resources/${resourceId}`)
      .then(() => {
        // Deletion successful, no data to return
      })
      .catch((error) => {
        throw error;
      });
  },
};
