import { type FC, useEffect, useState } from "react";
import { App, Button, Empty, Modal, Popconfirm, Spin, Tooltip } from "antd";
import {
  DeleteOutlined,
  DownloadOutlined,
  EditOutlined,
  PlusOutlined,
} from "@ant-design/icons";
import { useAuth } from "../../../hooks/useAuth.ts";
import { applicationRole } from "../../../constants/role.ts";
import type { CourseResourceProps } from "../../../types/pages/courses/types.ts";
import { resourceService } from "../../../services/resource/resourceService.ts";
import courseService from "../../../services/course";
import CourseResourceModalSimple from "../../CourseResources/components/CourseResourceModalSimple";
import type { CourseResourceResponse } from "../../../types/ResourceType.ts";
import { downloadFile, getFileNameFromUrl } from "../../../utils/FileHelper.ts";
import type { NotificationProps } from "../../../types/Notification.ts";

export const CourseResourceDialog: FC<CourseResourceProps> = ({
  course,
  active,
  onClose,
}) => {
  const [resourceFormModalVisible, setResourceFormModalVisible] =
    useState(false);
  const [isEditing, setIsEditing] = useState(false);
  const { user } = useAuth();
  const [notify, setNotify] = useState<NotificationProps | null>(null);
  const [loading, setLoading] = useState(false);
  const { notification } = App.useApp();

  const [editingResource, setEditingResource] = useState<
    CourseResourceResponse | undefined
  >(undefined);

  const handleOpenAddForm = () => {
    setIsEditing(false);
    setEditingResource(undefined);
    setResourceFormModalVisible(true);
  };

  const handleOpenEditForm = (resource: CourseResourceResponse) => {
    setIsEditing(true);
    setEditingResource(resource);
    setResourceFormModalVisible(true);
  };

  const handleFormCancel = () => {
    setResourceFormModalVisible(false);
    setIsEditing(false);
    setEditingResource(undefined);
  };

  const handleFormSubmit = async (values: any) => {
    try {
      setLoading(true);
      const formData = new FormData();
      formData.append("title", values.title);
      formData.append("description", values.description || "");
      formData.append("resource", values.resource);
      formData.append("courseId", values.courseId);

      if (isEditing && editingResource) {
        await resourceService.editResource(editingResource.id, formData);
        setNotify({
          type: "success",
          message: "Resource updated",
          description: "The resource has been successfully updated.",
        });
      } else {
        await resourceService.createResource(formData);
        setNotify({
          type: "success",
          message: "Resource created",
          description: "The resource has been successfully created.",
        });
      }
      if (course) {
        const updatedCourse = await courseService.get(course.id);
        if (updatedCourse) {
          onClose("refresh");
        }
      }
      handleFormCancel();
    } catch (error: any) {
      setNotify({
        type: "error",
        message: "Failed to save resource",
        description: error?.response?.data?.error || "Error saving resource.",
      });
    } finally {
      setLoading(false);
    }
  };

  const handleDownload = (courseResourceId: string, filename: string) => {
    downloadFile(courseResourceId, filename);
  };

  const handleDeleteResource = async (resourceId: string) => {
    try {
      if (!course) return;
      setLoading(true);
      await resourceService.deleteResource(resourceId);
      setNotify({
        type: "success",
        message: "Resource deleted",
        description: "The resource has been successfully deleted.",
      });
      const updatedCourse = await courseService.get(course.id);
      if (updatedCourse) {
        onClose("refresh");
      }
    } catch (error: any) {
      setNotify({
        type: "error",
        message: "Failed to delete resource",
        description: error?.response?.data?.error || "Error deleting resource.",
      });
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (notify) {
      notification[notify.type]({
        message: notify.message,
        description: notify.description,
        placement: "topRight",
        showProgress: true,
        duration: 3,
        pauseOnHover: true,
      });
      setNotify(null);
    }
  }, [notify, notification]);

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
      <div className="mb-6">
        <div className="flex justify-between items-center mb-2">
          <p className="text-sm text-gray-400">Course Materials</p>
          {user?.role === applicationRole.MENTOR && (
            <Button
              type="primary"
              icon={<PlusOutlined />}
              onClick={handleOpenAddForm}
              className="bg-orange-500 hover:bg-orange-600"
              style={{ backgroundColor: "#f97316" }}
            >
              Add Material
            </Button>
          )}
        </div>
        {loading && <Spin tip="Loading..." size="large" />}
        {course.resources.length > 0 ? (
          <div className="space-y-2">
            {course.resources.map((item) => (
              <div
                key={item.id}
                className="bg-gray-700 p-3 rounded-md flex justify-between items-center"
              >
                <div className="flex-1 min-w-0 pr-4">
                  {" "}
                  <p
                    className="font-medium text-white truncate"
                    title={item.title}
                  >
                    {item.title}
                  </p>
                  <p
                    className="text-xs text-gray-400 truncate"
                    title={item.description}
                  >
                    {item.description}
                  </p>
                  <p className="text-xs text-gray-500 mt-1">
                    Type: {item.resourceType}
                  </p>
                </div>
                <div className="flex space-x-2 flex-shrink-0">
                  <Tooltip title="Download material">
                    <Button
                      type="primary"
                      icon={<DownloadOutlined />}
                      onClick={() =>
                        handleDownload(
                          item.id,
                          getFileNameFromUrl(item.resourceUrl),
                        )
                      }
                      className="bg-orange-500 hover:bg-orange-600"
                      style={{ backgroundColor: "#f97316" }}
                    />
                  </Tooltip>
                  {user?.role === applicationRole.MENTOR && (
                    <>
                      <Tooltip title="Edit material">
                        <Button
                          icon={<EditOutlined />}
                          onClick={() =>
                            handleOpenEditForm(item as CourseResourceResponse)
                          }
                        />
                      </Tooltip>
                      <Tooltip title="Delete Resource">
                        <Popconfirm
                          title="Are you sure to delete this resource?"
                          onConfirm={() => handleDeleteResource(item.id)}
                          okText="Yes"
                          cancelText="No"
                        >
                          <Button danger icon={<DeleteOutlined />}></Button>
                        </Popconfirm>
                      </Tooltip>
                    </>
                  )}
                </div>
              </div>
            ))}
          </div>
        ) : (
          <Empty
            description="No Data"
            image={Empty.PRESENTED_IMAGE_SIMPLE}
            className="py-4"
          />
        )}
      </div>

      <CourseResourceModalSimple
        visible={resourceFormModalVisible}
        isEditing={isEditing}
        initialValues={editingResource}
        course={course}
        onCancel={handleFormCancel}
        onSubmit={handleFormSubmit}
        title={isEditing ? "Edit Material" : "Add New Material"}
        onText={isEditing ? "Update" : "Add"}
      />
    </Modal>
  );
};
