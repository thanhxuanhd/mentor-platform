import type React from "react";

import { useState, useRef, useEffect, useCallback } from "react";
import { Avatar, Input, Button, App } from "antd";
import { SendOutlined } from "@ant-design/icons";
import { useLocation, useParams } from "react-router-dom";
import type {
  AddMessageRequest,
  GetMessageResponse,
} from "../../../types/ChatType";
import { useAuth } from "../../../hooks";
import type { NotificationProps } from "../../../types/Notification";
import type { HubConnection } from "@microsoft/signalr";
import hubConnection from "../../../services/signalR";

const { TextArea } = Input;

export default function MessagingSession() {
  const { user } = useAuth();
  const { converId } = useParams();
  const location = useLocation();
  const { notification } = App.useApp();
  const { contactId, contactName, contactPhotoUrl } = location.state;
  const [pageIndex, setPageIndex] = useState(1);
  const [messages, setMessages] = useState<GetMessageResponse[]>([]);
  const [newMessage, setNewMessage] = useState("");
  const [notify, setNotify] = useState<NotificationProps | null>(null);
  const [connection, setConnection] = useState<HubConnection | null>(null);

  const messagesEndRef = useRef<HTMLDivElement>(null);

  const handleSendMessage = () => {
    if (newMessage.trim()) {
      const request: AddMessageRequest = {
        conversationId: converId ?? null,
        recipientId: contactId,
        content: newMessage,
      };

      setNewMessage("");

      if (connection) {
        return connection.invoke("SendMessage", request);
      }
    }
  };

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({
      behavior: "smooth",
      block: "nearest",
    });
  };

  useEffect(() => {
    setConnection(hubConnection);
  }, []);

  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  useEffect(() => {
    if (connection) {
      connection
        .start()
        .then(() => {
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
              setMessages((prev) => [
                ...prev,
                {
                  senderId,
                  senderName,
                  content,
                  messageId,
                  senderProfilePhotoUrl,
                  conversationId,
                  sentAt,
                },
              ]);
            },
          );
        })
        .catch((err) => console.error("Connection error:", err));
    }
  }, [connection]);

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

  const handleKeyPress = (e: React.KeyboardEvent) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      handleSendMessage();
    }
  };

  const formatTime = (timestamp: string) => {
    const date = new Date(timestamp);
    return date.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
  };

  return (
    <div className="bg-slate-600/50 backdrop-blur-sm rounded-xl border border-slate-500/30 shadow-xl h-[600px] flex flex-col">
      {/* Header */}
      <div className="flex items-center justify-between p-4 border-b border-slate-500/30">
        <div className="flex items-center gap-3">
          <div className="relative">
            <Avatar src={contactPhotoUrl} size={40} />
          </div>
          <div>
            <h3 className="text-white font-semibold">{contactName}</h3>
          </div>
        </div>
      </div>

      {/* Messages */}
      <div className="flex-1 overflow-y-auto p-4 space-y-4">
        {messages.map((message) => {
          const isCurrentUser = message.senderId === user?.id;
          return (
            <div
              key={message.messageId}
              className={`flex ${isCurrentUser ? "justify-end" : "justify-start"}`}
            >
              <div
                className={`flex gap-2 max-w-[70%] ${isCurrentUser ? "flex-row-reverse" : "flex-row"}`}
              >
                {!isCurrentUser && (
                  <Avatar src={message.senderProfilePhotoUrl} size={32} />
                )}
                <div
                  className={`flex flex-col ${isCurrentUser ? "items-end" : "items-start"}`}
                >
                  <p className="text-sm">{message.content}</p>
                  <div
                    className={`flex items-center gap-1 mt-1 ${isCurrentUser ? "flex-row-reverse" : "flex-row"}`}
                  >
                    <span className="text-xs text-slate-400">
                      {formatTime(message.sentAt)}
                    </span>
                  </div>
                </div>
              </div>
            </div>
          );
        })}
        <div ref={messagesEndRef} />
      </div>

      {/* Input */}
      <div className="p-4 border-t border-slate-500/30">
        <div className="flex items-end gap-2">
          <div className="flex-1">
            <TextArea
              value={newMessage}
              onChange={(e) => setNewMessage(e.target.value)}
              onKeyPress={handleKeyPress}
              placeholder="Type a message..."
              autoSize={{ minRows: 1, maxRows: 4 }}
              className="bg-slate-500/30 border-slate-400/50 text-white placeholder:text-slate-400 resize-none"
              style={{
                backgroundColor: "rgba(71, 85, 105, 0.3)",
                borderColor: "rgba(148, 163, 184, 0.5)",
                color: "white",
              }}
            />
          </div>

          <Button
            type="primary"
            icon={<SendOutlined />}
            onClick={handleSendMessage}
            disabled={!newMessage.trim()}
            className="bg-blue-500 hover:bg-blue-600 border-blue-500"
          />
        </div>
      </div>
    </div>
  );
}
