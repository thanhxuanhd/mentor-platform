import type {
  GetDetailConversationResponse,
  GetFilterConversationRequest,
  GetFilterConversationResponse,
  GetMinimalConversationResponse,
} from "../../types/ChatType";
import type { PaginatedList } from "../../types/Pagination";
import { axiosClient } from "../apiClient";

export const chatService = {
  getConversations: async (
    pageIndex: number,
  ): Promise<PaginatedList<GetMinimalConversationResponse>> => {
    return await axiosClient
      .get(`/Messages/conversations`, {
        params: { pageIndex },
      })
      .then((response) => {
        return response.data.value;
      })
      .catch((error) => {
        throw error;
      });
  },

  getById: async (
    conversationId: string,
    pageIndex: number,
  ): Promise<GetDetailConversationResponse> => {
    return await axiosClient
      .get(`/Messages/conversations/${conversationId}`, {
        params: { pageIndex },
      })
      .then((response) => {
        return response.data.value;
      })
      .catch((error) => {
        throw error;
      });
  },

  filterConversations: async (
    request: GetFilterConversationRequest,
  ): Promise<GetFilterConversationResponse[]> => {
    return await axiosClient
      .get(`/Messages/conversations/filter`, {
        params: request,
      })
      .then((response) => {
        return response.data.value;
      })
      .catch((error) => {
        throw error;
      });
  },
};
