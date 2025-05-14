import {
  Button,
  Input,
  Segmented,
  Space,
  Table,
  Tag,
  type GetProps,
} from "antd";
import { useEffect, useState } from "react";
import type { User } from "../../types/UserTypes";
import type { PaginatedList } from "../../types/Pagination";
import type { ColumnsType } from "antd/es/table";
import PaginationControls from "../../components/shared/Pagination";
import {
  CheckOutlined,
  EditFilled,
  MessageOutlined,
  StopOutlined,
} from "@ant-design/icons";
import { formatDate } from "../../utils/DateFormat";
import EditUserModal from "./components/EditUserModal";
type SearchProps = GetProps<typeof Input.Search>;

const { Search } = Input;

const filterOptions = [
  { label: "All", value: "all" },
  { label: "Mentor", value: "mentor" },
  { label: "Learner", value: "learner" },
];

export default function UsersPage() {
  const [usersData, setUsersData] = useState<PaginatedList<User>>({
    items: [],
    totalCount: 0,
    pageSize: 5,
    pageIndex: 1,
    totalPages: 0,
  });
  const [selectedFilter, setSelectedFilter] = useState(filterOptions[0].value);
  const [searchValue, setSearchValue] = useState("");
  const [pageSize, setPageSize] = useState(5);
  const [pageIndex, setPageIndex] = useState(1);
  const [isModalVisible, setIsModalVisible] = useState(false);
  const [editingUser, setEditingUser] = useState<User>({
    id: 1,
    name: "",
    email: "",
    role: "Admin",
    joinedDate: "",
    status: "Active",
    lastActive: "",
  });

  const columns: ColumnsType<User> = [
    {
      title: "User",
      width: "30%",
      dataIndex: "userInfo",
      ellipsis: true,
      key: "userInfo",
      render: (_, record) => (
        <div className="truncate">
          <div className="font-medium">{record.name}</div>
          <div className="text-gray-500 text-sm">{record.email}</div>
        </div>
      ),
    },
    {
      title: "Role",
      dataIndex: "role",
      key: "role",
      render: (_, { role }) => (
        <Tag
          color={
            role === "Admin" ? "pink" : role === "Mentor" ? "cyan" : "lime"
          }
        >
          {role}
        </Tag>
      ),
    },
    {
      title: "Joined Date",
      dataIndex: "joinedDate",
      key: "joinedDate",
      render: (_, { joinedDate }) => <div>{formatDate(joinedDate)}</div>,
    },
    {
      title: "Status",
      dataIndex: "status",
      key: "status",
      render: (_, { status }) => (
        <Tag
          color={
            status === "Active"
              ? "lime"
              : status === "Deactivated"
                ? "red"
                : "orange"
          }
        >
          {status}
        </Tag>
      ),
    },
    {
      title: "Last Active",
      dataIndex: "lastActive",
      key: "lastActive",
      render: (_, { lastActive }) => <div>{formatDate(lastActive)}</div>,
    },
    {
      title: "Action",
      key: "action",
      align: "right",
      render: (_, record) => (
        <Space size="middle">
          <Button
            type="primary"
            icon={<EditFilled />}
            onClick={handleEditClick}
          />
          <Button
            icon={
              record.status == "Active" ? (
                <StopOutlined type="default" style={{ color: "red" }} />
              ) : (
                <CheckOutlined type="default" style={{ color: "lime" }} />
              )
            }
          />
          <Button color="cyan" icon={<MessageOutlined />} />
        </Space>
      ),
    },
  ];

  const handleEditClick = () => setIsModalVisible(true);

  const handleCancel = () => setIsModalVisible(false);

  const handleSubmit = (updatedUser: User) => {
    console.log("Updated user:", updatedUser);
    setEditingUser(updatedUser);
    setIsModalVisible(false);
  };

  const fetchUsers = async () => {
    // Simulate an API call
    setUsersData({
      items: usersData.items,
      totalCount: usersData.totalCount,
      pageSize: usersData.pageSize,
      pageIndex: usersData.pageIndex,
      totalPages: usersData.totalPages,
    });
  };

  useEffect(() => {
    fetchUsers();
  }, []);

  const handleSearchInput: SearchProps["onSearch"] = (value, _e, info) => {
    throw new Error("Function not implemented.");
  };
  const handlePageChange = (page: number): void => {
    throw new Error("Function not implemented.");
  };

  const handlePageSizeChange = (pageSize: number): void => {
    throw new Error("Function not implemented.");
  };

  const handleFilterChange = (value: string): void => {
    setSelectedFilter(value);
  };

  return (
    <div className="bg-gray-800 rounded-lg overflow-hidden shadow-lg p-6">
      <div className="flex justify-between items-center mb-6">
        <h2 className="text-2xl font-semibold">User Management</h2>
        <div className="relative w-64">
          <Search
            placeholder="Search user..."
            allowClear
            size="large"
            enterButton
            onSearch={handleSearchInput}
          />
        </div>
      </div>

      <div className="p-1 mb-6">
        <Segmented<string>
          options={filterOptions}
          size="large"
          style={{ fontSize: "10px" }}
          value={selectedFilter}
          onChange={handleFilterChange}
        />
      </div>

      <Table<User>
        pagination={false}
        columns={columns}
        dataSource={usersData.items}
        className="mb-6"
        rowKey="id"
      />
      <PaginationControls<User>
        pagination={usersData}
        onPageChange={handlePageChange}
        onPageSizeChange={handlePageSizeChange}
      />
      <EditUserModal
        visible={isModalVisible}
        initialValues={editingUser}
        onCancel={handleCancel}
        onSubmit={handleSubmit}
      />
    </div>
  );
}
