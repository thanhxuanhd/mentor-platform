import api from './api';

export const getListCategories = async (filters: any) => {
  const response = await api.get('categories', {
    params: {
      ...filters,
    },
  });
  return response.data.value;
};
