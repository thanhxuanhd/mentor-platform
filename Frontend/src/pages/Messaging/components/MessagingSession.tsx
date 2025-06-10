import type React from "react";

import { useState, useRef, useEffect } from "react";
import { Avatar, Input, Button, Dropdown, Upload, Spin } from "antd";
import type { MenuProps } from "antd";
import {
  SendOutlined,
  SmileOutlined,
  PaperClipOutlined,
  MoreOutlined,
  PhoneOutlined,
  VideoCameraOutlined,
  InfoCircleOutlined,
  FileTextOutlined,
  DownloadOutlined,
  CheckOutlined,
  DoubleRightOutlined,
} from "@ant-design/icons";
import { useParams } from "react-router-dom";

const { TextArea } = Input;

interface Message {
  id: string;
  senderId: string;
  senderName: string;
  senderAvatar: string;
  content: string;
  timestamp: string;
  type: "text" | "image" | "file";
  fileName?: string;
  fileSize?: string;
  fileUrl?: string;
  status: "sending" | "sent" | "delivered" | "read";
}

interface User {
  id: string;
  name: string;
  avatar: string;
  role: "mentor" | "learner";
  status: "online" | "offline" | "away";
  lastSeen?: string;
}

// Mock data
const currentUser: User = {
  id: "user1",
  name: "Sarah Johnson",
  avatar: "/placeholder.svg?height=40&width=40",
  role: "mentor",
  status: "online",
};

const otherUser: User = {
  id: "user2",
  name: "Alex Chen",
  avatar: "/placeholder.svg?height=40&width=40",
  role: "learner",
  status: "online",
};

const initialMessages: Message[] = [
  {
    id: "1",
    senderId: "user2",
    senderName: "Alex Chen",
    senderAvatar: "/placeholder.svg?height=32&width=32",
    content:
      "Hi Sarah! I have a question about the Python assignment you gave us.",
    timestamp: "2023-12-05T10:30:00Z",
    type: "text",
    status: "read",
  },
  {
    id: "2",
    senderId: "user1",
    senderName: "Sarah Johnson",
    senderAvatar: "/placeholder.svg?height=32&width=32",
    content:
      "Hi Alex! Of course, I'd be happy to help. What specific part are you having trouble with?",
    timestamp: "2023-12-05T10:32:00Z",
    type: "text",
    status: "read",
  },
  {
    id: "3",
    senderId: "user2",
    senderName: "Alex Chen",
    senderAvatar: "/placeholder.svg?height=32&width=32",
    content:
      "I'm struggling with the data visualization part. Could you take a look at my code?",
    timestamp: "2023-12-05T10:35:00Z",
    type: "text",
    status: "read",
  },
  {
    id: "4",
    senderId: "user2",
    senderName: "Alex Chen",
    senderAvatar: "/placeholder.svg?height=32&width=32",
    content: "Here's my current code",
    timestamp: "2023-12-05T10:36:00Z",
    type: "file",
    fileName: "data_visualization.py",
    fileSize: "2.3 KB",
    status: "read",
  },
  {
    id: "5",
    senderId: "user1",
    senderName: "Sarah Johnson",
    senderAvatar: "/placeholder.svg?height=32&width=32",
    content:
      "Let me review this and get back to you with some suggestions. The structure looks good so far!",
    timestamp: "2023-12-05T10:40:00Z",
    type: "text",
    status: "delivered",
  },
];

