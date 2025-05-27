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
    },
    {
      title: 'Difficulty',
      dataIndex: 'difficulty',
      key: 'difficulty',
      width: 200,
    },
    {
      title: 'Status',
      dataIndex: 'status',
      key: 'status',
      width: 200,
    },
    {
      title: 'Due Date',
      dataIndex: 'dueDate',
      key: 'dueDate',
      render: (date: string) => formatDate(date),
      width: 200,
    },
  ];

  return (
    <Modal
      title="List Courses"
      open={visible}
      onCancel={onClose}
      footer={null}
      width={800}
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