import { type FC, use, useEffect, useState } from "react";
import type { Category, CourseFormDataOptions } from "./types.tsx";
import { Button, DatePicker, Form, Input, Modal, Select, Space, Tag } from "antd";
import { PlusOutlined } from "@ant-design/icons";
import {CourseDifficultyEnumMember} from "./initial-values.tsx";
// Import dayjs for proper date handling with Ant Design DatePicker
import dayjs from 'dayjs';
import { categoryList } from "./courseClient.tsx";

// Add a debounce utility
const debounce = <F extends (...args: any[]) => any>(
  func: F,
  waitFor: number,
) => {
  let timeout: ReturnType<typeof setTimeout> | null = null;

  const debounced = (...args: Parameters<F>) => {
    if (timeout !== null) {
      clearTimeout(timeout);
    }
    timeout = setTimeout(() => func(...args), waitFor);
  };

  return debounced as (...args: Parameters<F>) => ReturnType<F>;
};

type CourseFormProp = {
  formData: CourseFormDataOptions;
  categories: Category[];
  states: Record<string, string>;
  active: boolean;
  onClose: (targetAction?: string | undefined) => void;
};

export const CourseForm: FC<CourseFormProp> = ({
  formData,
  categories,
  states,
  active,
  onClose,
}) => {
  const [form] = Form.useForm<CourseFormDataOptions>();
  const [newTag, setNewTag] = useState<string>("");
  const [tags, setTags] = useState<string[]>([]);
  const [myCategories, setMyCategories] = useState<Category[]>([]);
  const [categoryKeyword, setCategoryKeyword] = useState<string>("");

const fetchCategories = async () => {
      try {
        const response = await categoryList(
          1,
          5,
          categoryKeyword.trim(),
        );
        setMyCategories(response.items);
      } catch (error) {
        console.error('Error fetching categories:', error);
      }
    };

  useEffect(() => {
    fetchCategories();
  }, [categoryKeyword]);
  // Helper to get current tags
  const getTags = () => tags;  
    useEffect(() => {
    if (active) {
      // Initial fetch of categories when the form becomes active
      fetchCategories();
      
      const formValues = {...formData};
      
      // Set status to "draft" for new courses
      if (!formData.title) {
        formValues.status = "draft";
      }      // Format the dueDate if it exists to be compatible with DatePicker
      if (formValues.dueDate && typeof formValues.dueDate === 'string') {
        try {
          // Convert string date to dayjs object - this is what Ant Design DatePicker expects
          const dateValue = dayjs(formValues.dueDate);
          // Use type assertion to bypass TypeScript's type checking
          (formValues as any).dueDate = dateValue;
        } catch (e) {
          console.error("Error formatting date:", e);
          // If conversion fails, set to undefined to avoid the isValid error
          (formValues as any).dueDate = undefined;
        }
      }
      
      form.setFieldsValue(formValues);
      
      // Initialize tags from formData
      const initialTags = Array.isArray(formData.tags) ? formData.tags : [];
      setTags(initialTags);
    }
  }, [active, form, formData]);
  const handleSubmit = async () => {
    try {
      const values = await form.validateFields();
        // Format the date for .NET DateTime compatibility
      // The dueDate from Ant Design DatePicker can be a Dayjs/Moment object
      let formattedDueDate = values.dueDate;
      // Check if it's an object with a dayjs format method
      if (values.dueDate && typeof values.dueDate === 'object') {
        // Using type assertion to safely access the properties
        const dateObj = values.dueDate as any;
        if (dateObj.format && typeof dateObj.format === 'function') {
          // Use dayjs format method to ensure consistent date format for .NET
          formattedDueDate = dateObj.format('YYYY-MM-DDTHH:mm:ss');
        } else if (dateObj.$d instanceof Date) {
          // Convert the date to ISO string format which is compatible with .NET DateTime
          formattedDueDate = dateObj.$d.toISOString();
        } else if (dateObj instanceof Date) {
          // If it's a simple Date object
          formattedDueDate = dateObj.toISOString();
        }
      }
      
      if (formData.title) {
        // Edit mode - would call update API
        console.log("EDIT MODE - Data that would be sent to update API:", {
          title: values.title,
          description: values.description,
          categoryId: values.categoryId,
          dueDate: formattedDueDate, // Using the formatted date
          difficulty: values.difficulty,
          status: values.status,
          tags: values.tags
        });
        
        console.log("BACKEND UPDATE EXPECTS:", {
          Title: "string",
          Description: "string", 
          CategoryId: "Guid",
          DueDate: "DateTime", // Now we're sending ISO format string which is compatible with .NET DateTime
          Difficulty: "CourseDifficulty enum"
        });
      } else {        
        console.log("CREATE MODE - Data that would be sent to create API:", {
          title: values.title,
          description: values.description,
          categoryId: values.categoryId,
          dueDate: formattedDueDate, 
          difficulty: values.difficulty,
          status: "draft",
          tags: values.tags,
          mentorId: "Would need to provide a mentorId here"
        });
        
        console.log("BACKEND CREATE EXPECTS:", {
          Title: "string",
          Description: "string",
          CategoryId: "Guid",
          MentorId: "Guid (missing in our form)",
          DueDate: "DateTime", 
          Difficulty: "CourseDifficulty enum"
        });
      }
      
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
      title={formData.title ? `Edit Course: ${formData.title}` : "Add New Course"}
      open={active}
      onCancel={() => onClose()}
      width={800}footer={[
        <Button key="cancel" onClick={() => onClose()}>
          Cancel
        </Button>,
        <Button key="submit" type="primary" onClick={handleSubmit}>
          {formData.title ? "Save Changes" : "Create Course"}
        </Button>,
      ]}
    >      <Form form={form} layout="vertical" initialValues={{
        ...formData, 
        tags: Array.isArray(formData.tags) ? formData.tags : [],
        status: !formData.title ? "draft" : formData.status,
        // Don't set dueDate in initialValues, we'll set it via form.setFieldsValue in useEffect
        dueDate: undefined
      }}>
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
            <Select
              showSearch
              placeholder="Select a category"
              options={myCategories.map((category) => ({
                label: category.name,
                value: category.id
              }))}
              filterOption={false}
              onSearch={debounce((input) => setCategoryKeyword(input), 100)}
              notFoundContent={categoryKeyword ? "No matching categories" : "Type to search categories"}
              loading={!myCategories.length && categoryKeyword !== ""}
            />
          </Form.Item>
          <Form.Item
            name="status"
            label="Status"
            initialValue="draft"
            rules={[{ required: true, message: "Please select a status!" }]}
          >
            {!formData.title ? (
              <>
                <Input 
                  value={states["draft"]} 
                  readOnly 
                  className="ant-select-selector" // Match Select styling
                />
                <div className="text-xs text-gray-500 mt-1">New courses are created with Draft status only</div>
              </>
            ) : (
              <Select>
                {Object.entries(states).map(([value, label]) => (
                  <Select.Option key={value} value={value}>
                    {label}
                  </Select.Option>
                ))}
              </Select>
            )}
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
            label="Due Date"
            rules={[{ required: true, message: "Please select a due date!" }]}
          >
            <DatePicker 
              style={{ width: '100%' }}
              placeholder="Select due date"
              format="YYYY-MM-DD"
            />
          </Form.Item><Form.Item
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