export default function MessagingSession() {
  const { converId } = useParams();
  console.log(converId);
  const [messages, setMessages] = useState<Message[]>(initialMessages);
  const [newMessage, setNewMessage] = useState("");
  const [isTyping, setIsTyping] = useState(false);
  const [isOnline, setIsOnline] = useState(true);
  const messagesEndRef = useRef<HTMLDivElement>(null);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  };

  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  const handleSendMessage = () => {
    if (newMessage.trim()) {
      const message: Message = {
        id: Date.now().toString(),
        senderId: currentUser.id,
        senderName: currentUser.name,
        senderAvatar: currentUser.avatar,
        content: newMessage,
        timestamp: new Date().toISOString(),
        type: "text",
        status: "sending",
      };

      setMessages((prev) => [...prev, message]);
      setNewMessage("");

      // Simulate message delivery
      setTimeout(() => {
        setMessages((prev) =>
          prev.map((msg) =>
            msg.id === message.id ? { ...msg, status: "delivered" } : msg,
          ),
        );
      }, 1000);
    }
  };

  const handleKeyPress = (e: React.KeyboardEvent) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      handleSendMessage();
    }
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case "online":
        return "bg-green-500";
      case "away":
        return "bg-yellow-500";
      case "offline":
        return "bg-gray-500";
      default:
        return "bg-gray-500";
    }
  };

  const getMessageStatus = (status: string) => {
    switch (status) {
      case "sending":
        return <Spin size="small" />;
      case "sent":
        return <CheckOutlined className="text-gray-400 text-xs" />;
      case "delivered":
        return <DoubleRightOutlined className="text-gray-400 text-xs" />;
      case "read":
        return <DoubleRightOutlined className="text-blue-400 text-xs" />;
      default:
        return null;
    }
  };

  const formatTime = (timestamp: string) => {
    const date = new Date(timestamp);
    return date.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
  };

  const moreMenuItems: MenuProps["items"] = [
    {
      key: "call",
      label: (
        <div className="flex items-center gap-2 text-white">
          <PhoneOutlined />
          Voice Call
        </div>
      ),
    },
    {
      key: "video",
      label: (
        <div className="flex items-center gap-2 text-white">
          <VideoCameraOutlined />
          Video Call
        </div>
      ),
    },
    {
      key: "info",
      label: (
        <div className="flex items-center gap-2 text-white">
          <InfoCircleOutlined />
          View Profile
        </div>
      ),
    },
  ];

  return (
    <div className="bg-slate-600/50 backdrop-blur-sm rounded-xl border border-slate-500/30 shadow-xl h-[600px] flex flex-col">
      {/* Header */}
      <div className="flex items-center justify-between p-4 border-b border-slate-500/30">
        <div className="flex items-center gap-3">
          <div className="relative">
            <Avatar src={otherUser.avatar} size={40} />
            <div
              className={`absolute -bottom-1 -right-1 w-3 h-3 rounded-full border-2 border-slate-600 ${getStatusColor(otherUser.status)}`}
            />
          </div>
          <div>
            <h3 className="text-white font-semibold">{otherUser.name}</h3>
            <p className="text-slate-400 text-sm">
              {otherUser.status === "online"
                ? "Active now"
                : `Last seen ${otherUser.lastSeen || "recently"}`}
            </p>
          </div>
        </div>
        <div className="flex items-center gap-2">
          <Button
            type="text"
            icon={<PhoneOutlined />}
            className="text-white hover:bg-slate-500/50"
            size="large"
          />
          <Button
            type="text"
            icon={<VideoCameraOutlined />}
            className="text-white hover:bg-slate-500/50"
            size="large"
          />
          <Dropdown
            menu={{ items: moreMenuItems }}
            trigger={["click"]}
            placement="bottomRight"
            overlayClassName="messaging-dropdown"
          >
            <Button
              type="text"
              icon={<MoreOutlined />}
              className="text-white hover:bg-slate-500/50"
              size="large"
            />
          </Dropdown>
        </div>
      </div>

      {/* Messages */}
      <div className="flex-1 overflow-y-auto p-4 space-y-4">
        {messages.map((message) => {
          const isCurrentUser = message.senderId === currentUser.id;
          return (
            <div
              key={message.id}
              className={`flex ${isCurrentUser ? "justify-end" : "justify-start"}`}
            >
              <div
                className={`flex gap-2 max-w-[70%] ${isCurrentUser ? "flex-row-reverse" : "flex-row"}`}
              >
                {!isCurrentUser && (
                  <Avatar src={message.senderAvatar} size={32} />
                )}
                <div
                  className={`flex flex-col ${isCurrentUser ? "items-end" : "items-start"}`}
                >
                  <div
                    className={`px-4 py-2 rounded-2xl ${
                      isCurrentUser
                        ? "bg-blue-500 text-white"
                        : "bg-slate-500/50 text-white"
                    } ${message.type === "file" ? "border border-slate-400/30" : ""}`}
                  >
                    {message.type === "text" && (
                      <p className="text-sm">{message.content}</p>
                    )}
                    {message.type === "file" && (
                      <div className="flex items-center gap-3">
                        <FileTextOutlined className="text-lg" />
                        <div className="flex-1">
                          <p className="text-sm font-medium">
                            {message.fileName}
                          </p>
                          <p className="text-xs opacity-75">
                            {message.fileSize}
                          </p>
                        </div>
                        <Button
                          type="text"
                          icon={<DownloadOutlined />}
                          size="small"
                          className="text-white hover:bg-white/20"
                        />
                      </div>
                    )}
                    {message.type === "image" && (
                      <div className="relative">
                        <img
                          src={message.fileUrl || "/placeholder.svg"}
                          alt="Shared image"
                          className="max-w-full h-auto rounded-lg"
                        />
                        <Button
                          type="text"
                          icon={<DownloadOutlined />}
                          size="small"
                          className="absolute top-2 right-2 text-white bg-black/50 hover:bg-black/70"
                        />
                      </div>
                    )}
                  </div>
                  <div
                    className={`flex items-center gap-1 mt-1 ${isCurrentUser ? "flex-row-reverse" : "flex-row"}`}
                  >
                    <span className="text-xs text-slate-400">
                      {formatTime(message.timestamp)}
                    </span>
                    {isCurrentUser && getMessageStatus(message.status)}
                  </div>
                </div>
              </div>
            </div>
          );
        })}
        {isTyping && (
          <div className="flex justify-start">
            <div className="flex gap-2 max-w-[70%]">
              <Avatar src={otherUser.avatar} size={32} />
              <div className="bg-slate-500/50 px-4 py-2 rounded-2xl">
                <div className="flex gap-1">
                  <div className="w-2 h-2 bg-slate-400 rounded-full animate-bounce" />
                  <div
                    className="w-2 h-2 bg-slate-400 rounded-full animate-bounce"
                    style={{ animationDelay: "0.1s" }}
                  />
                  <div
                    className="w-2 h-2 bg-slate-400 rounded-full animate-bounce"
                    style={{ animationDelay: "0.2s" }}
                  />
                </div>
              </div>
            </div>
          </div>
        )}
        <div ref={messagesEndRef} />
      </div>

      {/* Input */}
      <div className="p-4 border-t border-slate-500/30">
        <div className="flex items-end gap-2">
          <Upload showUploadList={false}>
            <Button
              type="text"
              icon={<PaperClipOutlined />}
              className="text-slate-400 hover:text-white hover:bg-slate-500/50"
            />
          </Upload>
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
            type="text"
            icon={<SmileOutlined />}
            className="text-slate-400 hover:text-white hover:bg-slate-500/50"
          />
          <Button
            type="primary"
            icon={<SendOutlined />}
            onClick={handleSendMessage}
            disabled={!newMessage.trim()}
            className="bg-blue-500 hover:bg-blue-600 border-blue-500"
          />
        </div>
      </div>

      <style jsx global>{`
        .messaging-dropdown .ant-dropdown-menu {
          background-color: rgba(51, 65, 85, 0.95) !important;
          backdrop-filter: blur(8px);
          border: 1px solid rgba(148, 163, 184, 0.3) !important;
        }
        .messaging-dropdown .ant-dropdown-menu-item:hover {
          background-color: rgba(71, 85, 105, 0.7) !important;
        }
      `}</style>
    </div>
  );
}
