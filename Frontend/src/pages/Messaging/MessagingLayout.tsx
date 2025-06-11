import { useCallback, useEffect, useState } from "react";
import { Avatar, App, Select } from "antd";
import DefaultAvatar from "../../assets/images/default-account.svg";
import type {
  GetDetailConversationResponse,
  GetFilterConversationResponse,
  GetMinimalConversationResponse,
} from "../../types/ChatType";
import { chatService } from "../../services/chat/chatService";
import type { NotificationProps } from "../../types/Notification";
import hubConnection from "../../services/signalR";
import type { HubConnection } from "@microsoft/signalr";
import MessagingSession from "./components/MessagingSession";
import type { AddMessageRequest } from "../../types/ChatType";

export default function MessagingLayout() {
  // List user conversations
  const [pageIndex, setPageIndex] = useState(1);
  const [conversations, setConversations] = useState<GetMinimalConversationResponse[]>([]);
  const [selectedContact, setSelectedContact] = useState<GetFilterConversationResponse | null>(null);

  // Select conversation
  const [selectedConversationId, setSelectedConversationId] = useState<string | null>(null);
  const [conversationDetails, setConversationDetails] = useState<GetDetailConversationResponse | null>(null);
  const [messagePageIndex, setMessagePageIndex] = useState<number>(1);

  // Filtering Contact search bar
  const [filterPageIndex, setFilterPageIndex] = useState(1);
  const [searchKeyword, setSearchKeyword] = useState<string | null>(null);
  const [filteredContacts, setFilteredContacts] = useState<GetFilterConversationResponse[]>([]);

  // SignalR
  const [connection, setConnection] = useState<HubConnection | null>(null);

  // Toast
  const [notify, setNotify] = useState<NotificationProps | null>(null);
  const { notification } = App.useApp();

  const fetchFilteredContacts = useCallback(async () => {
    try {
      const response = await chatService.filterConversations({
        keyword: searchKeyword,
        pageIndex: filterPageIndex,
      });
      setFilteredContacts([...filteredContacts, ...response]);
    } catch (error: any) {
      setNotify({
        type: "error",
        message: "Failed to fetch conversations",
        description: error?.response?.data?.error || "Error fetching conversations.",
      });
    }
  }, [filterPageIndex, searchKeyword]);

  const fetchConversations = useCallback(async () => {
    try {
      const response = await chatService.getConversations(pageIndex);
      setConversations(response.items);
    } catch (error: any) {
      setNotify({
        type: "error",
        message: "Failed to fetch conversations",
        description: error?.response?.data?.error || "Error fetching conversations.",
      });
    }
  }, [pageIndex]);

  const fetchConversationDetails = useCallback(async () => {
    if (selectedConversationId) {
      try {
        const response = await chatService.getById(selectedConversationId, messagePageIndex);
        setConversationDetails(response);
      } catch (error: any) {
        setNotify({
          type: "error",
          message: "Failed to fetch conversation details",
          description: error?.response?.data?.error || "Error fetching conversation details.",
        });
      }
    }
  }, [selectedConversationId, messagePageIndex]);

  const handleSendMessage = useCallback(
    (content: string) => {
      if (content.trim() && (selectedConversationId || selectedContact?.id) && connection) {
        const request: AddMessageRequest = {
          conversationId: selectedConversationId ?? null,
          recipientId: selectedContact?.id ?? null,
          content,
        };
        connection.invoke("SendMessage", request).catch((err) => {
          console.error("Send message error:", err);
          setNotify({
            type: "error",
            message: "Failed to send message",
            description: "An error occurred while sending the message.",
          });
        });
      }
    },
    [connection, selectedConversationId, selectedContact],
  );

  useEffect(() => {
    fetchConversations();
    fetchFilteredContacts();
  }, [fetchConversations, fetchFilteredContacts]);

  useEffect(() => {
    fetchConversationDetails();
  }, [fetchConversationDetails]);

  useEffect(() => {
    if (notify) {
      notification[notify.type]({
        message: notify.message,
        description: notify.description,
        placement: "topRight",
        showProgress: true,
        duration: 3,
        pauseOnHover: true,
      });
      setNotify(null);
    }
  }, [notify, notification]);

  useEffect(() => {
    setConnection(hubConnection);
  }, []);

  useEffect(() => {
    if (connection) {
      connection
        .start()
        .then(() => console.log("SignalR Connected"))
        .catch((err) => console.error("Connection error:", err));

      connection.on(
        "ReceiveMessage",
        (
          senderId: string,
          content: string,
          messageId: string,
          senderName: string,
          senderProfilePhotoUrl: string | null,
          conversationId: string,
          sentAt: string,
        ) => {
          if (conversationId === selectedConversationId) {
            setConversationDetails((prev) => {
              if (!prev) return prev;
              return {
                ...prev,
                messages: {
                  ...prev.messages,
                  items: [
                    ...prev.messages.items,
                    {
                      senderId,
                      senderName,
                      content,
                      messageId,
                      senderProfilePhotoUrl,
                      conversationId,
                      sentAt,
                    },
                  ],
                },
              };
            });
          }
        },
      );

      return () => {
        connection.off("ReceiveMessage");
        connection.stop().catch((err) => console.error("Connection stop error:", err));
      };
    }
  }, [connection, selectedConversationId]);

  function handleFilterScroll(e: React.UIEvent<HTMLDivElement>) {
    const element = e.target as HTMLElement | null;
    if (element && element.scrollHeight === element.clientHeight + element.scrollTop) {
      setFilterPageIndex(filterPageIndex + 1);
    }
  }

  function handleFilterContactSearch(value: string) {
    setSearchKeyword(value);
    setFilterPageIndex(1);
    setFilteredContacts([]); // Clear previous results on new search
  }

  function handleFilteredContactChange(
    value: string,
    option?: GetFilterConversationResponse | GetFilterConversationResponse[] | undefined,
  ) {
    if (option && !Array.isArray(option)) {
      setSelectedConversationId(option.conversationId);
      setSelectedContact(option);
      setMessagePageIndex(1); // Reset message page index
    } else {
      setSelectedConversationId(null);
      setSelectedContact(null);
      setConversationDetails(null);
    }
  }

  return (
    <div className="bg-gray-800 rounded-lg overflow-hidden shadow-lg p-6">
      <div className="mb-6">
        <h2 className="text-2xl font-semibold">Messages</h2>
        <p className="text-slate-300">Connect with your mentors and learners</p>
      </div>

      <div className="flex gap-6 h-[600px]">
        {/* Conversations List */}
        <div className="bg-slate-600/50 backdrop-blur-sm rounded-xl border border-slate-500/30 shadow-xl w-75">
          <div className="p-4 border-b border-slate-500/30">
            <Select
              showSearch
              allowClear
              placeholder="Select conversations"
              size="large"
              className="w-full"
              options={filteredContacts}
              optionRender={(option) => (
                <div className="p-1">
                  <Avatar src={option.data.photoUrl || DefaultAvatar} size={36} />
                  <span className="mx-3">{option.data.name}</span>
                </div>
              )}
              filterOption={(input, option) =>
                (option?.name ?? "").toLowerCase().includes(input.toLowerCase())
              }
              onSearch={handleFilterContactSearch}
              onChange={handleFilteredContactChange}
              onPopupScroll={handleFilterScroll}
              fieldNames={{ label: "name", value: "id" }}
            />
          </div>
          <div className="overflow-y-auto h-[calc(600px-80px)]">
            {conversations.map((conversation) => (
              <div
                key={conversation.conversationId}
                className={`p-4 border-b border-slate-500/20 cursor-pointer transition-colors hover:bg-slate-500/30 ${selectedConversationId === conversation.conversationId ? "bg-slate-500/40" : ""
                  }`}
                onClick={() => {
                  setSelectedConversationId(conversation.conversationId);
                  // Optionally, set selectedContact based on conversation data if needed
                }}
              >
                <div className="flex items-center gap-3">
                  <div className="relative">
                    {/* <Avatar src={} size={48} /> */}
                  </div>
                  <div className="flex-1 min-w-0">
                    <div className="flex items-center justify-between mb-1">
                      <h4 className="text-white font-medium truncate">
                        {conversation.conversationName}
                      </h4>
                      <span className="text-slate-400 text-xs">{conversation.lastMessage.sentAt}</span>
                    </div>
                    <div className="flex items-center justify-between">
                      <p className="text-slate-300 text-sm truncate flex-1">
                        {conversation.lastMessage.content}
                      </p>
                    </div>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>

        <div className="flex-1">
          <MessagingSession
            conversationDetails={conversationDetails}
            contactId={selectedContact?.id ?? null}
            contactName={selectedContact?.name ?? null}
            contactPhotoUrl={selectedContact?.photoUrl ?? null}
            onSendMessage={handleSendMessage}
          />
        </div>
      </div>
    </div>
  );
}