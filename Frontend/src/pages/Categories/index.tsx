import { useState, useEffect } from 'react';
import { Table, Space, Button, Tooltip, Tag, Popconfirm, message } from 'antd';
import { EditOutlined, DeleteOutlined, PlusOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import Search from 'antd/es/input/Search';
import EditCategoryModal from './components/EditCategoryModal';
import type { Category, CategoryFilter, CategoryRequest } from '../../types/CategoryTypes';
import { createCategory, editCategory, getListCategories } from '../../services/categoryServices';

export default function CategoriesPage() {
  const [categories, setCategories] = useState<Category[]>([]);
  const [filters, setFilters] = useState<CategoryFilter>({
    pageIndex: 1,
    pageSize: 50,
    keyword: '',
  });
  const [isModalVisible, setIsModalVisible] = useState(false);
  const [selectedCategory, setSelectedCategory] = useState<Category | null>(null);
  const [isCreating, setIsCreating] = useState(true);
  const [loading, setLoading] = useState(false);

  const fetchData = async () => {
    setLoading(true);
    try {
      const apiResponse = await getListCategories(filters);
      const items = apiResponse.items;
      setCategories(items);
    } catch (error: any) {
      message.error(`Failed to fetch categories: ${error.message}`);
    } finally {
      setLoading(false);
    }
  };
  useEffect(() => {
    fetchData();
  }, [filters]);

  const onSearch = (value: string) => {
    setFilters((prev) => ({
      ...prev,
      keyword: value,
      pageIndex: 1,
    }));
  };

  const onPageSizeChange = (value: number) => {
    setFilters((prev) => ({
      ...prev,
      pageSize: value,
    }));
  };

  const onPageIndexChange = (value: number) => {
    setFilters((prev) => ({
      ...prev,
      pageIndex: value,
    }));
  };

  const handleEditClick = (category: Category) => {
    setSelectedCategory(category);
    setIsCreating(false);
    setIsModalVisible(true);
  }

  const handleCreateClick = () => {
    setSelectedCategory(null);
    setIsCreating(true);
    setIsModalVisible(true);
  };

  const handleModalCancel = () => {
    setIsModalVisible(false);
    setSelectedCategory(null);
    setIsCreating(false);
  };

  const handleModalSubmit = async (values: CategoryRequest) => {
    try {
      const payload = {
        name: values.name,
        description: values.description || '',
        status: values.status,
      };
      console.log('payload', payload);
      if (isCreating) {
        await createCategory(payload);
        message.success('Category created successfully');
      } else if (selectedCategory) {
        await editCategory(selectedCategory.id, payload);
        message.success('Category updated successfully');
      }

      await fetchData();
      setIsModalVisible(false);
      setSelectedCategory(null);
      setIsCreating(false);
    } catch (error: any) {
      message.error(isCreating ? `Failed to create category: ${error.message}` : `Failed to update category: ${error.message}`);
    }
  };

  const columns: ColumnsType<Category> = [
    {
      title: 'Name',
      dataIndex: 'name',
      key: 'name',
      width: 200,
      render: (text: string) => <span className="font-medium">{text}</span>,
    },
    {
      title: 'Description',
      dataIndex: 'description',
      key: 'description',
      ellipsis: true,
    },
    {
      title: 'Courses',
      dataIndex: 'courses',
      key: 'courses',
      align: 'center',
      width: 100,
    },
    {
      title: 'Status',
      dataIndex: 'status',
      key: 'status',
      align: 'center',
      width: 100,
      render: (status: boolean) => (
        <Tag color={status ? 'green' : 'red'}>
          {status ? 'Active' : 'Inactive'}
        </Tag>
      ),
    },
    {
      title: 'Actions',
      key: 'actions',
      align: 'center',
      width: 120,
      render: (_: any, record: Category) => (
        <Space size="small">
          <Tooltip title="Edit Category">
            <Button
              icon={<EditOutlined />}
              size="small"
              className="text-green-600"
              onClick={() => handleEditClick(record)}
            />
          </Tooltip>
          <Tooltip title="Delete Category">
            <Button icon={<DeleteOutlined />} size="small" danger />
          </Tooltip>
        </Space>
      ),
    },
  ];

  return (
    <div className="bg-gray-800 rounded-lg overflow-hidden shadow-lg p-6">
      <div className="flex justify-between items-center">
        <h2 className="text-2xl font-semibold">Category Management</h2>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={() => handleCreateClick()}
        >
          Create Category
        </Button>
      </div>
      <Search
        placeholder="Search by category name..."
        allowClear
        size="large"
        enterButton
        onSearch={onSearch}
        className="my-8"
      />
      <Table<Category>
        columns={columns}
        dataSource={categories}
        rowKey="id"
        pagination={false}
      />
      <EditCategoryModal
        visible={isModalVisible}
        initialValues={
          isCreating
            ? { id: '', name: '', description: '', status: true }
            : selectedCategory
              ? {
                id: selectedCategory.id,
                name: selectedCategory.name,
                description: selectedCategory.description || '',
                status: selectedCategory.status,
              }
              : { id: '', name: '', description: '', status: false }
        }
        onCancel={handleModalCancel}
        onSubmit={handleModalSubmit}
        title={isCreating ? 'Create Category' : 'Edit Category'}
      />
    </div>
  );
}

