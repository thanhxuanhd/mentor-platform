import type { Course } from "./types.tsx";
import { CoursePopoverTarget } from "./coursePopoverTarget.tsx";
import type { FC } from "react";

export type CourseDetailProp = {
  course?: Course;
  states: Record<string, string>;
  active: boolean;
  onClose: (targetAction?: string | undefined) => void;
};

export const CourseDetail: FC<CourseDetailProp> = ({
  course,
  states,
  active,
  onClose,
}) => {
  const close = () => {
    onClose();
  };

  if (!course || !active) {
    return;
  }

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-gray-800 rounded-lg p-6 w-full max-w-3xl max-h-[90vh] overflow-y-auto">
        <div className="flex justify-between items-start mb-4">
          <h2 className="text-xl font-semibold">{course.title}</h2>
          <button
            onClick={() => close()}
            className="text-gray-400 hover:text-white"
          >
            ×
          </button>
        </div>

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
            <p>{course.dueDate}</p>
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
                <div className="flex space-x-2 mt-2">
                  <span
                    key={tag}
                    className="bg-gray-700 px-2 py-1 rounded-md text-xs"
                  >
                    {tag}
                  </span>
                </div>
              ))
            ) : (
              <span>-</span>
            )}
          </div>
        </div>

        <div className="flex justify-end space-x-3">
          <button
            onClick={() => close()}
            className="px-4 py-2 bg-gray-600 hover:bg-gray-500 text-white rounded-md transition duration-200"
          >
            Close
          </button>
        </div>
      </div>
    </div>
  );
};
