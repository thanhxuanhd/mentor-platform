import React, { useEffect } from "react";
import { Modal, Form, Input, Select } from "antd";
import type { User } from "../../../types/UserTypes";

interface EditUserModalProps {
  visible: boolean;
  initialValues: User;
  onCancel: () => void;
  onSubmit: (values: User) => void;
}

const EditUserModal: React.FC<EditUserModalProps> = ({
  visible,
  initialValues,
  onCancel,
  onSubmit,
}) => {
  const [form] = Form.useForm<User>();

  useEffect(() => {
    if (visible) {
      form.setFieldsValue(initialValues);
    }
  }, [visible, form, initialValues]);

  const handleOk = () => {
    form
      .validateFields()
      .then((values) => {
        onSubmit({ ...initialValues, ...values });
        form.resetFields();
      })
      .catch((err) => console.log("Validation failed:", err));
  };

  const handleCancel = () => {
    form.resetFields();
    onCancel();
  };

  return (
    <Modal
      centered
      title="Edit User"
      open={visible}
      onOk={handleOk}
      onCancel={handleCancel}
      okText="Save"
    >
      <Form
        form={form}
        layout="vertical"
        name="edit_user_form"
        requiredMark={false}
      >
        <Form.Item name="name" label="Name" rules={[{ required: true }]}>
          <Input />
        </Form.Item>
        <Form.Item
          name="email"
          label="Email"
          rules={[{ required: true, type: "email", message: "Invalid email" }]}
        >
          <Input />
        </Form.Item>
        <Form.Item
          name="role"
          label="Role"
          rules={[{ required: true, message: "Please select a role" }]}
        >
          <Select placeholder="Select a role">
            <Select.Option value="Admin">Admin</Select.Option>
            <Select.Option value="Mentor">Mentor</Select.Option>
            <Select.Option value="Learner">Learner</Select.Option>
          </Select>
        </Form.Item>
        <Form.Item
          name="status"
          label="Status"
          rules={[{ required: true, message: "Please select a status" }]}
        >
          <Select placeholder="Select a status">
            <Select.Option value="Pending">Pending</Select.Option>
            <Select.Option value="Active">Active</Select.Option>
            <Select.Option value="Deactivated">Deactivated</Select.Option>
          </Select>
        </Form.Item>
      </Form>
    </Modal>
  );
};

export default EditUserModal;
