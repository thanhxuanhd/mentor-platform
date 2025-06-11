import type { CourseDetailProps } from "../../../types/pages/courses/types.ts";
import type { FC } from "react";
import { Modal } from "antd";
import {formatDate} from "../../../utils/DateFormat.ts";

export const CourseDetail: FC<CourseDetailProps> = ({
  course,
  states,
  active,
  onClose,
}) => {
  if (!course || !active) {
    return null;
  }

  return (
    <Modal
      title={<span className="text-xl font-semibold">{course.title}</span>}
      open={active}
      onCancel={() => onClose()}
      footer={null}
      width={800}
      style={{ top: 50 }}
      centered
      className="antd-modal-dark-theme bg-gray-800 rounded-lg"
    >
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-6">
        <div>
          <p className="text-sm text-gray-400 mb-1">Category</p>
          <p>{course.categoryName}</p>
        </div>
        <div>
          <p className="text-sm text-gray-400 mb-1">Status</p>
          <p className="capitalize">
            {states[course.status] ?? course.status}
          </p>
        </div>
        <div>
          <p className="text-sm text-gray-400 mb-1">Due Date</p>
          <p>{formatDate(course.dueDate)}</p>
        </div>
        <div>
          <p className="text-sm text-gray-400 mb-1">Difficulty</p>
          <p className="capitalize">{course.difficulty}</p>
        </div>
        <div>
          <p className="text-sm text-gray-400 mb-1">Mentor</p>
          <p className="capitalize">{course.mentorName}</p>
        </div>
      </div>

      <div className="mb-6">
        <p className="text-sm text-gray-400 mb-1">Description</p>
        <p>{course.description}</p>
      </div>

      <div className="mb-6">
        <p className="text-sm text-gray-400 mb-1">Tags</p>
        <div className="flex flex-wrap gap-2">
          {course.tags && course.tags.length > 0 ? (
            course.tags.map((tag) => (
              <div key={tag} className="flex space-x-2 mt-2">
                <span className="bg-gray-700 px-2 py-1 rounded-md text-xs">
                  {tag}
                </span>
              </div>
            ))
          ) : (
            <span>-</span>
          )}
        </div>
      </div>
    </Modal>
  );
};
