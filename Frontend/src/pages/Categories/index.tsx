import { useState, useEffect } from 'react';
import { Table, Space, Button, Tooltip, Tag } from 'antd';
import { EditOutlined, DeleteOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { getListCategories } from '../../services/categoryServices';
import Search from 'antd/es/input/Search';

interface Category {
  id: string;
  name: string;
  description?: string;
  courses: number;
  status: boolean;
}

interface CategoryFilter {
  pageSize: number;
  pageIndex: number;
  keyword: string;
}

export default function CategoriesPage() {
  const [categories, setCategories] = useState<Category[]>([]);
  const [filters, setFilters] = useState<CategoryFilter>({
    pageIndex: 1,
    pageSize: 5,
    keyword: '',
  });

  useEffect(() => {
    const fetchData = async () => {
      const apiResponse = await getListCategories(filters);
      const items = apiResponse.items;
      setCategories(items);
    };
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
    </div>
  );
}
