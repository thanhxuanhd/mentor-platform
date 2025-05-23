import { type FC, useState } from "react";
import { App, Button } from "antd";
import { PlusOutlined } from "@ant-design/icons";
import CourseMaterialForm from "./CourseMaterialForm.tsx";
import { courseService } from "../../../services/course";
import type {CourseResourceProps} from "../../../types/pages/courses/types.ts";

export const CourseResource: FC<CourseResourceProps> = ({
  course,
  onDownload,
  active,
  onClose,
}) => {
  const [materialModalVisible, setMaterialModalVisible] = useState(false);
  const { message, modal } = App.useApp();
  const handleDeleteMaterial = async (materialId: string) => {
    if (!course) return;

    modal.confirm({
      title: "Delete Material",
      content: "Are you sure you want to delete this material?",
      okText: "Yes",
      okType: "danger",
      cancelText: "No",
      onOk: async () => {
        try {
          await courseService.deleteResource(course.id, materialId);
          // Since a successful deletion returns 204 No Content, we don't need to check the response
          message.success("Material deleted successfully");
          onClose("refresh");
        } catch (error) {
          console.error("Failed to delete material:", error);
          message.error("Failed to delete material");
        }
      },
    });
  };

  const close = () => {
    onClose();
  };

  if (!course || !active) {
    return null;
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
          <div className="flex justify-between items-center mb-2">
            <p className="text-sm text-gray-400">Course Materials</p>
            <Button
              type="primary"
              icon={<PlusOutlined />}
              onClick={() => setMaterialModalVisible(true)}
              className="bg-orange-500"
            >
              Add Material
            </Button>
          </div>
          {course.items.length > 0 ? (
            <div className="space-y-2">
              {course.items.map((item) => (
                <div
                  key={item.id}
                  className="bg-gray-700 p-3 rounded-md flex justify-between items-center"
                >
                  <div className="flex-1">
                    <p className="font-medium">{item.title}</p>
                    <p className="text-xs text-gray-400">{item.description}</p>
                    <p className="text-xs text-gray-500 mt-1">
                      Type: {item.mediaType}
                    </p>
                  </div>
                  <div className="flex space-x-2">
                    <Button
                      type="primary"
                      onClick={() => onDownload(item)}
                      className="bg-orange-500"
                    >
                      Download
                    </Button>
                    <Button
                      danger
                      onClick={() => handleDeleteMaterial(item.id)}
                    >
                      Delete
                    </Button>
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

        <CourseMaterialForm
          visible={materialModalVisible}
          courseId={course.id}
          onCancel={() => setMaterialModalVisible(false)}
          onSuccess={() => {
            setMaterialModalVisible(false);
            message.success("Material added successfully");
            onClose("refresh");
          }}
        />
      </div>
    </div>
  );
};
