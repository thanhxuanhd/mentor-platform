import type { PaginatedList } from "./Pagination";

export interface AddMessageRequest {
  conversationId: string | null;
  recipientId: string | null;
  content: string;
}

export interface GetFilterConversationRequest {
  keyword: string | null;
  pageIndex: number;
}

export interface ConversationParticipantResponse {
  id: string;
  fullName: string;
  profilePhotoUrl: string | null;
  role: string;
}

export interface GetDetailConversationResponse {
  conversationId: string;
  conversationName: string;
  messages: PaginatedList<GetMessageResponse>;
}

export interface GetFilterConversationResponse {
  id: string;
  photoUrl: string | null;
  name: string;
  conversationId: string | null;
  isGroup: boolean;
}

export interface GetMessageResponse {
  messageId: string;
  senderId: string;
  senderName: string;
  content: string;
  sentAt: string;
  senderProfilePhotoUrl: string | null;
}

export interface GetMinimalConversationResponse {
  conversationId: string;
  conversationName: string;
  lastMessage: GetMessageResponse;
  participants: ConversationParticipantResponse[];
}
