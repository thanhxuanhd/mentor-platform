import type { CategoryRequest } from '../types/CategoryTypes';
import api from './api';

export const getListCategories = async (filters: any) => {
  const response = await api.get('categories', {
    params: {
      ...filters,
    },
  });
  return response.data.value;
};

export const getCategoryById = async (categoryId: string) => {
  const response = await api.get(`categories/${categoryId}`);
  return response.data.value;
}

export const createCategory = async (category: any) => {
  const response = await api.post('categories', category);
  return response.data.value;
};

export const editCategory = async (categoryId: string, request: CategoryRequest) => {
  const response = await api.put(`categories/${categoryId}`, request);
  return response.data.value;
}

export const changeCategoryStatus = async (categoryId: string) => {
  const response = await api.put(`categories/status/${categoryId}`);
  return response.data;
};