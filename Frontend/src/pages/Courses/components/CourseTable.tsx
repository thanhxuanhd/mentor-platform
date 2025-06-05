import type { Course } from "../types.tsx";
import { Button, Space, Table, type TableProps, Tag } from "antd";
import type { CourseTableProps } from "../../../types/pages/courses/types.ts";
import type { FC } from "react";
import { useAuth } from "../../../hooks/useAuth.ts";
import { applicationRole } from "../../../constants/role.ts";
import {
  CheckOutlined,
  DeleteOutlined,
  EditOutlined,
  EyeOutlined,
  FolderOutlined,
  InboxOutlined,
} from "@ant-design/icons";
import { formatDate } from "../../../utils/DateFormat.ts";

export const CourseTable: FC<CourseTableProps> = ({
  courses,
  states,
  onResourceView,
  onView,
  onEdit,
  onDelete,
  onPublish,
  onArchive,
  tableProps,
}) => {
  const { user } = useAuth();
  const columns: TableProps<Course>["columns"] = [
    {
      title: "Title",
      dataIndex: "title",
      key: "title",
      render: (_: string, course: Course) => (
        <div>
          <div className="font-medium">{course.title}</div>
          <div className="text-xs text-gray-400">
            {formatDate(course.dueDate)}
          </div>
          <div className="text-xs text-gray-400">{course.difficulty}</div>
        </div>
      ),
    },
    {
      title: "Category",
      dataIndex: "categoryName",
      key: "categoryName",
    },
    {
      title: "Status",
      dataIndex: "status",
      key: "status",
      render: (_: string, record: Course) => (
        <Tag
          color={
            record.status === "published"
              ? "success"
              : record.status === "draft"
                ? "warning"
                : "default"
          }
        >
          {states[record.status] ?? record.status}
        </Tag>
      ),
    },
    {
      title: "Mentor",
      dataIndex: "mentorName",
      key: "mentorName",
    },
    {
      title: "Difficulty",
      dataIndex: "difficulty",
      key: "difficulty",
    },
    {
      title: "Action",
      key: "action",
      render: (_: string, course: Course) => (
        <Space>
          <Button
            icon={<FolderOutlined />}
            onClick={() => onResourceView(course)}
          />
          <Button icon={<EyeOutlined />} onClick={() => onView(course)} />
          {user?.role === applicationRole.MENTOR && (
            <>
              <Button
                type="primary"
                icon={<EditOutlined />}
                onClick={() => onEdit(course)}
              />
              <Button
                danger
                icon={<DeleteOutlined />}
                onClick={() => onDelete(course)}
              />
            </>
          )}
          {user?.role === applicationRole.ADMIN && (
            <>
              {course.status.toLowerCase() === "draft" && (
                <Button
                  type="primary"
                  icon={<CheckOutlined />}
                  onClick={() => onPublish(course)}
                />
              )}
              {course.status.toLowerCase() === "published" && (
                <Button
                  icon={<InboxOutlined />}
                  onClick={() => onArchive(course)}
                />
              )}
            </>
          )}
        </Space>
      ),
    },
  ];

  return (
    <Table<Course>
      dataSource={courses}
      columns={columns}
      rowKey="id"
      loading={tableProps.loading}
      pagination={tableProps.pagination}
      scroll={{ x: "max-content" }}
    />
  );
};
