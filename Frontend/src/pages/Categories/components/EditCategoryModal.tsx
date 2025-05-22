import { Form, Input, Modal, Radio } from "antd";
import { useEffect } from "react";
import type { Category, EditCategory } from "../../../types/CategoryTypes";

interface EditCategoryModalProps {
  visible: boolean;
  initialValues: EditCategory;
  onCancel: () => void;
  onSubmit: (values: Category) => void;
  title: string;
  onText: string;
}

const EditCategoryModal: React.FC<EditCategoryModalProps> = ({
  visible,
  initialValues,
  onCancel,
  onSubmit,
  title,
  onText,
}) => {
  const [form] = Form.useForm();

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
      title={title}
      open={visible}
      onOk={handleOk}
      onCancel={handleCancel}
      okText={onText}
    >
      <Form
        form={form}
        layout="vertical"
        name="edit_category_form"
        requiredMark={false}
      >
        <Form.Item name="name" label="Name" rules={[{ required: true, message: 'Please enter category name' },
        { max: 50, message: 'Category name should not exceed 50 characters' },
        { whitespace: true, message: 'Category name cannot be empty' }
        ]}>
          <Input placeholder="Enter new category name" />
        </Form.Item>
        <Form.Item
          name="description"
          label="Description"
          rules={[{ max: 200, message: 'Description cannot exceed 200 characters' }
          ]}
        >
          <Input.TextArea placeholder="Enter your description" />
        </Form.Item>
        <Form.Item name="status" label="Status">
          <Radio.Group>
            <Radio value={true}>Active</Radio>
            <Radio value={false}>Inactive</Radio>
          </Radio.Group>
        </Form.Item>
      </Form>
    </Modal>
  );
}
export default EditCategoryModal;