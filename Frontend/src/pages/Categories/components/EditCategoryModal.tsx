import { Form, Input, Modal, Select } from "antd";
import { useEffect } from "react";
import type { Category, EditCategory } from '../../../types/CategoryTypes';

interface EditCategoryModalProps {
    visible: boolean;
    initialValues: EditCategory;
    onCancel: () => void;
    onSubmit: (values: Category) => void;
    title: string;
}

const EditCategoryModal: React.FC<EditCategoryModalProps> = ({
    visible,
    initialValues,
    onCancel,
    onSubmit,
    title
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
            title={title}
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
                <Form.Item name="name" label="Name" rules={[{ required: true, message: 'Please enter the category name' },
                { max: 50, message: 'Name cannot exceed 50 characters' },
                { min: 3, message: 'Name must be at least 3 characters' }]}>
                    <Input />
                </Form.Item>
                <Form.Item
                    name="description"
                    label="Description"
                    rules={[{ max: 200, message: 'Description cannot exceed 200 characters' }]}
                >
                    <Input.TextArea />
                </Form.Item>
                <Form.Item name="status" label="Status">
                    <Select>
                        <Select.Option value={true}>Active</Select.Option>
                        <Select.Option value={false}>Inactive</Select.Option>
                    </Select>
                </Form.Item>
            </Form>
        </Modal>
    );
}
export default EditCategoryModal;