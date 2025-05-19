import { type FC, useEffect, useState } from "react";
import type { Category, CourseFormDataOptions } from "./types.tsx";
import { Button, Form, Input, Modal, Select, Space, Tag } from "antd";
import { PlusOutlined } from "@ant-design/icons";
import {CourseDifficultyEnumMember} from "./initial-values.tsx";

type CourseFormProp = {
  formData: CourseFormDataOptions;
  categories: Category[];
  states: Record<string, string>;
  active: boolean;
  onClose: (targetAction?: string | undefined) => void;
  onSubmit: string;
};

export const CourseForm: FC<CourseFormProp> = ({
  formData,
  categories,
  states,
  active,
  onClose,
  onSubmit
}) => {
  const [form] = Form.useForm<CourseFormDataOptions>();
  const [newTag, setNewTag] = useState<string>("");
  const [tags, setTags] = useState<string[]>([]);

  // Helper to get current tags
  const getTags = () => tags;
  useEffect(() => {
    if (active) {
      form.setFieldsValue(formData);
      
      // Initialize tags from formData
      const initialTags = Array.isArray(formData.tags) ? formData.tags : [];
      setTags(initialTags);
    }
  }, [active, form, formData]);

  const handleSubmit = async () => {
    try {
      const values = await form.validateFields();
      console.log("Success:", values);
      onClose();
    } catch (errorInfo) {
      console.log("Failed:", errorInfo);
    }
  };

  if (!active) {
    return null;
  }

  const handleAddTag = () => {
    const trimmedTag = newTag.trim();
    if (trimmedTag && !tags.includes(trimmedTag)) {
      const updatedTags = [...tags, trimmedTag];
      setTags(updatedTags);
      form.setFieldValue("tags", updatedTags);
      setNewTag("");
    }
  };
  const handleRemoveTag = (tagToRemove: string) => {
    const updatedTags = tags.filter((t: string) => t !== tagToRemove);
    setTags(updatedTags);
    form.setFieldValue("tags", updatedTags);
  };

  return (
    <Modal
      title={`Course: ${formData.title || "New Course"}`}
      open={active}
      onCancel={() => onClose()}
      width={800}
      footer={[
        <Button key="cancel" onClick={() => onClose()}>
          Cancel
        </Button>,
        <Button key="submit" type="primary" onClick={handleSubmit}>
          Save Changes
        </Button>,
      ]}
    >
      <Form form={form} layout="vertical" initialValues={{...formData, tags: Array.isArray(formData.tags) ? formData.tags : []}}>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <Form.Item
            name="title"
            label="Title"
            rules={[{ required: true, message: "Please input the title!" }]}
          >
            <Input placeholder="Course title" />
          </Form.Item>

          <Form.Item
            name="categoryId"
            label="Category"
            rules={[{ required: true, message: "Please select a category!" }]}
          >
            <Select placeholder="Select a category">
              {categories.map((category) => (
                <Select.Option key={category.id} value={category.id}>
                  {category.name}
                </Select.Option>
              ))}
            </Select>
          </Form.Item>

          <Form.Item
            name="status"
            label="Status"
            rules={[{ required: true, message: "Please select a status!" }]}
          >
            <Select>
              {Object.entries(states).map(([value, label]) => (
                <Select.Option key={value} value={value}>
                  {label}
                </Select.Option>
              ))}
            </Select>
          </Form.Item>

          <Form.Item
            name="difficulty"
            label="Difficulty"
            rules={[{ required: true, message: "Please select a difficulty!" }]}
          >
            <Select>
              {Object.entries(CourseDifficultyEnumMember).map(([value, label]) => (
                <Select.Option key={value} value={value}>
                  {label}
                </Select.Option>
              ))}
            </Select>
          </Form.Item>

          <Form.Item
            name="dueDate"
            label="Duration"
            rules={[{ required: true, message: "Please input the duration!" }]}
          >
            <Input placeholder="e.g. 6 weeks" />
          </Form.Item>          <Form.Item
            name="tags"
            label="Tags"
            rules={[
              { required: true, message: "Please add at least one tag!" },
            ]}
          >
            <Space direction="vertical" style={{ width: "100%" }}>
              <Space.Compact style={{ width: "100%" }}>
                <Input
                  value={newTag}
                  onChange={(e) => setNewTag(e.target.value)}
                  placeholder="Add a tag"
                  onPressEnter={e => {
                    e.preventDefault();
                    handleAddTag();
                  }}
                />
                <Button
                  type="primary"
                  icon={<PlusOutlined />}
                  onClick={handleAddTag}
                  disabled={!newTag.trim() || getTags().includes(newTag.trim())}
                >
                  Add
                </Button>
              </Space.Compact>
              <Space wrap style={{ marginTop: 8 }}>
                {getTags().map((tag: string) => (
                  <Tag
                    key={tag}
                    closable
                    onClose={() => handleRemoveTag(tag)}
                  >
                    {tag}
                  </Tag>
                ))}
              </Space>
            </Space>
          </Form.Item>

          <Form.Item
            name="description"
            label="Description"
            className="md:col-span-2"
            rules={[
              { required: true, message: "Please input the description!" },
            ]}
          >
            <Input.TextArea rows={4} placeholder="Course description" />
          </Form.Item>
        </div>
      </Form>
    </Modal>
  );
};
