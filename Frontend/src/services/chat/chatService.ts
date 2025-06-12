import type {
  GetDetailConversationResponse,
  GetFilterConversationRequest,
  GetFilterConversationResponse,
  GetMinimalConversationResponse,
} from "../../types/ChatType";
import { axiosClient } from "../apiClient";

export const chatService = {
  getConversations: async (): Promise<GetMinimalConversationResponse[]> => {
    return await axiosClient
      .get(`/Messages/conversations`)
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
    skip: number,
  ): Promise<GetDetailConversationResponse> => {
    return await axiosClient
      .get(`/Messages/conversations/${conversationId}`, {
        params: { pageIndex, skip },
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
