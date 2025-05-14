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

const myUsers: User[] = [
  {
    id: 1,
    name: "Alice Wonderland",
    email: "alice.wonderland@example.com",
    role: "Admin",
    joinedDate: "2023-01-15",
    status: "Active",
    lastActive: "2025-05-10T14:20:00Z",
  },
  {
    id: 2,
    name: "Bob The Builder",
    email: "bob.builder@example.com",
    role: "Mentor",
    joinedDate: "2023-02-20",
    status: "Active",
    lastActive: "2025-05-12T09:00:00Z",
  },
  {
    id: 3,
    name: "Charlie Brown",
    email: "charlie.brown@example.com",
    role: "Learner",
    joinedDate: "2024-03-10",
    status: "Pending",
    lastActive: "2024-03-10T09:15:00Z",
  },
  {
    id: 4,
    name: "Diana Prince",
    email: "diana.prince@example.com",
    role: "Mentor",
    joinedDate: "2022-11-05",
    status: "Deactivated",
    lastActive: "2024-01-20T10:00:00Z",
  },
  {
    id: 5,
    name: "Edward Scissorhands",
    email: "edward.s@example.com",
    role: "Learner",
    joinedDate: "2025-01-10",
    status: "Active",
    lastActive: "2025-05-01T11:30:00Z",
  },
  {
    id: 6,
    name: "Fiona Gallagher",
    email: "fiona.g@example.com",
    role: "Learner",
    joinedDate: "2023-06-01",
    status: "Active",
    lastActive: "2025-04-28T17:00:00Z",
  },
  {
    id: 7,
    name: "George Jetson",
    email: "george.jetson@example.com",
    role: "Admin",
    joinedDate: "2022-09-15",
    status: "Active",
    lastActive: "2025-05-13T10:00:00Z",
  },
  {
    id: 8,
    name: "Harry Potter",
    email: "harry.potter@example.com",
    role: "Learner",
    joinedDate: "2024-08-01",
    status: "Pending",
    lastActive: "2024-08-01T18:00:00Z",
  },
  {
    id: 9,
    name: "Irene Adler",
    email: "irene.adler@example.com",
    role: "Mentor",
    joinedDate: "2023-03-25",
    status: "Active",
    lastActive: "2025-03-15T12:45:00Z",
  },
  {
    id: 10,
    name: "Jack Sparrow",
    email: "jack.sparrow@example.com",
    role: "Learner",
    joinedDate: "2023-07-12",
    status: "Deactivated",
    lastActive: "2023-12-01T08:20:00Z",
  },
  {
    id: 11,
    name: "Kara Danvers",
    email: "kara.d@example.com",
    role: "Learner",
    joinedDate: "2024-10-05",
    status: "Active",
    lastActive: "2025-05-05T16:50:00Z",
  },
  {
    id: 12,
    name: "Leo Fitz",
    email: "leo.fitz@example.com",
    role: "Mentor",
    joinedDate: "2022-05-18",
    status: "Active",
    lastActive: "2025-04-30T13:00:00Z",
  },
  {
    id: 13,
    name: "Mulan Fa",
    email: "mulan.fa@example.com",
    role: "Learner",
    joinedDate: "2025-02-01",
    status: "Pending",
    lastActive: "2025-02-01T19:00:00Z",
  },
  {
    id: 14,
    name: "Neo Anderson",
    email: "neo.anderson@example.com",
    role: "Admin",
    joinedDate: "2021-12-01",
    status: "Active",
    lastActive: "2025-05-11T23:59:00Z",
  },
  {
    id: 15,
    name: "Olivia Pope",
    email: "olivia.pope@example.com",
    role: "Mentor",
    joinedDate: "2023-09-30",
    status: "Active",
    lastActive: "2025-05-09T14:30:00Z",
  },
  {
    id: 16,
    name: "Peter Pan",
    email: "peter.pan@example.com",
    role: "Learner",
    joinedDate: "2024-01-20",
    status: "Active",
    lastActive: "2025-03-22T18:00:00Z",
  },
  {
    id: 17,
    name: "Quinn Fabray",
    email: "quinn.f@example.com",
    role: "Learner",
    joinedDate: "2023-11-11",
    status: "Deactivated",
    lastActive: "2024-05-01T10:00:00Z",
  },
  {
    id: 18,
    name: "Rachel Green",
    email: "rachel.green@example.com",
    role: "Learner",
    joinedDate: "2024-04-01",
    status: "Active",
    lastActive: "2025-05-08T12:15:00Z",
  },
  {
    id: 19,
    name: "Steve Rogers",
    email: "steve.rogers@example.com",
    role: "Mentor",
    joinedDate: "2022-07-04",
    status: "Active",
    lastActive: "2025-05-02T11:00:00Z",
  },
  {
    id: 20,
    name: "Tony Stark",
    email: "tony.stark@example.com",
    role: "Admin",
    joinedDate: "2022-08-10",
    status: "Active",
    lastActive: "2025-05-12T18:00:00Z",
  },
  {
    id: 21,
    name: "Ursula Buffay",
    email: "ursula.b@example.com",
    role: "Learner",
    joinedDate: "2025-03-01",
    status: "Pending",
    lastActive: "2025-03-01T14:50:00Z",
  },
  {
    id: 22,
    name: "Victor Frankenstein",
    email: "victor.f@example.com",
    role: "Mentor",
    joinedDate: "2023-04-13",
    status: "Deactivated",
    lastActive: "2023-10-31T23:59:00Z",
  },
  {
    id: 23,
    name: "Wendy Darling",
    email: "wendy.darling@example.com",
    role: "Learner",
    joinedDate: "2024-06-25",
    status: "Active",
    lastActive: "2025-04-15T09:30:00Z",
  },
  {
    id: 24,
    name: "Xavier Charles",
    email: "xavier.charles@example.com",
    role: "Admin",
    joinedDate: "2023-01-01",
    status: "Active",
    lastActive: "2025-05-13T00:01:00Z",
  },
  {
    id: 25,
    name: "Yvonne Strahovski",
    email: "yvonne.s@example.com",
    role: "Mentor",
    joinedDate: "2022-10-10",
    status: "Active",
    lastActive: "2025-05-01T10:10:00Z",
  },
  {
    id: 21,
    name: "Ursula Buffay",
    email: "ursula.b@example.com",
    role: "Learner",
    joinedDate: "2025-03-01",
    status: "Pending",
    lastActive: "2025-03-01T14:50:00Z",
  },
  {
    id: 22,
    name: "Victor Frankenstein",
    email: "victor.f@example.com",
    role: "Mentor",
    joinedDate: "2023-04-13",
    status: "Deactivated",
    lastActive: "2023-10-31T23:59:00Z",
  },
  {
    id: 23,
    name: "Wendy Darling",
    email: "wendy.darling@example.com",
    role: "Learner",
    joinedDate: "2024-06-25",
    status: "Active",
    lastActive: "2025-04-15T09:30:00Z",
  },
  {
    id: 24,
    name: "Xavier Charles",
    email: "xavier.charles@example.com",
    role: "Admin",
    joinedDate: "2023-01-01",
    status: "Active",
    lastActive: "2025-05-13T00:01:00Z",
  },
  {
    id: 25,
    name: "Yvonne Strahovski",
    email: "yvonne.s@example.com",
    role: "Mentor",
    joinedDate: "2022-10-10",
    status: "Active",
    lastActive: "2025-05-01T10:10:00Z",
  },
  {
    id: 21,
    name: "Ursula Buffay",
    email: "ursula.b@example.com",
    role: "Learner",
    joinedDate: "2025-03-01",
    status: "Pending",
    lastActive: "2025-03-01T14:50:00Z",
  },
  {
    id: 22,
    name: "Victor Frankenstein",
    email: "victor.f@example.com",
    role: "Mentor",
    joinedDate: "2023-04-13",
    status: "Deactivated",
    lastActive: "2023-10-31T23:59:00Z",
  },
  {
    id: 23,
    name: "Wendy Darling",
    email: "wendy.darling@example.com",
    role: "Learner",
    joinedDate: "2024-06-25",
    status: "Active",
    lastActive: "2025-04-15T09:30:00Z",
  },
  {
    id: 24,
    name: "Xavier Charles",
    email: "xavier.charles@example.com",
    role: "Admin",
    joinedDate: "2023-01-01",
    status: "Active",
    lastActive: "2025-05-13T00:01:00Z",
  },
  {
    id: 25,
    name: "Yvonne Strahovski",
    email: "yvonne.s@example.com",
    role: "Mentor",
    joinedDate: "2022-10-10",
    status: "Active",
    lastActive: "2025-05-01T10:10:00Z",
  },
  {
    id: 21,
    name: "Ursula Buffay",
    email: "ursula.b@example.com",
    role: "Learner",
    joinedDate: "2025-03-01",
    status: "Pending",
    lastActive: "2025-03-01T14:50:00Z",
  },
  {
    id: 22,
    name: "Victor Frankenstein",
    email: "victor.f@example.com",
    role: "Mentor",
    joinedDate: "2023-04-13",
    status: "Deactivated",
    lastActive: "2023-10-31T23:59:00Z",
  },
  {
    id: 23,
    name: "Wendy Darling",
    email: "wendy.darling@example.com",
    role: "Learner",
    joinedDate: "2024-06-25",
    status: "Active",
    lastActive: "2025-04-15T09:30:00Z",
  },
  {
    id: 24,
    name: "Xavier Charles",
    email: "xavier.charles@example.com",
    role: "Admin",
    joinedDate: "2023-01-01",
    status: "Active",
    lastActive: "2025-05-13T00:01:00Z",
  },
  {
    id: 25,
    name: "Yvonne Strahovski",
    email: "yvonne.s@example.com",
    role: "Mentor",
    joinedDate: "2022-10-10",
    status: "Active",
    lastActive: "2025-05-01T10:10:00Z",
  },
];

const filterOptions = [
  {
    label: `All Users (${myUsers.length})`,
    value: "all",
  },
  {
    label: `Mentors (${myUsers.reduce(
      (count, user) => (user.role === "Mentor" ? count + 1 : count),
      0,
    )})`,
    value: "mentors",
  },
  {
    label: `Learners (${myUsers.reduce(
      (count, user) => (user.role === "Learner" ? count + 1 : count),
      0,
    )})`,
    value: "learners",
  },
];

const { Search } = Input;

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
      items: myUsers,
      totalCount: myUsers.length,
      pageSize: 5,
      pageIndex: 1,
      totalPages: Math.ceil(myUsers.length / 5),
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
