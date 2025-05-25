import { useState, useEffect } from 'react';
import { Table, Space, Button, Tooltip, Tag, App, Popconfirm } from 'antd';
import { EditOutlined, DeleteOutlined, PlusOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import Search from 'antd/es/input/Search';
import EditCategoryModal from './components/EditCategoryModal';
import type { Category, CategoryFilter, CategoryRequest } from '../../types/CategoryTypes';
import type { NotificationProps } from '../../types/Notification';
import type { PaginatedList } from '../../types/Pagination';
import PaginationControls from '../../components/shared/Pagination';
import { createCategory, deleteCategory, editCategory, getCategoryById, getListCategories } from '../../services/category/categoryServices';

export default function CategoriesPage() {
  const [categories, setCategories] = useState<Category[]>([]);
  const [pagination, setPagination] = useState<PaginatedList<Category>>({
    items: [],
    pageIndex: 1,
    totalPages: 1,
    totalCount: 0,
    pageSize: 5,
  });
  const [filters, setFilters] = useState<CategoryFilter>({
    pageIndex: 1,
    pageSize: 5,
    keyword: "",
  });
  const [isModalVisible, setIsModalVisible] = useState(false);
  const [selectedCategory, setSelectedCategory] = useState<Category | null>(
    null,
  );
  const [notify, setNotify] = useState<NotificationProps | null>(null);
  const [isCreating, setIsCreating] = useState(true);
  const { notification } = App.useApp();

  const fetchData = async () => {
    try {
      const apiResponse = await getListCategories(
        filters.pageIndex,
        filters.pageSize,
        filters.keyword,
      );
      const items = apiResponse.items;
      setPagination({
        items: apiResponse.items || [],
        pageIndex: apiResponse.pageIndex || 1,
        totalPages: apiResponse.totalPages || 1,
        totalCount: apiResponse.totalCount || 0,
        pageSize: apiResponse.pageSize || filters.pageSize,
      });
      setCategories(items || []);
    } catch {
      setNotify({
        type: "error",
        message: "Error",
        description: "An error occurred while fetching users.",
      });
      setCategories([]);
      setPagination((prev) => ({ ...prev, items: [], totalCount: 0 }));
    }
  };
  useEffect(() => {
    fetchData();
  }, [filters]);

  useEffect(() => {
    if (notify) {
      notification[notify.type]({
        message: notify.message,
        description: notify.description,
        placement: "topRight",
      });
      setNotify(null);
    }
  }, [notify, notification]);

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
      pageIndex: 1,
    }));
  };

  const onPageIndexChange = (value: number) => {
    setFilters((prev) => ({
      ...prev,
      pageIndex: value,
    }));
  };

  const handleEditClick = async (category: Category) => {
    const apiResponse = await getCategoryById(category.id);
    setSelectedCategory(apiResponse);
    setIsCreating(false);
    setIsModalVisible(true);
  };

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

  const handleModelDelete = async (categoryId: string) => {
    try {
      await deleteCategory(categoryId);
      setNotify({
        type: "success",
        message: "Category deleted successfully",
        description: "The category has been deleted successfully.",
      });
      await fetchData();
    } catch {
      setNotify({
        type: "error",
        message: "Error",
        description: "An error occurred while deleting the category.",
      });
    }
  };

  const handleModalSubmit = async (values: CategoryRequest) => {
    try {
      const payload = {
        name: values.name,
        description: values.description || "",
        status: values.status,
      };
      console.log("payload", payload);
      if (isCreating) {
        await createCategory(payload);
        setNotify({
          type: "success",
          message: "Category created successfully",
          description: "The category has been created successfully.",
        });
      } else if (selectedCategory) {
        await editCategory(selectedCategory.id, payload);
        setNotify({
          type: "success",
          message: "Category updated successfully",
          description: "The category has been updated successfully.",
        });
      }

      await fetchData();
      setIsModalVisible(false);
      setSelectedCategory(null);
      setIsCreating(false);
    } catch (error: any) {
      setNotify({
        type: 'error',
        message: 'Error',
        description: error.response?.data?.error || 'An error occurred while processing your request.',
      });
      console.log('Error:', error);
    }
  };

  const columns: ColumnsType<Category> = [
    {
      title: "Name",
      dataIndex: "name",
      key: "name",
      width: 200,
      render: (text: string) => <span className="font-medium">{text}</span>,
    },
    {
      title: "Description",
      dataIndex: "description",
      key: "description",
      ellipsis: true,
    },
    {
      title: "Courses",
      dataIndex: "courses",
      key: "courses",
      align: "center",
      width: 100,
    },
    {
      title: "Status",
      dataIndex: "status",
      key: "status",
      align: "center",
      width: 100,
      render: (status: boolean) => (
        <Tag color={status ? "green" : "red"}>
          {status ? "Active" : "Inactive"}
        </Tag>
      ),
    },
    {
      title: "Actions",
      key: "actions",
      align: "center",
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
              title="Are you sure to delete this category?"
              onConfirm={() => handleModelDelete(record.id)}
              okText="Yes"
              cancelText="No"
            >
              <Button
                icon={<DeleteOutlined />}
                size="small"
                danger
                className="text-red-600"
              />
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
          name=""
        >
          Add Category
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
      <PaginationControls
        pageIndex={pagination.pageIndex}
        pageSize={pagination.pageSize}
        totalCount={pagination.totalCount}
        onPageChange={onPageIndexChange}
        onPageSizeChange={onPageSizeChange}
        itemName="categories"
      />
      <EditCategoryModal
        visible={isModalVisible}
        initialValues={
          isCreating
            ? { id: "", name: "", description: "", status: true }
            : selectedCategory
              ? {
                id: selectedCategory.id,
                name: selectedCategory.name.trimEnd().trimStart(),
                description: selectedCategory.description?.trimEnd() || "",
                status: selectedCategory.status,
              }
              : { id: "", name: "", description: "", status: false }
        }
        onCancel={handleModalCancel}
        onSubmit={handleModalSubmit}
        title={isCreating ? "Add Category" : "Edit Category"}
        onText={isCreating ? "Add" : "Update"}
      />
    </div>
  );
}
