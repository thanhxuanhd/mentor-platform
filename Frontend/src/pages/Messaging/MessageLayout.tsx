import { useState } from "react";
import { Avatar, Input, Badge } from "antd";
import { SearchOutlined } from "@ant-design/icons";
import { Outlet, useNavigate } from "react-router-dom";

interface Conversation {
  id: number | null;
  user: {
    name: string;
    avatar: string;
    status: "online" | "offline" | "away";
  };
  lastMessage: string;
  timestamp: string;
  unreadCount: number;
  isActive?: boolean;
}

const conversations: Conversation[] = [
  {
    id: 1,
    user: {
      name: "Alex Chen",
      avatar: "/placeholder.svg?height=40&width=40",
      status: "online",
    },
    lastMessage: "I'm struggling with the data visualization part...",
    timestamp: "10:36 AM",
    unreadCount: 0,
    isActive: true,
  },
  {
    id: 2,
    user: {
      name: "Maria Rodriguez",
      avatar: "/placeholder.svg?height=40&width=40",
      status: "away",
    },
    lastMessage: "Thank you for the feedback on my project!",
    timestamp: "Yesterday",
    unreadCount: 2,
  },
  {
    id: 3,
    user: {
      name: "James Wilson",
      avatar: "/placeholder.svg?height=40&width=40",
      status: "offline",
    },
    lastMessage: "When is our next session scheduled?",
    timestamp: "Monday",
    unreadCount: 0,
  },
  {
    id: 4,
    user: {
      name: "Emily Davis",
      avatar: "/placeholder.svg?height=40&width=40",
      status: "online",
    },
    lastMessage: "I've completed the assignment you gave me",
    timestamp: "Sunday",
    unreadCount: 1,
  },
];

export default function MessagingLayout() {
  const [selectedConversation, setSelectedConversation] = useState<
    number | null
  >(null);
  const navigate = useNavigate();
  const [searchQuery, setSearchQuery] = useState("");

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

  const filteredConversations = conversations.filter((conv) =>
    conv.user.name.toLowerCase().includes(searchQuery.toLowerCase()),
  );

  return (
    <div className="bg-gray-800 rounded-lg overflow-hidden shadow-lg p-6">
      <div className="mb-6">
        <h2 className="text-2xl font-semibold">Messages</h2>
        <p className="text-slate-300">Connect with your mentors and learners</p>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 h-[600px]">
        {/* Conversations List */}
        <div className="bg-slate-600/50 backdrop-blur-sm rounded-xl border border-slate-500/30 shadow-xl">
          <div className="p-4 border-b border-slate-500/30">
            <Input
              placeholder="Search conversations..."
              prefix={<SearchOutlined className="text-slate-400" />}
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              className="bg-slate-500/30 border-slate-400/50 text-white placeholder:text-slate-400"
              style={{
                backgroundColor: "rgba(71, 85, 105, 0.3)",
                borderColor: "rgba(148, 163, 184, 0.5)",
                color: "white",
              }}
            />
          </div>
          <div className="overflow-y-auto h-[calc(600px-80px)]">
            {filteredConversations.map((conversation) => (
              <div
                key={conversation.id}
                className={`p-4 border-b border-slate-500/20 cursor-pointer transition-colors hover:bg-slate-500/30 ${
                  selectedConversation === conversation.id
                    ? "bg-slate-500/40"
                    : ""
                }`}
                onClick={() => {
                  navigate(`conversation/${conversation.id}`);
                  setSelectedConversation(conversation.id);
                }}
              >
                <div className="flex items-center gap-3">
                  <div className="relative">
                    <Avatar src={conversation.user.avatar} size={48} />
                    <div
                      className={`absolute -bottom-1 -right-1 w-3 h-3 rounded-full border-2 border-slate-600 ${getStatusColor(
                        conversation.user.status,
                      )}`}
                    />
                  </div>
                  <div className="flex-1 min-w-0">
                    <div className="flex items-center justify-between mb-1">
                      <h4 className="text-white font-medium truncate">
                        {conversation.user.name}
                      </h4>
                      <span className="text-slate-400 text-xs">
                        {conversation.timestamp}
                      </span>
                    </div>
                    <div className="flex items-center justify-between">
                      <p className="text-slate-300 text-sm truncate flex-1">
                        {conversation.lastMessage}
                      </p>
                      {conversation.unreadCount > 0 && (
                        <Badge
                          count={conversation.unreadCount}
                          size="small"
                          className="ml-2"
                        />
                      )}
                    </div>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>

        <div className="lg:col-span-2">
          <Outlet />
        </div>
      </div>
    </div>
  );
}
