import { App, Button, Card, Form, Input, Modal, Upload } from "antd";
import type { UploadFile } from "antd/es/upload/interface";
import { useEffect, useState } from "react";
import type {
  CourseResourceRequest as CourseResource,
  CourseResourceResponse,
} from "../../../types/ResourceType";
import { UploadOutlined } from "@ant-design/icons";
import type { Course } from "../../Courses/types";
import type { NotificationProps } from "../../../types/Notification";
import {
  getBinaryFileFromUrl,
  getFileNameFromUrl,
} from "../../../utils/FileHelper";

interface CourseResourceModalSimpleProps {
  visible: boolean;
  initialValues?: CourseResourceResponse;
  onCancel: () => void;
  onSubmit: (values: CourseResource) => void;
  title: string;
  onText: string;
  isEditing: boolean;
  course: Course;
}

const CourseResourceModalSimple: React.FC<CourseResourceModalSimpleProps> = ({
  visible,
  initialValues,
  onCancel,
  onSubmit,
  title,
  onText,
  isEditing,
  course,
}) => {
  const [form] = Form.useForm();
  const [fileList, setFileList] = useState<UploadFile[]>([]);
  const [notify, setNotify] = useState<NotificationProps | null>(null);
  const { notification } = App.useApp();

  useEffect(() => {
    if (visible && isEditing && initialValues) {
      if (initialValues.resourceUrl) {
        const initialFileList: UploadFile[] = [
          {
            uid: course.id,
            name: getFileNameFromUrl(initialValues.resourceUrl),
            status: "done" as const,
            url: initialValues.resourceUrl,
          },
        ];
        setFileList(initialFileList);
        form.setFieldsValue({
          title: initialValues.title,
          description: initialValues.description,
          resource: initialFileList,
        });
      } else {
        setFileList([]);
      }
    } else {
      form.resetFields();
      setFileList([]);
    }
  }, [visible, form, initialValues, isEditing]);

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
            : await getBinaryFileFromUrl(fileList[0].url as string);
        }
        onSubmit({ ...values, resource: resourceValue, courseId: course.id });
      })
      .catch((err) => console.log("Validation failed:", err));
  };

  const handleCancel = () => {
    form.resetFields();
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
        name="course_resource_form_simple"
        requiredMark={false}
      >
        <Form.Item label="Course">
          <Input value={course.title} disabled />
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
            showUploadList={false}
            accept=".pdf,.png,.jpg,.jpeg,.mp4,.avi,.mpeg,.mp3,.wav,.aac"
            listType="picture"
          >
            <Button icon={<UploadOutlined />}>Upload</Button>
          </Upload>
        </Form.Item>
        {fileList.length > 0 && (
          <Card>
            <div className="flex justify-between">
              <p>{fileList[0].name}</p>
              <Button
                danger
                onClick={() => {
                  form.setFieldValue("resource", undefined);
                  setFileList([]);
                }}
              >
                Remove
              </Button>
            </div>
          </Card>
        )}
      </Form>
    </Modal>
  );
};
export default CourseResourceModalSimple;
