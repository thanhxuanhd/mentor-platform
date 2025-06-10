import { useCallback, useEffect, useState } from "react";
import { Avatar, App, Select } from "antd";
import { UserOutlined } from "@ant-design/icons";
import { Outlet, useNavigate } from "react-router-dom";
import type {
  GetFilterConversationResponse,
  GetMinimalConversationResponse,
} from "../../types/ChatType";
import { chatService } from "../../services/chat/chatService";
import type { NotificationProps } from "../../types/Notification";

export default function MessagingLayout() {
  const navigate = useNavigate();
  const { notification } = App.useApp();
  const [conversations, setConversations] = useState<
    GetMinimalConversationResponse[]
  >([]);
  const [selectedConversationId, setSelectedConversationId] = useState<
    string | null
  >(null);
  const [filteredContacts, setFilteredContacts] = useState<
    GetFilterConversationResponse[]
  >([]);
  const [notify, setNotify] = useState<NotificationProps | null>(null);

  const [searchKeyword, setSearchKeyword] = useState<string | null>(null);
  const [pageIndex, setPageIndex] = useState(1);
  const [filterPageIndex, setFilterPageIndex] = useState(1);

  const fetchFilteredContacts = useCallback(async () => {
    try {
      await chatService
        .filterConversations({
          keyword: searchKeyword,
          pageIndex: filterPageIndex,
        })
        .then((response) => {
          setFilteredContacts([...filteredContacts, ...response]);
          console.log(filteredContacts);
        });
    } catch (error: any) {
      setNotify({
        type: "error",
        message: "Failed to fetch conversations",
        description:
          error?.response?.data?.error || "Error fetching conversations.",
      });
    }
  }, [filterPageIndex, searchKeyword]);

  const fetchConversations = useCallback(async () => {
    try {
      await chatService.getConversations(pageIndex).then((response) => {
        setConversations(response.items);
      });
    } catch (error: any) {
      setNotify({
        type: "error",
        message: "Failed to fetch conversations",
        description:
          error?.response?.data?.error || "Error fetching conversations.",
      });
    }
  }, [pageIndex]);

  useEffect(() => {
    if (selectedConversationId) navigate(`${selectedConversationId}`);
  }, [selectedConversationId]);

  useEffect(() => {
    fetchConversations();
    fetchFilteredContacts();
  }, [fetchConversations, fetchFilteredContacts]);

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

  function handleFilterScroll(e: React.UIEvent<HTMLDivElement>) {
    const element = e.target as HTMLElement | null;
    if (
      element &&
      element.scrollHeight === element.clientHeight + element.scrollTop
    ) {
      setFilterPageIndex(filterPageIndex + 1);
    }
  }

  function handleFilterContactSearch(value: string) {
    setSearchKeyword(value);
    setFilterPageIndex(1);
  }

  function handleFilteredContactChange(
    value: string,
    option?:
      | GetFilterConversationResponse
      | GetFilterConversationResponse[]
      | undefined,
  ) {
    if (option && !Array.isArray(option)) {
      navigate(`${option.conversationId}`, {
        state: {
          contactId: value,
          contactName: option.name,
          contactPhotoUrl: option.photoUrl,
        },
      });
    }
  }

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
            <Select
              showSearch
              allowClear
              placeholder="Select conversations"
              size="large"
              className="w-full"
              options={filteredContacts}
              optionRender={(option) => (
                <div className="p-1">
                  {option.data.photoUrl ? (
                    <Avatar src={option.data.photoUrl} size={48} />
                  ) : (
                    <Avatar size={48} icon={<UserOutlined />} />
                  )}

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
                className={`p-4 border-b border-slate-500/20 cursor-pointer transition-colors hover:bg-slate-500/30 ${
                  selectedConversationId === conversation.conversationId
                    ? "bg-slate-500/40"
                    : ""
                }`}
                onClick={() => {
                  setSelectedConversationId(conversation.conversationId);
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
                      <span className="text-slate-400 text-xs">
                        {conversation.lastMessage.sentAt}
                      </span>
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

        <div className="lg:col-span-2">
          <Outlet />
        </div>
      </div>
    </div>
  );
}
