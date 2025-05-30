import type { Course } from "../types.tsx";
import { Button, Space, Table, type TableProps, Tag } from "antd";
import type { CourseTableProps } from "../../../types/pages/courses/types.ts";
import type { FC } from "react";
import { useAuth } from "../../../hooks/useAuth.ts";
import { applicationRole } from "../../../constants/role.ts";
import dayjs from "dayjs";
import {DeleteOutlined, FolderOutlined, EyeOutlined, EditOutlined} from "@ant-design/icons";

export const CourseTable: FC<CourseTableProps> = ({
  courses,
  states,
  onResourceView,
  onView,
  onEdit,
  onDelete,
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
            {dayjs(course.dueDate).format("YYYY-MM-DD")}
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
        </Space>
      ),
    },
    // {
    //   title: "",
    //   key: "action",
    //   render: (_: string, course: Course) => {
    //     const items: MenuProps["items"] = [
    //       {
    //         key: "viewResource",
    //         label: "View Resource",
    //         onClick: () => onResourceView(course),
    //       },
    //       {
    //         key: "viewCourse",
    //         label: "View Course Details",
    //         onClick: () => onView(course),
    //       },
    //       {
    //         key: "editCourse",
    //         label: "Edit Course",
    //         onClick: () => onEdit(course),
    //       },
    //       {
    //         key: "deleteCourse",
    //         label: "Delete Course",
    //         danger: true,
    //         onClick: () => onDelete(course),
    //       },
    //     ];
    //
    //     return (
    //       <Dropdown menu={{ items }} trigger={["click"]}>
    //         <Button icon={<EllipsisOutlined />} />
    //       </Dropdown>
    //     );
    //   },
    // },
  ];

  return (
    <Table<Course>
      dataSource={courses}
      columns={columns}
      rowKey="id"
      loading={tableProps.loading}
      pagination={tableProps.pagination}
      scroll={{x: "max-content"}}
    />
  );
};
