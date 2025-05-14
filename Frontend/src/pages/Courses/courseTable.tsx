import type { Course } from "./types.tsx";
import { use } from "react";

type CourseTableProp = {
  coursesPromise: Promise<Course[]>;
  states: Record<string, string>;
  onResourceView: (course: Course) => void;
  onView: (course: Course) => void;
  onEdit: (course: Course) => void;
  onDelete: (course: Course) => void;
};

export const CourseTable = ({
  coursesPromise,
  states,
  onResourceView,
  onView,
  onEdit,
  onDelete,
}: CourseTableProp) => {
  const courses = use(coursesPromise);
  return (
    <div className="overflow-x-auto">
      <table className="min-w-full bg-gray-700 rounded-lg overflow-hidden">
        <thead>
          <tr className="bg-gray-600">
            <th className="py-3 px-4 text-left text-sm font-medium">Title</th>
            <th className="py-3 px-4 text-left text-sm font-medium">
              Category
            </th>
            <th className="py-3 px-4 text-left text-sm font-medium">Status</th>
            <th className="py-3 px-4 text-left text-sm font-medium">
              Students
            </th>
            <th className="py-3 px-4 text-left text-sm font-medium">
              Completion
            </th>
            <th className="py-3 px-4 text-left text-sm font-medium">Actions</th>
          </tr>
        </thead>
        <tbody className="divide-y divide-gray-600">
          {courses.map((course) => (
            <tr key={course.id} className="hover:bg-gray-650">
              <td className="py-3 px-4">
                <div className="font-medium">{course.title}</div>
                <div className="text-xs text-gray-400">{course.dueDate}</div>
                <div className="text-xs text-gray-400">{course.difficulty}</div>
              </td>
              <td className="py-3 px-4">{course.categoryName}</td>
              <td className="py-3 px-4">
                <span
                  className={`px-2 py-1 text-xs rounded-full ${course.status === "published" ? "bg-green-500 bg-opacity-20 text-green-300" : course.status === "draft" ? "bg-yellow-500 bg-opacity-20 text-yellow-300" : "bg-gray-500 bg-opacity-20 text-gray-300"}`}
                >
                  {states[course.status] ?? course.status}
                </span>
              </td>
              <td className="py-3 px-4">{course.enrolledStudents}</td>
              <td className="py-3 px-4">{course.completionRate}%</td>
              <td className="py-3 px-4">
                <div className="flex space-x-2">
                  <button
                    onClick={() => onResourceView(course)}
                    className="px-3 py-1 bg-yellow-500 hover:bg-yellow-600 text-white rounded-md text-xs transition duration-200"
                  >
                    Resources
                  </button>
                  <button
                    onClick={() => onView(course)}
                    className="px-3 py-1 bg-blue-500 hover:bg-blue-600 text-white rounded-md text-xs transition duration-200"
                  >
                    View
                  </button>
                  <button
                    onClick={() => onEdit(course)}
                    className="px-3 py-1 bg-orange-500 hover:bg-orange-600 text-white rounded-md text-xs transition duration-200"
                  >
                    Edit
                  </button>
                  <button
                    onClick={() => onDelete(course)}
                    className="px-3 py-1 bg-red-500 hover:bg-red-600 text-white rounded-md text-xs transition duration-200"
                  >
                    Delete
                  </button>
                </div>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};
