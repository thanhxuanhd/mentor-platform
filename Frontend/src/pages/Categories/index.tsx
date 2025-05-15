import { useState, useEffect } from 'react';
import { Table, Space, Button, Tooltip, Tag, Popconfirm, message } from 'antd';
import { EditOutlined, DeleteOutlined, PlusOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import Search from 'antd/es/input/Search';
import EditCategoryModal from './components/EditCategoryModal';
import type { Category, CategoryFilter } from '../../types/CategoryTypes';
import { getListCategories } from '../../services/categoryServices';

export default function CategoriesPage() {
  const [categories, setCategories] = useState<Category[]>([]);
  const [filters, setFilters] = useState<CategoryFilter>({
    pageIndex: 1,
    pageSize: 5,
    keyword: '',
  });
  const [isModalVisible, setIsModalVisible] = useState(false);
  const [selectedCategory, setSelectedCategory] = useState<Category | null>(null);
  const [isCreating, setIsCreating] = useState(false);
  const [loading, setLoading] = useState(false);

  const fetchData = async () => {
    setLoading(true);
    try {
      const apiResponse = await getListCategories(filters);
      const items = apiResponse.items;
      setCategories(items);
    } catch (error) {
      message.error('Failed to fetch categories');
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
    setIsModalVisible(true);
  }

  const handleCreateClick = () => {
    setSelectedCategory(null);
    setIsCreating(true);
    setIsModalVisible(true);
  };

  const handleDelete = (id: string) => {
    setCategories((prev) => prev.filter((cat) => cat.id !== id));
    message.success('Category deleted successfully');
  };

  const handleModalCancel = () => {
    setIsModalVisible(false);
    setSelectedCategory(null);
  };

  const handleModalSubmit = (values: Category) => {
    if (isCreating) {
      const newCategory = {
        ...values,
        id: `cat-${Date.now()}`,
      };
      setCategories((prev) => [...prev, newCategory]);
    } else {
      setCategories((prev) =>
        prev.map((cat) => (cat.id === values.id ? values : cat))
      );
    }
    setIsModalVisible(false);
    setSelectedCategory(null);
    setIsCreating(false);
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
            <Popconfirm
              title="Are you sure you want to delete this category?"
              onConfirm={() => handleDelete(record.id)}
              okText="Yes"
              cancelText="No"
            >
              <Button icon={<DeleteOutlined />} size="small" danger />
            </Popconfirm>

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
            ? { id: '', name: '', description: '', status: 'Active' }
            : selectedCategory
              ? {
                id: selectedCategory.id,
                name: selectedCategory.name,
                description: selectedCategory.description || '',
                status: selectedCategory.status ? 'Active' : 'Inactive',
              }
              : { id: '', name: '', description: '', status: 'Active' }
        }
        onCancel={handleModalCancel}
        onSubmit={handleModalSubmit}
        title={isCreating ? 'Create Category' : 'Edit Category'}
      />
    </div>
  );
}

