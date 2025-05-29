import { axiosClient } from "../apiClient";
import type { CategoryRequest } from "../../types/CategoryTypes";

interface CategoryListParams {
  pageIndex?: number;
  pageSize?: number;
  keyword?: string;
  status?: boolean;
}

export const categoryService = {
  /**
   * Get a list of categories
   */
  list: async (params: CategoryListParams = {}) => {

    const response = await axiosClient.get("Categories", {
      params: {
        ...params,
        keyword: params.keyword?.trim(),
      },
    });
    return response.data.value;
  },

  /**
   * Get a category by ID
   */
  getById: async (categoryId: string) => {
    const response = await axiosClient.get(`Categories/${categoryId}`);
    return response.data.value;
  },

  /**
   * Create a new category
   */
  create: async (category: any) => {
    const response = await axiosClient.post("Categories", category);
    return response.data.value;
  },

  /**
   * Update a category
   */
  update: async (categoryId: string, request: CategoryRequest) => {
    const response = await axiosClient.put(`Categories/${categoryId}`, request);
    return response.data.value;
  },

  /**
   * Change category status
   */
  changeStatus: async (categoryId: string) => {
    const response = await axiosClient.put(`Categories/status/${categoryId}`);
    return response.data;
  },

  /**
   * Delete a category
   */
  delete: async (categoryId: string) => {
    const response = await axiosClient.delete(`Categories/${categoryId}`);
    return response.data;
  },
};

export default categoryService;
