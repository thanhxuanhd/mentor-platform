import {
  App,
  Button,
  Card,
  Col,
  Empty,
  type GetProps,
  Input,
  Popconfirm,
  Row,
  Select,
  Tag,
  Tooltip,
} from "antd";
import {
  PlusOutlined,
  EditOutlined,
  DeleteOutlined,
  DownloadOutlined,
} from "@ant-design/icons";
import PaginationControls from "../../components/shared/Pagination";
import { useCallback, useEffect, useState } from "react";
import type { CourseResourceResponse } from "../../types/ResourceType";
import type { NotificationProps } from "../../types/Notification";
import { resourceService } from "../../services/resource/resourceService";
import CourseResourceModal from "./components/CourseResourceModal"; // Import the modal
import { useAuth } from "../../hooks";
import { downloadFile, getFileNameFromUrl } from "../../utils/FileHelper";
import { applicationRole } from "../../constants/role";
import type { Category } from "../Courses/types";
import categoryService from "../../services/category";

type SearchProps = GetProps<typeof Input.Search>;

const { Search } = Input;

const fileTypeColors: Record<string, string> = {
  pdf: "orange",
  video: "purple",
  image: "green",
  audio: "cyan",
};

export default function CourseResourcesPage() {
  const [resourcesData, setResourcesData] = useState<CourseResourceResponse[]>(
    [],
  );
  const [pageIndex, setPageIndex] = useState(1);
  const [pageSize, setPageSize] = useState(6);
  const [totalCount, setTotalCount] = useState(0);
  const [categories, setCategories] = useState<Category[]>([]);
  const [selectedCategoryId, setSelectedCategoryId] = useState<string | null>(
    null,
  );
  const [searchValue, setSearchValue] = useState<string | null>(null);
  const [notify, setNotify] = useState<NotificationProps | null>(null);
  const [loading, setLoading] = useState(false);
  const { notification } = App.useApp();
  const { user } = useAuth();
  // Modal state
  const [isModalVisible, setIsModalVisible] = useState(false);
  const [isEditing, setIsEditing] = useState(false);
  const [editingResourceId, setEditingResourceId] = useState<string | null>(
    null,
  );

  const fetchCategories = useCallback(async () => {
    try {
      const response = await categoryService.getActive()
      setCategories(response);
    } catch {
      setNotify({
        type: "error",
        message: "Error",
        description: "Failed to fetch categories",
      });
    }
  }, []);

  const fetchResources = useCallback(async () => {
    try {
      setLoading(true);
      await resourceService
        .getResources({
          pageIndex: pageIndex,
          pageSize: pageSize,
          categoryId: selectedCategoryId,
          keyWord: searchValue,
        })
        .then((response) => {
          setTotalCount(response.totalCount);
          setResourcesData(response.items);
        });
    } catch (error: any) {
      setNotify({
        type: "error",
        message: "Failed to fetch resources",
        description:
          error?.response?.data?.error || "Error fetching resources.",
      });
    } finally {
      setLoading(false);
    }
  }, [pageIndex, pageSize, searchValue, selectedCategoryId]);

  useEffect(() => {
    fetchResources();
    fetchCategories();
  }, [fetchCategories, fetchResources]);

  useEffect(() => {
    if (notify) {
      notification[notify.type]({
        message: notify.message,
        description: notify.description,
        placement: "topRight",
        showProgress: true,
        duration: 3,
        pauseOnHover: true,
      });
      setNotify(null);
    }
  }, [notify, notification]);

  const handlePageChange = useCallback((newPage: number) => {
    setPageIndex(newPage);
  }, []);

  const handlePageSizeChange = useCallback((newPageSize: number) => {
    setPageSize(newPageSize);
    setPageIndex(1);
  }, []);

  const handleSearchInput: SearchProps["onSearch"] = useCallback(
    (value: string) => {
      setSearchValue(value.trim());
      setPageIndex(1);
    },
    [],
  );

  // Modal Handlers
  const handleCreateResource = () => {
    setIsModalVisible(true);
    setIsEditing(false);
  };

  const handleEditResource = (resourceId: string) => {
    setEditingResourceId(resourceId);
    setIsModalVisible(true);
    setIsEditing(true);
  };

  const handleDeleteResource = async (resourceId: string) => {
    try {
      setLoading(true);
      await resourceService.deleteResource(resourceId);
      setNotify({
        type: "success",
        message: "Resource deleted",
        description: "The resource has been successfully deleted.",
      });
      await fetchResources(); // Refresh resources
    } catch (error: any) {
      setNotify({
        type: "error",
        message: "Failed to delete resource",
        description: error?.response?.data?.error || "Error deleting resource.",
      });
    } finally {
      setLoading(false);
    }
  };

  const handleModalCancel = () => {
    setEditingResourceId(null);
    setIsModalVisible(false);
    setIsEditing(false);
  };

  const handleModalSubmit = async (values: any) => {
    try {
      setLoading(true);
      const formData = new FormData();
      formData.append("title", values.title);
      formData.append("description", values.description || "");
      formData.append("courseId", values.courseId);
      formData.append("resource", values.resource);

      if (isEditing && editingResourceId) {
        //Edit
        await resourceService.editResource(editingResourceId, formData);
        setNotify({
          type: "success",
          message: "Resource updated",
          description: "The resource has been successfully updated.",
        });
      } else {
        //Create
        await resourceService.createResource(formData);
        setNotify({
          type: "success",
          message: "Resource created",
          description: "The resource has been successfully created.",
        });
      }
      await fetchResources(); // Refresh resources
      handleModalCancel();
    } catch (error: any) {
      setNotify({
        type: "error",
        message: "Failed to save resource",
        description: error?.response?.data?.error || "Error saving resource.",
      });
    } finally {
      setLoading(false);
    }
  };

  const handleDownload = (courseResourceId: string, filename: string) => {
    downloadFile(courseResourceId, filename);
  };

  const handleCategoryChange = (value: string) => {
    setSelectedCategoryId(value);
    setPageIndex(1);
  };

  return (
    <div className="bg-gray-800 rounded-lg overflow-hidden shadow-lg p-6">
      <div className="flex justify-between items-center gap-4 mb-8">
        <div>
          <h1 className="text-2xl font-semibold">Courses Management</h1>
          <p className="text-slate-300 text-sm">
            Manage your courses in the system
          </p>
        </div>
        <div className="w-50 sm:w-75 lg:w-100">
          <Search
            placeholder="Search resource..."
            allowClear
            size="large"
            enterButton
            onSearch={handleSearchInput}
            onChange={(e) => {
              if (e.target.value === "") {
                setSearchValue(null);
              }
            }}
          />
        </div>
      </div>

      <div className="flex justify-between mb-6 flex-wrap">
        <Select
          showSearch
          allowClear
          value={selectedCategoryId}
          placeholder="Select category"
          size="large"
          options={categories}
          filterOption={(input, option) =>
            (option?.name ?? "")
              .trim()
              .toLowerCase()
              .includes(input.trim().toLowerCase())
          }
          className="w-25 sm:w-50 lg:w-100"
          onChange={handleCategoryChange}
          onClick={fetchCategories}
          fieldNames={{ label: "name", value: "id" }}
        />
        {user?.role === applicationRole.MENTOR && (
          <Button
            type="primary"
            icon={<PlusOutlined />}
            onClick={handleCreateResource}
            name=""
            size="large"
          >
            Add New Resource
          </Button>
        )}
      </div>
      {resourcesData.length === 0 ? (
        <Empty
          description="No Data"
          image={Empty.PRESENTED_IMAGE_SIMPLE}
          className="py-4"
        />
      ) : (
        <Row gutter={[16, 16]} className="my-6">
          {resourcesData.map((resource) => (
            <Col key={resource.id} sm={24} md={12} lg={8} xxl={6}>
              <Card
                title={
                  <div className="flex justify-between gap-2">
                    <div className="truncate">{resource.title}</div>
                    <Tag
                      className="mr-0!"
                      color={
                        fileTypeColors[resource.resourceType.toLowerCase()] ||
                        fileTypeColors.default
                      }
                    >
                      {resource.resourceType}
                    </Tag>
                  </div>
                }
                loading={loading}
                className="resource-card min-h-full flex flex-col"
                variant="borderless"
                styles={{
                  body: {
                    flex: 1,
                  },
                }}
                actions={[
                  <div className="card-actions flex flex-wrap gap-4 px-6 bottom-0">
                    <Button
                      type="primary"
                      size="large"
                      className="flex-1"
                      onClick={() =>
                        handleDownload(
                          resource.id,
                          getFileNameFromUrl(resource.resourceUrl),
                        )
                      }
                      icon={<DownloadOutlined />}
                    >
                      Download
                    </Button>
                    {user?.role === applicationRole.MENTOR &&
                      user.id === resource.mentorId && (
                        <>
                          <Button
                            size="large"
                            icon={<EditOutlined />}
                            onClick={() => handleEditResource(resource.id)}
                          >
                            Edit
                          </Button>
                          <Tooltip title="Delete Resource">
                            <Popconfirm
                              title="Are you sure to delete this resource?"
                              onConfirm={() =>
                                handleDeleteResource(resource.id)
                              }
                              okText="Yes"
                              cancelText="No"
                            >
                              <Button
                                danger
                                size="large"
                                icon={<DeleteOutlined />}
                              >
                                Delete
                              </Button>
                            </Popconfirm>
                          </Tooltip>
                        </>
                      )}
                  </div>,
                ]}
              >
                <div className="card-header h-full">
                  <div className="text-gray-400 truncate">
                    {resource.description}
                  </div>
                  <div className="text-orange-300 truncate">
                    Course: {resource.courseTitle}
                  </div>
                </div>
              </Card>
            </Col>
          ))}
        </Row>
      )}

      <PaginationControls
        pageIndex={pageIndex}
        pageSize={pageSize}
        totalCount={totalCount}
        onPageChange={handlePageChange}
        onPageSizeChange={handlePageSizeChange}
        pageSizeOptions={[6, 8, 12, 24]}
      />

      <CourseResourceModal
        visible={isModalVisible}
        isEditing={isEditing}
        resourceId={editingResourceId ?? undefined}
        onCancel={handleModalCancel}
        onSubmit={handleModalSubmit}
        title={isEditing ? "Edit Resource" : "Add New Resource"}
        onText={isEditing ? "Update" : "Create"}
      />
    </div>
  );
}
