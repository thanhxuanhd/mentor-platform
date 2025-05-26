import type { CategoryRequest } from '../../types/CategoryTypes';
import { axiosClient } from '../apiClient';

export const getListCategories = async (
  pageIndex: number = 1,
  pageSize: number = 5,
  keyword: string = ''
): Promise<any> => {
  const response = await axiosClient.get('categories', {
    params: {
      pageIndex,
      pageSize,
      keyword: keyword.trim(),
    },
  });
  return response.data.value;
};

export const getCategoryById = async (categoryId: string) => {
  const response = await axiosClient.get(`categories/${categoryId}`);
  return response.data.value;
}

export const createCategory = async (category: any) => {
  const response = await axiosClient.post('categories', category);
  return response.data.value;
};

export const editCategory = async (categoryId: string, request: CategoryRequest) => {
  const response = await axiosClient.put(`categories/${categoryId}`, request);
  return response.data.value;
};

export const changeCategoryStatus = async (categoryId: string) => {
  const response = await axiosClient.put(`categories/status/${categoryId}`);
  return response.data;
};

export const deleteCategory = async (categoryId: string) => {
  const response = await axiosClient.delete(`categories/${categoryId}`);
  return response.data;
};