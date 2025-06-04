import type { CategoryFilterCourse } from "../../../types/CategoryTypes";
import { Modal, Table, type TableColumnsType } from "antd";
import { formatDate } from "../../../utils/DateFormat";

export default function DisplayCourseModal({ visible, courses, onClose }: { visible: boolean; courses: CategoryFilterCourse[]; onClose: () => void }) {
  const courseColumns: TableColumnsType<CategoryFilterCourse> = [
    {
      title: 'Course Name',
      dataIndex: 'title',
      key: 'title',
      width: 300,
      ellipsis: true,
    },
    {
      title: 'Description',
      dataIndex: 'description',
      key: 'description',
      width: 400,
      ellipsis: true,
    },
    {
      title: 'Difficulty',
      dataIndex: 'difficulty',
      key: 'difficulty',
      width: 100,
    },
    {
      title: 'Status',
      dataIndex: 'status',
      key: 'status',
      width: 100,
    },
    {
      title: 'Due Date',
      dataIndex: 'dueDate',
      key: 'dueDate',
      render: (date: string) => formatDate(date),
      width: 100,
    },
  ];

  return (
    <Modal
      title="List Courses"
      open={visible}
      onCancel={onClose}
      footer={null}
      width={1200}
      height={300}
      centered
    >
      <Table
        columns={courseColumns}
        dataSource={courses}
        rowKey="id"
        pagination={false}
        size="small"
      />
    </Modal>
  );
};