import { Form, Input, Modal, Select } from "antd";
import type { Category } from "../../../types/CategoryTypes";
import { useEffect } from "react";

interface EditCategoryModalProps {
    visible: boolean;
    initialValues: Category;
    onCancel: () => void;
    onSubmit: (values: Category) => void;
}

const EditCategoryModal: React.FC<EditCategoryModalProps> = ({
    visible,
    initialValues,
    onCancel,
    onSubmit,
}) => {
    const [form] = Form.useForm<Category>();

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
            title="Edit Category"
            open={visible}
            onOk={handleOk}
            onCancel={handleCancel}
            okText="Save"
        >
            <Form
                form={form}
                layout="vertical"
                name="edit_category_form"
                requiredMark={false}
            >
                <Form.Item name="name" label="Name" rules={[{ required: true }]}>
                    <Input />
                </Form.Item>
                <Form.Item
                    name="description"
                    label="Description"
                    rules={[{ required: true }]}
                >
                    <Input.TextArea />
                </Form.Item>
                <Form.Item name="status" label="Status">
                    <Select>
                        <Select.Option value="Active">Active</Select.Option>
                        <Select.Option value="Inactive">Inactive</Select.Option>
                    </Select>
                </Form.Item>
            </Form>
        </Modal>
    );
}