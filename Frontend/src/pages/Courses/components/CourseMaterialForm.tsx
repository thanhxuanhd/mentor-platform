import { Button, Form, Input, Modal, Select } from "antd";
import { type FC, useState } from "react";
import { courseService } from "../../../services/course";
import type { CourseMaterialFormProps } from "../../../types/pages/courses/types.ts";

export type MediaType = "pdf" | "ExternalWebAddress" | "video" | "Course Id";

export interface MaterialFormData {
  title: string;
  description: string;
  mediaType: MediaType;
  webAddress?: string;
}

const CourseMaterialForm: FC<CourseMaterialFormProps> = ({
  visible,
  courseId,
  onCancel,
  onSuccess,
}) => {
  const [form] = Form.useForm<MaterialFormData>();
  const [submitting, setSubmitting] = useState(false);

  const handleSubmit = async () => {
    if (!courseId) return;

    try {
      setSubmitting(true);
      const values = await form.validateFields();

      await courseService.createResource(courseId, {
        title: values.title,
        description: values.description,
        mediaType: values.mediaType,
        webAddress: values.webAddress || "",
      });

      form.resetFields();
      onSuccess();
    } catch (error) {
      console.error("Failed to create material:", error);
      Modal.error({
        title: "Failed to add material",
        content: "There was an error adding your material. Please try again.",
      });
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <Modal
      title="Add Course Material"
      open={visible}
      onCancel={onCancel}
      footer={[
        <Button key="cancel" onClick={onCancel}>
          Cancel
        </Button>,
        <Button
          key="submit"
          type="primary"
          onClick={handleSubmit}
          loading={submitting}
          disabled={submitting}
          className="bg-orange-500"
        >
          Add Material
        </Button>,
      ]}
      width={600}
      centered
    >
      <Form form={form} layout="vertical" initialValues={{ mediaType: "pdf" }}>
        <Form.Item
          name="title"
          label="Title"
          rules={[
            { required: true, message: "Please enter material title" },
            {
              max: 256,
              message: "Material title should not exceed 256 characters",
            },
          ]}
        >
          <Input placeholder="Enter material title" />
        </Form.Item>

        <Form.Item
          name="description"
          label="Description"
          rules={[
            { required: true, message: "Description is required" },
            { max: 256, message: "Course Item must not exceed 256 characters" },
          ]}
        >
          <Input.TextArea rows={4} placeholder="Enter material description" />
        </Form.Item>

        <Form.Item
          name="mediaType"
          label="Media Type"
          rules={[{ required: true, message: "Please select media type" }]}
        >
          <Select>
            <Select.Option value="pdf">PDF</Select.Option>
            <Select.Option value="ExternalWebAddress">
              External Web Address
            </Select.Option>
            <Select.Option value="video">Video</Select.Option>
            <Select.Option value="Course Id">Course ID</Select.Option>
          </Select>
        </Form.Item>

        <Form.Item
          noStyle
          shouldUpdate={(prevValues, currentValues) =>
            prevValues.mediaType !== currentValues.mediaType
          }
        >
          {({ getFieldValue }) =>
            getFieldValue("mediaType") !== "pdf" && (
              <Form.Item
                name="webAddress"
                label="Web Address"
                rules={[
                  { required: true, message: "Please enter web address" },
                  { type: "url", message: "Please enter a valid URL" },
                  { max: 2048, message: "URL is too long" },
                ]}
              >
                <Input placeholder="Enter web address" />
              </Form.Item>
            )
          }
        </Form.Item>
      </Form>
    </Modal>
  );
};

export default CourseMaterialForm;
