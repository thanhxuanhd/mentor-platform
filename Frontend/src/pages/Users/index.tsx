import {
  App,
  Button,
  Input,
  Segmented,
  Space,
  Table,
  Tag,
  type GetProps,
} from "antd";
import { useCallback, useEffect, useState } from "react";
import type { EditUserRequest, GetUserResponse } from "../../types/UserTypes";
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
import { userService } from "../../services/user/userService";
import type { NotificationProps } from "../../types/Notification";

type SearchProps = GetProps<typeof Input.Search>;

const { Search } = Input;

const roleOptions = [
  { label: "All", value: "" },
  { label: "Admin", value: "Admin" },
  { label: "Mentor", value: "Mentor" },
  { label: "Learner", value: "Learner" },
];

export default function UsersPage() {
  const [usersData, setUsersData] = useState<GetUserResponse[]>([]);
  const [pageIndex, setPageIndex] = useState(1);
  const [pageSize, setPageSize] = useState(5);
  const [totalCount, setTotalCount] = useState(0);
  const [selectedRoleSegment, setSelectedRoleSegment] = useState(
    roleOptions[0].value,
  );
  const [searchValue, setSearchValue] = useState("");
  const [notify, setNotify] = useState<NotificationProps | null>(null);
  const [loading, setLoading] = useState(false);

  const [isModalVisible, setIsModalVisible] = useState(false);
  const [editingUser, setEditingUser] = useState<EditUserRequest>({
    id: "",
    fullName: "",
    email: "",
    roleId: 1,
  });

  const { notification } = App.useApp();

  const columns: ColumnsType<GetUserResponse> = [
    {
      title: "User",
      width: "30%",
      dataIndex: "userInfo",
      ellipsis: true,
      key: "userInfo",
      render: (_, record) => (
        <div className="truncate">
          <div className="font-medium">{record.fullName}</div>
          <div className="text-gray-500 text-sm">{record.email}</div>
        </div>
      ),
    },
    {
      title: "Role",
      dataIndex: "role",
      key: "role",
      render: (_, { roleId }) => (
        <Tag color={roleId === 1 ? "pink" : roleId === 2 ? "cyan" : "lime"}>
          {roleId === 1 ? "Admin" : roleId === 2 ? "Mentor" : "Learner"}
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
        <Tag color={status === 1 ? "lime" : status === 2 ? "red" : "orange"}>
          {status === 1 ? "Active" : status === 2 ? "Inactive" : "Pending"}
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
            onClick={() => handleEditClick(record)}
          />
          <Button
            icon={
              record.status === 1 ? (
                <StopOutlined type="default" style={{ color: "red" }} />
              ) : (
                <CheckOutlined type="default" style={{ color: "lime" }} />
              )
            }
            onClick={() => handleChangeStatus(record.id)}
          />
          <Button color="cyan" icon={<MessageOutlined />} />
        </Space>
      ),
    },
  ];

  const fetchUsers = useCallback(async () => {
    try {
      setLoading(true);
      const response = await userService.getUsers({
        pageIndex: pageIndex,
        pageSize: pageSize,
        roleName: selectedRoleSegment,
        fullName: searchValue,
      });
      setTotalCount(response.totalCount);
      setUsersData(response.items);
    } catch {
      setNotify({
        type: "error",
        message: "Error",
        description: "An error occurred while fetching users.",
      });
    } finally {
      setLoading(false);
    }
  }, [pageIndex, pageSize, searchValue, selectedRoleSegment]);

  useEffect(() => {
    fetchUsers();
  }, [fetchUsers]);

  useEffect(() => {
    if (notify) {
      notification[notify.type]({
        message: notify.message,
        description: notify.description,
        placement: "topRight",
      });
      setNotify(null);
    }
  }, [notify, notification]);

  const handleEditClick = (user: GetUserResponse) => {
    setEditingUser({
      id: user.id,
      fullName: user.fullName,
      email: user.email,
      roleId: user.roleId,
    });
    setIsModalVisible(true);
  };

  const handleCancel = () => setIsModalVisible(false);

  const handleSubmit = useCallback(
    async (updatedUser: EditUserRequest) => {
      try {
        setLoading(true);
        await userService.updateUser(updatedUser.id, updatedUser);
        setPageIndex(1);
        fetchUsers();
        setIsModalVisible(false);
        setNotify({
          type: "success",
          message: "Success",
          description: "User updated successfully.",
        });
      } catch {
        setNotify({
          type: "error",
          message: "Error",
          description: "An error occurred while updating user.",
        });
      } finally {
        setLoading(false);
      }
    },
    [fetchUsers],
  );

  const handleSearchInput: SearchProps["onSearch"] = useCallback(
    (value: string) => {
      setSearchValue(value);
    },
    [],
  );

  const handlePageChange = useCallback((newPage: number) => {
    setPageIndex(newPage);
  }, []);

  const handlePageSizeChange = useCallback((newPageSize: number) => {
    setPageSize(newPageSize);
    setPageIndex(1);
  }, []);

  const handleFilterChange = useCallback((value: string) => {
    setSelectedRoleSegment(value);
    setPageIndex(1);
  }, []);

  const handleChangeStatus = useCallback(
    async (userId: string) => {
      try {
        const user = usersData.find((user) => user.id === userId);
        if (!user) return;

        const newStatus = user.status === 1 ? 2 : 1;
        await userService.changeUserStatus(userId);

        setUsersData((prev) =>
          prev.map((u) => (u.id === userId ? { ...u, status: newStatus } : u)),
        );

        setNotify({
          type: "success",
          message: "Success",
          description: `User status changed to ${
            newStatus === 1 ? "Active" : "Inactive"
          }.`,
        });
      } catch {
        setNotify({
          type: "error",
          message: "Error",
          description: "An error occurred while changing user status.",
        });
      }
    },
    [usersData],
  );

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
            onChange={(e) => {
              if (e.target.value === "") {
                setSearchValue("");
              }
            }}
          />
        </div>
      </div>

      <div className="p-1 mb-6">
        <Segmented<string>
          options={roleOptions}
          size="large"
          style={{ fontSize: "10px" }}
          value={selectedRoleSegment}
          onChange={handleFilterChange}
        />
      </div>

      <Table<GetUserResponse>
        pagination={false}
        columns={columns}
        dataSource={usersData}
        className="mb-6"
        rowKey="id"
        loading={loading}
        scroll={{ x: true }}
      />
      <PaginationControls
        pageIndex={pageIndex}
        pageSize={pageSize}
        totalCount={totalCount}
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
