import {
  App,
  Button,
  Card,
  Col,
  Empty,
  Input,
  Popconfirm,
  Row,
  Segmented,
  Spin,
  Tag,
  Tooltip,
  type GetProps,
} from "antd";
import { PlusOutlined, EditOutlined, DeleteOutlined } from "@ant-design/icons";
import PaginationControls from "../../components/shared/Pagination";
import { useCallback, useEffect, useState } from "react";
import { FileType } from "../../types/enums/FileType";
import type { CourseResourceResponse } from "../../types/ResourceType";
import type { NotificationProps } from "../../types/Notification";
import { resourceService } from "../../services/resource/resourceService";
import CourseResourceModal from "./components/CourseResourceModal"; // Import the modal
import { useAuth } from "../../hooks";
import { downloadFile, getFileNameFromUrl } from "../../utils/FileHelper";
import { applicationRole } from "../../constants/role";

type SearchProps = GetProps<typeof Input.Search>;

const { Search } = Input;

const typeOptions = [
  { label: "All", value: null },
  { label: "Pdf", value: FileType.Pdf },
  { label: "Video", value: FileType.Video },
  { label: "Audio", value: FileType.Audio },
  { label: "Image", value: FileType.Image },
];

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
  const [selectedTypeSegment, setSelectedTypeSegment] =
    useState<FileType | null>(null);
  const [searchValue, setSearchValue] = useState<string | null>(null);
  const [notify, setNotify] = useState<NotificationProps | null>(null);
  const [loading, setLoading] = useState(false);
  const { notification } = App.useApp();
  const { user } = useAuth();
  // Modal state
  const [isModalVisible, setIsModalVisible] = useState(false);
  const [isEditing, setIsEditing] = useState(false);
  const [editingResource, setEditingResource] =
    useState<CourseResourceResponse | null>(null);

  const fetchResources = useCallback(async () => {
    try {
      setLoading(true);
      await resourceService
        .getResources({
          pageIndex: pageIndex,
          pageSize: pageSize,
          resourceType: selectedTypeSegment,
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
  }, [pageIndex, pageSize, searchValue, selectedTypeSegment]);

  useEffect(() => {
    fetchResources();
  }, [fetchResources]);

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

  const handleFilterChange = useCallback((value: FileType | null) => {
    setSelectedTypeSegment(value);
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
    setEditingResource(null);
  };

  const handleEditResource = (resource: CourseResourceResponse) => {
    setIsModalVisible(true);
    setIsEditing(true);
    setEditingResource(resource);
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
    setIsModalVisible(false);
    setIsEditing(false);
    setEditingResource(null);
  };

  const handleModalSubmit = async (values: any) => {
    try {
      setLoading(true);
      const formData = new FormData();
      formData.append("title", values.title);
      formData.append("description", values.description || "");
      formData.append("courseId", values.courseId);
      formData.append("resource", values.resource);

      if (isEditing && editingResource) {
        //Edit
        await resourceService.editResource(editingResource.id, formData);
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

  const handleDownload = (url: string, filename: string) => {
    downloadFile(url, filename);
  };

  return (
    <div className="bg-gray-800 rounded-lg overflow-hidden shadow-lg p-6">
      <div className="flex justify-between items-center mb-6">
        <h2 className="text-2xl font-semibold">Your Course Resources</h2>
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
        <Segmented<FileType | null>
          key={selectedTypeSegment}
          options={typeOptions}
          size="large"
          value={selectedTypeSegment}
          onChange={handleFilterChange}
          name="roleSegment"
          className="h-content!"
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
      {loading && <Spin tip="Loading..." size="large" />}
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
                  <div className="flex justify-between">
                    <div className="truncate">{resource.title}</div>
                    <Tag
                      color={
                        fileTypeColors[resource.resourceType.toLowerCase()] ||
                        fileTypeColors.default
                      }
                    >
                      {resource.resourceType}
                    </Tag>
                  </div>
                }
                className="resource-card"
                variant="borderless"
                actions={[
                  <div className="card-actions flex flex-wrap gap-4 px-6">
                    <Button
                      type="primary"
                      size="large"
                      className="flex-1"
                      onClick={() =>
                        handleDownload(
                          resource.resourceUrl,
                          getFileNameFromUrl(resource.resourceUrl),
                        )
                      }
                    >
                      Download
                    </Button>
                    {user?.role === applicationRole.MENTOR && (
                      <>
                        <Button
                          size="large"
                          icon={<EditOutlined />}
                          onClick={() => handleEditResource(resource)}
                        >
                          Edit
                        </Button>
                        <Tooltip title="Delete Resource">
                          <Popconfirm
                            title="Are you sure to delete this resource?"
                            onConfirm={() => handleDeleteResource(resource.id)}
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
                <div className="card-header">
                  <div className="text-gray-400 truncate">
                    {resource.description}
                  </div>
                </div>
                <div className="text-orange-300 truncate">
                  Course: {resource.courseTitle}
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
      />

      <CourseResourceModal
        visible={isModalVisible}
        isEditing={isEditing}
        initialValues={editingResource ?? undefined}
        onCancel={handleModalCancel}
        onSubmit={handleModalSubmit}
        title={isEditing ? "Edit Resource" : "Add New Resource"}
        onText={isEditing ? "Update" : "Create"}
      />
    </div>
  );
}
