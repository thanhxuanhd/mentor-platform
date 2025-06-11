import type React from "react";
import { useState, useRef, useEffect, useCallback } from "react";
import { Avatar, Input, Button, App, Spin } from "antd";
import { SendOutlined } from "@ant-design/icons";
import InfiniteScroll from "react-infinite-scroll-component";
import type {
  AddMessageRequest,
  GetDetailConversationResponse,
  GetMessageResponse,
} from "../../../types/ChatType";
import { useAuth } from "../../../hooks";
import type { NotificationProps } from "../../../types/Notification";
import DefaultAvatar from "../../../assets/images/default-account.svg";
import connection from "../../../services/signalR";
import { Bubble, Sender } from "@ant-design/x";
import { chatService } from "../../../services/chat/chatService";

interface MessagingSessionProps {
  conversationDetails: GetDetailConversationResponse | null;
  contactId: string | null;
  contactName: string | null;
  contactPhotoUrl: string | null;
}

export default function MessagingSession({
  conversationDetails: initialConversationDetails,
  contactId,
  contactName,
  contactPhotoUrl,
}: MessagingSessionProps) {
  const { user } = useAuth();
  const { notification } = App.useApp();
  const [conversationDetails, setConversationDetails] =
    useState<GetDetailConversationResponse | null>(initialConversationDetails);
  const [newMessage, setNewMessage] = useState("");
  const [pageIndex, setPageIndex] = useState(1);
  const [notify, setNotify] = useState<NotificationProps | null>(null);
  const messagesEndRef = useRef<HTMLDivElement>(null);
  const scrollableDivRef = useRef<HTMLDivElement>(null);

  const fetchMoreMessages = useCallback(async () => {
    try {
      const response = await chatService.getById(
        conversationDetails?.conversationId || "",
        pageIndex,
      );
      setConversationDetails((prev) => {
        if (!prev || !prev.messages) return prev;
        return {
          ...prev,
          messages: {
            ...prev.messages,
            items: [...prev.messages.items, ...response.messages.items],
          },
        };
      });
      setPageIndex((prev) => prev + 1);
    } catch (error: any) {
      setNotify({
        type: "error",
        message: "Failed to fetch conversations",
        description:
          error?.response?.data?.error || "Error fetching conversations.",
      });
    }
  }, [conversationDetails?.conversationId, pageIndex]);

  useEffect(() => {
    setConversationDetails(initialConversationDetails);
  }, [initialConversationDetails]);

  const scrollToBottom = useCallback(() => {
    messagesEndRef.current?.scrollIntoView({
      behavior: "smooth",
    });
  }, []);

  useEffect(() => {
    scrollToBottom();
  }, [conversationDetails?.messages.items, scrollToBottom]);

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
    connection.on(
      "ReceiveMessage",
      (
        senderId: string,
        content: string,
        messageId: string,
        senderName: string,
        senderProfilePhotoUrl: string | null,
        sentAt: string,
        conversationId: string,
      ) => {
        const newMessage: GetMessageResponse = {
          senderId,
          content,
          messageId,
          senderName,
          senderProfilePhotoUrl,
          sentAt,
        };

        if (conversationId === conversationDetails?.conversationId) {
          setConversationDetails((prev) => {
            if (!prev || !prev.messages) return prev;

            console.log(conversationDetails?.messages.items);

            return {
              ...prev,
              messages: {
                ...prev.messages,
                items: [newMessage, ...prev.messages.items],
              },
            };
          });
        }
      },
    );

    return () => {
      connection.off("ReceiveMessage");
    };
  }, [conversationDetails]);

  const handleSendMessage = useCallback((contentString: string) => {
    if (
      contentString.trim() &&
      (conversationDetails?.conversationId || contactId)
    ) {
      const request: AddMessageRequest = {
        conversationId: conversationDetails?.conversationId ?? null,
        recipientId: contactId ?? null,
        content: contentString,
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
  }, []);

  const formatTime = (timestamp: string) => {
    const date = new Date(timestamp);
    return date.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
  };

  return (
    <div className="bg-slate-600/50 backdrop-blur-sm rounded-xl border border-slate-500/30 shadow-xl h-[600px] flex flex-col p-4">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <div className="relative">
            <Avatar src={contactPhotoUrl || DefaultAvatar} size={40} />
          </div>
          <div>
            <h3 className="text-white font-semibold">
              {conversationDetails
                ? conversationDetails.conversationName
                : contactName || "Select a conversation"}
            </h3>
          </div>
        </div>
      </div>

      <div
        id="scrollableDiv"
        ref={scrollableDivRef}
        className="flex-1 flex-col-reverse overflow-auto p-2 mt-4 bg-slate-700 rounded-lg shadow-inner mb-4"
      >
        <InfiniteScroll
          dataLength={conversationDetails?.messages.items.length ?? 0}
          next={fetchMoreMessages}
          hasMore={true} // Set to true if you implement pagination
          inverse={true}
          loader={<Spin />}
          scrollableTarget="scrollableDiv"
        >
          {conversationDetails?.messages.items
            .slice()
            .reverse()
            .map((message: GetMessageResponse) => {
              const isCurrentUser = message.senderId === user?.id;
              return (
                <Bubble
                  key={message.messageId}
                  placement={isCurrentUser ? "end" : "start"}
                  avatar={
                    <Avatar
                      src={message.senderProfilePhotoUrl || DefaultAvatar}
                      size={32}
                    />
                  }
                  header={
                    isCurrentUser ? "You" : message.senderName || "Unknown"
                  }
                  content={
                    <p className="text-sm break-words">{message.content}</p>
                  }
                  footer={
                    <span className="text-xs text-slate-400">
                      {formatTime(message.sentAt)}
                    </span>
                  }
                  styles={{
                    content: {
                      backgroundColor: isCurrentUser ? "orange" : "darkgray",
                      padding: "10px",
                      borderRadius: "8px",
                      textWrap: "wrap",
                      maxWidth: "70%",
                    },
                    footer: { margin: 0 },
                  }}
                ></Bubble>
              );
            })}
        </InfiniteScroll>
      </div>
      <div ref={messagesEndRef} />
      <Sender
        value={newMessage}
        onChange={(v) => {
          setNewMessage(v);
        }}
        onSubmit={() => {
          if (newMessage.trim() && (conversationDetails || contactId)) {
            handleSendMessage(newMessage);
            setNewMessage("");
          }
        }}
        onCancel={() => {
          setNewMessage("");
        }}
        autoSize={{ minRows: 2, maxRows: 6 }}
        placeholder="Type your message here..."
        style={{
          backgroundColor: "rgba(255, 255, 255, 0.1)",
        }}
      />
    </div>
  );
}
