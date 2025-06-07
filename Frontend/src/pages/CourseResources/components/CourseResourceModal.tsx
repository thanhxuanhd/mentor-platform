import { App, Button, Form, Input, Modal, Select, Upload } from "antd";
import type { UploadFile } from "antd/es/upload/interface";
import { useCallback, useEffect, useState } from "react";
import type {
  CourseResourceRequest as CourseResource,
  CourseResourceResponse,
} from "../../../types/ResourceType";
import { UploadOutlined } from "@ant-design/icons";
import type { Course } from "../../Courses/types";
import courseService from "../../../services/course";
import type { NotificationProps } from "../../../types/Notification";
import {
  getBinaryFileFromUrl,
  getFileNameFromUrl,
} from "../../../utils/FileHelper";
import { resourceService } from "../../../services/resource/resourceService";

interface CourseResourceModalProps {
  visible: boolean;
  resourceId?: string;
  onCancel: () => void;
  onSubmit: (values: CourseResource) => void;
  title: string;
  onText: string;
  isEditing: boolean;
}

const CourseResourceModal: React.FC<CourseResourceModalProps> = ({
  visible,
  resourceId,
  onCancel,
  onSubmit,
  title,
  onText,
  isEditing,
}) => {
  const [form] = Form.useForm();
  const [courseResource, setCourseResource] =
    useState<CourseResourceResponse | null>(null);
  const [fileList, setFileList] = useState<UploadFile[]>([]);
  const [courses, setCourses] = useState<Course[]>([]);
  const [keyword, setKeyword] = useState<string | null>(null);
  const [notify, setNotify] = useState<NotificationProps | null>(null);
  const [selectedCourseId, setSelectedCourseId] = useState<string | undefined>(
    undefined,
  );
  const { notification } = App.useApp();

  const fetchResource = async () => {
    if (resourceId) {
      try {
        const response = await resourceService.getResourceById(resourceId);
        setCourseResource(response);
      } catch (error: any) {
        setNotify({
          type: "error",
          message: "Error",
          description: error.response?.data?.value || "Error fetching data",
        });
      }
    }
  };

  const fetchCourses = useCallback(async () => {
    try {
      // Get courses
      const courseResponse = await courseService.list({
        pageIndex: 1,
        pageSize: 10,
        keyword: keyword ?? undefined,
      });
      setCourses(courseResponse.items);
    } catch (error: any) {
      setNotify({
        type: "error",
        message: "Error",
        description: error.response?.data?.value || "Error fetching data",
      });
    }
  }, []);

  useEffect(() => {
    fetchResource();
    fetchCourses();
  }, [resourceId]);

  useEffect(() => {
    if (visible && isEditing && courseResource) {
      if (courseResource.resourceUrl) {
        const initialFileList: UploadFile[] = [
          {
            uid: courseResource.courseId, // Unique ID
            name: getFileNameFromUrl(courseResource.resourceUrl), // Display name
            status: "done" as const,
            url: courseResource.resourceUrl,
          },
        ];
        setFileList(initialFileList);
        form.setFieldsValue({
          title: courseResource.title,
          description: courseResource.description,
          courseId: courseResource.courseId,
          resource: initialFileList,
        });
      } else {
        setFileList([]);
      }
    } else {
      form.resetFields();
      setFileList([]);
      setSelectedCourseId(undefined);
    }
  }, [visible, form, courseResource, isEditing]);

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

  const handleOk = async () => {
    form
      .validateFields()
      .then(async (values) => {
        let resourceValue: any = null;
        if (fileList.length > 0) {
          resourceValue = fileList[0].originFileObj
            ? fileList[0].originFileObj
            : await getBinaryFileFromUrl(
                courseResource?.id as string,
                getFileNameFromUrl(
                  courseResource?.resourceUrl as string,
                ) as string,
              );
        }

        onSubmit({ ...values, resource: resourceValue });
      })
      .catch((err) => console.log("Validation failed:", err));
  };

  const handleCancel = () => {
    form.resetFields();
    setKeyword(null);
    onCancel();
    setFileList([]);
  };

  const beforeUpload = (file: File) => {
    const isLt1M = file.size / 1024 / 1024 < 1;
    if (!isLt1M) {
      setNotify({
        type: "error",
        message: "Error",
        description: "File size must smaller than 1MB!",
      });
    }

    return isLt1M ? false : Upload.LIST_IGNORE;
  };

  const handleChange = ({
    fileList: newFileList,
  }: {
    fileList: UploadFile[];
  }) => {
    setFileList(newFileList);
  };

  const handleCourseChange = (value: string) => {
    setSelectedCourseId(value);
    form.setFieldsValue({ courseId: value });
  };

  const onSearch = (value: string) => {
    setKeyword(value);
    fetchCourses();
  };
  return (
    <Modal
      centered
      title={title}
      open={visible}
      onOk={handleOk}
      onCancel={handleCancel}
      okText={onText}
    >
      <Form
        form={form}
        layout="vertical"
        name="course_resource_form"
        requiredMark={false}
      >
        <Form.Item
          name="courseId"
          label="Course"
          rules={[{ required: !isEditing, message: "Please select a course" }]}
        >
          <Select
            showSearch
            placeholder="Select a course"
            onChange={handleCourseChange}
            onSearch={onSearch}
            disabled={isEditing}
            value={selectedCourseId}
            options={courses}
            filterOption={(input, option) =>
              (option?.title ?? "").toLowerCase().includes(input.toLowerCase())
            }
            fieldNames={{ label: "title", value: "id" }}
          />
        </Form.Item>
        <Form.Item
          name="title"
          label="Title"
          rules={[
            { required: true, message: "Please enter material title" },
            { max: 50, message: "Title should not exceed 50 characters" },
            { whitespace: true, message: "Title cannot be empty" },
          ]}
        >
          <Input placeholder="Enter resource title" />
        </Form.Item>
        <Form.Item
          name="description"
          label="Description"
          rules={[
            { max: 300, message: "Description cannot exceed 300 characters" },
          ]}
        >
          <Input.TextArea placeholder="Enter resource description" />
        </Form.Item>

        <Form.Item
          name="resource"
          label="Resource"
          valuePropName="fileList"
          getValueFromEvent={(e: any) => {
            if (Array.isArray(e)) {
              return e;
            }
            return e && e.fileList;
          }}
          rules={[{ required: true, message: "Please upload a resource file" }]}
        >
          <Upload
            beforeUpload={beforeUpload}
            onChange={handleChange}
            fileList={fileList}
            maxCount={1}
            accept=".pdf,.png,.jpg,.jpeg,.mp4,.avi,.mpeg,.mp3,.wav,.aac"
            listType="picture"
          >
            <Button icon={<UploadOutlined />}>Upload</Button>
          </Upload>
        </Form.Item>
      </Form>
    </Modal>
  );
};
export default CourseResourceModal;
