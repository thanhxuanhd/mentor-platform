import type { Course } from "../types.tsx";
import type { TableProps } from "antd";
import { Button, Space, Table, Tag } from "antd";
import {
  DeleteOutlined,
  EditOutlined,
  EyeOutlined,
  FolderOutlined,
} from "@ant-design/icons";
import type { CourseTableProps } from "../../../types/pages/courses/types.ts";
import type { FC } from "react";

export const CourseTable: FC<CourseTableProps> = ({
  courses,
  states,
  onResourceView,
  onView,
  onEdit,
  onDelete,
  tableProps,
}) => {
  const columns: TableProps<Course>["columns"] = [
    {
      title: "Title",
      dataIndex: "title",
      key: "title",
      width: "25%",
      render: (_: string, course: Course) => (
        <div>
          <div className="font-medium">{course.title}</div>
          <div className="text-xs text-gray-400">{course.dueDate}</div>
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
    // {
    //   title: "Students",
    //   dataIndex: "enrolledStudents",
    //   key: "enrolledStudents",
    // },
    // {
    //   title: "Completion",
    //   dataIndex: "completionRate",
    //   key: "completionRate",
    //   render: (text: number) => `${text}%`,
    // },
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
    />
  );
};
