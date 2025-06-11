import { type FC, useEffect, useState } from "react";
import type { Category, CourseFormDataOptions } from "../types.tsx";
import {
  App,
  Button,
  DatePicker,
  Form,
  Input,
  Modal,
  Select,
  Space,
  Tag,
} from "antd";
import { PlusOutlined } from "@ant-design/icons";
import { CourseDifficultyEnumMember } from "../initial-values.tsx";
import dayjs from "dayjs";
import { courseService } from "../../../services/course";
import { useAuth } from "../../../hooks";
import type { CourseFormProps } from "../../../types/pages/courses/types.ts";
import { isAxiosError } from "axios";
import { getActiveCategories } from "../../../services/category/categoryServices.tsx";

export const CourseForm: FC<CourseFormProps> = ({
  formData,
  active,
  onClose,
}) => {
  const { user } = useAuth(); // Get current user from auth context
  const { message } = App.useApp();
  const [form] = Form.useForm<CourseFormDataOptions>();
  const [newTag, setNewTag] = useState<string>("");
  const [tags, setTags] = useState<string[]>([]);
  const [myCategories, setMyCategories] = useState<Category[]>([]);
  const [categoryKeyword, setCategoryKeyword] = useState<string>("");
  const [submitting, setSubmitting] = useState<boolean>(false);
  const fetchCategories = async () => {
    try {
      const response = await getActiveCategories();
      // Lọc danh mục phía client dựa trên categoryKeyword
      const filteredCategories = categoryKeyword
        ? response.filter((category: { name: string }) =>
          category.name.toLowerCase().includes(categoryKeyword.trim().toLowerCase())
        )
        : response;
      setMyCategories(filteredCategories);
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

      const formValues = { ...formData };

      if (formValues.dueDate && typeof formValues.dueDate === "string") {
        try {
          // Convert string date to dayjs object - this is what Ant Design DatePicker expects
          // Use type assertion to bypass TypeScript's type checking
          (formValues as any).dueDate = dayjs(formValues.dueDate);
        } catch (e) {
          console.error("Error formatting date:", e);
          (formValues as any).dueDate = undefined;
        }
      }

      form.setFieldsValue(formValues);

      const initialTags = Array.isArray(formData.tags) ? formData.tags : [];
      setTags(initialTags);
    } else {
      setNewTag("");
    }
  }, [active, form, formData]);
  const handleSubmit = async () => {
    try {
      const values = await form.validateFields();

      // Format the date for .NET DateTime compatibility
      let formattedDueDate = values.dueDate;

      // Check if it's an object with a dayjs format method
      if (values.dueDate && typeof values.dueDate === "object") {
        // Using type assertion to safely access the properties
        const dateObj = values.dueDate as any;
        if (dateObj.format && typeof dateObj.format === "function") {
          // Use dayjs format method to ensure consistent date format for .NET
          formattedDueDate = dateObj.format("YYYY-MM-DDTHH:mm:ss");
        } else if (dateObj.$d instanceof Date) {
          // Convert the date to ISO string format which is compatible with .NET DateTime
          formattedDueDate = dateObj.$d.toISOString();
        } else if (dateObj instanceof Date) {
          // If it's a simple Date object
          formattedDueDate = dateObj.toISOString();
        }
      }
      if (formData.id) {
        // Edit mode - call update API
        console.log(
          "EDIT MODE - Sending update request for course ID:",
          formData.id,
        );

        setSubmitting(true);
        try {
          // Make the API call to update the course
          await courseService.update(formData.id, {
            title: values.title,
            description: values.description,
            categoryId: values.categoryId || "",
            dueDate: formattedDueDate as string,
            difficulty: values.difficulty,
            mentorId: user?.id || "", // Use current user's ID as mentorId
            tags: getTags(), // Include tags in the API call
          });

          // Close the form and signal to refresh the course list
          onClose("refresh");
          message.success("Update successfully!");
        } catch (error) {
          console.error("Error updating course:", error);
          if (!isAxiosError(error)) {
            message.error("An unknown error occurred.");
          } else {
            if (error.response?.data?.errors) {
              const allErrors: string[] = [];
              const fluentValidationErrors = error.response?.data?.errors;
              Object.values(fluentValidationErrors)
                .filter(Boolean)
                .forEach((fieldErrors) => {
                  if (Array.isArray(fieldErrors)) {
                    allErrors.push(...fieldErrors);
                  } else if (typeof fieldErrors === "string") {
                    allErrors.push(fieldErrors);
                  }
                });
              message.error(allErrors.join("\n"));
              return;
            }

            if (error.response?.data?.error) {
              const errorMessage =
                error.response?.data?.error ?? "An unknown error occurred.";
              message.error(errorMessage);
              return;
            }
          }
        } finally {
          setSubmitting(false);
        }
      } else {
        // Create mode - call the POST /api/course endpoint
        console.log("CREATE MODE - Data being sent to create API:", {
          title: values.title,
          description: values.description,
          categoryId: values.categoryId,
          dueDate: formattedDueDate,
          difficulty: values.difficulty,
          tags: getTags(),
          mentorId: user?.id || "", // Use current user's ID as mentorId
        });

        setSubmitting(true);
        try {
          // Make the API call to create a new course
          await courseService.create({
            title: values.title,
            description: values.description,
            categoryId: values.categoryId || "",
            dueDate: formattedDueDate as string,
            difficulty: values.difficulty,
            mentorId: user?.id || "", // Use current user's ID as mentorId
            tags: getTags(), // Include tags in the API call
          });

          // Close the form and signal to refresh the course list
          onClose("refresh");
          message.success("Create successfully!");
        } catch (error) {
          console.error("Error creating course:", error);
          if (!isAxiosError(error)) {
            message.error("An unknown error occurred.");
          } else {
            if (error.response?.data?.errors) {
              const allErrors: string[] = [];
              const fluentValidationErrors = error.response?.data?.errors;
              Object.values(fluentValidationErrors)
                .filter(Boolean)
                .forEach((fieldErrors) => {
                  if (Array.isArray(fieldErrors)) {
                    allErrors.push(...fieldErrors);
                  } else if (typeof fieldErrors === "string") {
                    allErrors.push(fieldErrors);
                  }
                });
              message.error(allErrors.join("\n"));
              return;
            }

            if (error.response?.data?.error) {
              const errorMessage =
                error.response?.data?.error ?? "An unknown error occurred.";
              message.error(errorMessage);
              return;
            }
          }
        } finally {
          setSubmitting(false);
        }
      }
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
      title={formData.id ? `Edit Course: ${formData.title}` : "Add New Course"}
      open={active}
      onCancel={() => onClose()}
      width={800}
      footer={[
        <Button key="cancel" onClick={() => onClose()}>
          Cancel
        </Button>,
        <Button
          key="submit"
          type="primary"
          onClick={handleSubmit}
          loading={submitting}
          disabled={submitting}
        >
          {formData.id ? "Save Changes" : "Create Course"}
        </Button>,
      ]}
    >
      <Form
        form={form}
        layout="vertical"
        initialValues={{
          ...formData,
          tags: Array.isArray(formData.tags) ? formData.tags : [],
          dueDate: undefined,
        }}
      >
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <Form.Item
            name="title"
            label="Title"
            rules={[
              { required: true, message: "Please enter course title" },
              {
                max: 256,
                message: "Course title should not exceed 256 characters",
              },
            ]}
          >
            <Input placeholder="Enter new course title" />
          </Form.Item>

          <Form.Item
            name="categoryId"
            label="Category"
            rules={[{ required: true, message: "Category is required" }]}
          >
            <Select
              showSearch
              placeholder="Select a category"
              options={myCategories.map((category) => ({
                label: category.name,
                value: category.id,
              }))}
              filterOption={false}
              onSearch={(input) => setCategoryKeyword(input)}
              notFoundContent={
                categoryKeyword
                  ? "No matching categories"
                  : "Type to search categories"
              }
              loading={!myCategories.length && categoryKeyword !== ""}
            />
          </Form.Item>

          <Form.Item
            name="difficulty"
            label="Difficulty"
            rules={[
              {
                validator: (_, value) => {
                  if (!value) {
                    return Promise.reject(
                      new Error("Please select a difficulty!"),
                    );
                  }
                  return Promise.resolve();
                },
              },
            ]}
          >
            <Select>
              {Object.entries(CourseDifficultyEnumMember).map(
                ([value, label]) => (
                  <Select.Option key={value} value={value}>
                    {label}
                  </Select.Option>
                ),
              )}
            </Select>
          </Form.Item>
          <Form.Item
            name="dueDate"
            label="Due Date"
            rules={[
              {
                validator: (_, value) => {
                  if (!value) {
                    return Promise.reject(
                      new Error("Please select a due date!"),
                    );
                  }
                  return Promise.resolve();
                },
              },
            ]}
          >
            <DatePicker
              style={{ width: "100%" }}
              placeholder="Select due date"
              format="YYYY-MM-DD"
              minDate={dayjs().add(1, "day")}
              inputReadOnly={true}
            />
          </Form.Item>
          <Form.Item
            name="tags"
            label="Tags"
            rules={[
              {
                validator: async () => {
                  if (getTags().length > 5) {
                    throw new Error("You can choose maximum 5 tags");
                  }
                  return Promise.resolve();
                },
              },
            ]}
          >
            <Space direction="vertical" style={{ width: "100%" }}>
              <Space.Compact style={{ width: "100%" }}>
                <Input
                  value={newTag}
                  onChange={(e) => setNewTag(e.target.value)}
                  placeholder="Add a tag"
                  onPressEnter={(e) => {
                    e.preventDefault();
                    if (getTags().length >= 5) {
                      message.error("You can choose maximum 5 tags");
                      return;
                    }
                    handleAddTag();
                  }}
                />
                <Button
                  type="primary"
                  icon={<PlusOutlined />}
                  onClick={handleAddTag}
                  disabled={
                    !newTag.trim() ||
                    getTags().includes(newTag.trim()) ||
                    getTags().length >= 5
                  }
                >
                  Add
                </Button>
              </Space.Compact>
              <Space wrap style={{ marginTop: 8 }}>
                {getTags().map((tag: string) => (
                  <Tag key={tag} closable onClose={() => handleRemoveTag(tag)}>
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
              {
                max: 256,
                message: "Course description should not exceed 256 characters",
              },
            ]}
          >
            <Input.TextArea rows={4} placeholder="Enter course description" />
          </Form.Item>
        </div>
      </Form>
    </Modal>
  );
};