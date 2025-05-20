import { type FC } from "react";
import type { Course, CourseItem } from "./types.tsx";

export type CourseResourceProps = {
  course?: Course;
  onDownload: (material: CourseItem) => void;
  active: boolean;
  onClose: (targetAction?: string | undefined) => void;
};

export const CourseResource: FC<CourseResourceProps> = ({
  course,
  onDownload,
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

        <div className="mb-6">
          <p className="text-sm text-gray-400 mb-2">Course Materials</p>
          {course.items.length > 0 ? (
            <div className="space-y-2">
              {course.items.map((item) => (
                <div
                  key={item.id}
                  className="bg-gray-700 p-3 rounded-md flex justify-between items-center"
                >
                  <div>
                    <p className="font-medium">{item.title}</p>
                    <p className="text-xs text-gray-400">
                      {item.description}
                    </p>
                  </div>
                  <div className="mt-4 flex space-x-2">
                    <button
                      onClick={() => onDownload(item)}
                      className="bg-orange-500 hover:bg-orange-600 text-white px-3 py-1 rounded text-sm transition duration-200"
                    >
                      Download
                    </button>
                    {/*<button*/}
                    {/*  onClick={() => close()}*/}
                    {/*  className="bg-gray-600 hover:bg-gray-500 text-white px-3 py-1 rounded text-sm transition duration-200"*/}
                    {/*>*/}
                    {/*  Edit*/}
                    {/*</button>*/}
                    {/*<button*/}
                    {/*  onClick={() => close()}*/}
                    {/*  className="bg-red-600 hover:bg-red-700 text-white px-3 py-1 rounded text-sm transition duration-200"*/}
                    {/*>*/}
                    {/*  Delete*/}
                    {/*</button>*/}
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <p className="text-gray-400">
              No materials available for this course.
            </p>
          )}
        </div>
      </div>
    </div>
  );
};
