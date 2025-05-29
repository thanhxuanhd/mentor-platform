import { Avatar, List, App, Pagination } from "antd"
import { UserOutlined, CalendarOutlined, TagOutlined } from "@ant-design/icons"
import { ApplicationStatus } from "../../types/enums/ApplicationStatus"
import { useCallback, useEffect, useState } from "react"
import MentorApplicationDetail from "./components/MentorApplicationDetail"
import MentorApplicationFilter from "./components/MentorApplicationFilter"
import type { MentorApplicationDetailItemProp, MentorApplicationFilterProp, MentorApplicationListItemProp } from "../../types/MentorApplicationType"
import type { NotificationProps } from "../../types/Notification"
import { formatDate } from "../../utils/DateFormat"
import { mentorApplicationService } from "../../services/mentorAppplications/mentorApplicationService"
import { renderStatusTag } from "./utils/renderStatusTag"
import DefaultAvatar from "../../assets/images/default-account.svg"

const defaultMentorApplicationFilter: MentorApplicationFilterProp = {
  pageSize: 3,
  pageIndex: 1,
  status: ApplicationStatus.Submitted,
  keyword: "",
}

export default function MentorApplicationPage() {
  const [filters, setFilters] = useState<MentorApplicationFilterProp>(defaultMentorApplicationFilter);
  const [mentorApplications, setMentorApplications] = useState<MentorApplicationListItemProp[]>([]);
  const [pagination, setPagination] = useState({
    pageIndex: filters.pageIndex || 1,
    totalCount: 3,
    pageSize: filters.pageSize || 3,
  });
  const [selectedApplication, setSelectedApplication] = useState<MentorApplicationDetailItemProp | null>(null);
  const [loading, setLoading] = useState(false);
  const [notify, setNotify] = useState<NotificationProps | null>(null);
  const [applicationCount, setApplicationCount] = useState(0);
  const { notification } = App.useApp();

  const fetchApplications = useCallback(async (filters: MentorApplicationFilterProp) => {
    try {
      setLoading(true);
      const response = await mentorApplicationService.getMentorApplications(filters);
      setMentorApplications(response.items);
      setPagination({
        ...pagination,
        pageIndex: response.pageIndex,
        totalCount: response.totalCount,
        pageSize: response.pageSize,
      });
      setApplicationCount(response.totalCount);
    } catch (error: any) {
      setNotify({
        type: "error",
        message: "Failed to fetch applications",
        description: error?.response?.data?.error || "Error fetching applications.",
      });
    } finally {
      setLoading(false);
    }
  }, [filters]);

  useEffect(() => {
    fetchApplications(filters);
  }, [fetchApplications]);

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

  const fetchApplicationDetail = useCallback(async (applicationId: string) => {
    try {
      setLoading(true);
      const response = await mentorApplicationService.getMentorApplicationById(applicationId);
      setSelectedApplication(response);
    } catch (error: any) {
      setNotify({
        type: "error",
        message: "Failed to fetch application details",
        description: error?.response?.data?.error || "Error fetching application details.",
      });
    } finally {
      setLoading(false);
    }
  }, []);

  const handleFilterChange = (newFilters: MentorApplicationFilterProp) => {
    setFilters({
      ...filters,
      ...newFilters,
      pageIndex: Object.keys(newFilters).some((key) => key !== "pageIndex") ? 1 : newFilters.pageIndex ?? filters.pageIndex,
    });
  };

  const handleApplicationClick = async (application: any) => {
    await fetchApplicationDetail(application.mentorApplicationId)
  }

  const handleNoteEmpty = () => {
    if (!selectedApplication) return false;
    const isEmpty = selectedApplication.note?.trim() === '';
    debugger;
    console.log("Note is empty:", isEmpty);
    return isEmpty;
  }

  const handleApprove = async () => {
    if (!selectedApplication) return;
    if (handleNoteEmpty()) {
      setNotify({
        type: "warning",
        message: "Note Required",
        description: "Please provide a note before approving the application.",
      });
      return;
    }
    try {
      await mentorApplicationService.updateMentorApplicationStatus(selectedApplication.mentorApplicationId, ApplicationStatus.Approved, selectedApplication.note || '');
      setNotify({
        type: "success",
        message: "Application Approved",
        description: `You have approved ${selectedApplication.mentorName}'s application.`,
      });
      await fetchApplications(filters);
      await fetchApplicationDetail(selectedApplication.mentorApplicationId);
    } catch (error: any) {
      setNotify({
        type: "error",
        message: "Failed to approve application",
        description: error?.response?.data?.error || "Error approving application.",
      });
      return;
    }
  }

  const handleReject = async () => {
    if (!selectedApplication) return;
    if (handleNoteEmpty()) {
      setNotify({
        type: "warning",
        message: "Note Required",
        description: "Please provide a note before rejecting the application.",
      });
      return;
    }
    try {
      await mentorApplicationService.updateMentorApplicationStatus(selectedApplication.mentorApplicationId, ApplicationStatus.Rejected, selectedApplication.note || '');
      setNotify({
        type: "success",
        message: "Application Rejected",
        description: `You have rejected ${selectedApplication.mentorName}'s application.`,
      });
      await fetchApplications(filters);
      await fetchApplicationDetail(selectedApplication.mentorApplicationId);
    } catch (error: any) {
      setNotify({
        type: "error",
        message: "Failed to reject application",
        description: error?.response?.data?.error || "Error rejecting application.",
      });
      return;
    }
  }

  const handleRequestInfo = async () => {
    if (!selectedApplication) return;
    if (handleNoteEmpty()) {
      setNotify({
        type: "warning",
        message: "Note Required",
        description: "Please provide a note before send request info to the application.",
      });
      return;
    }
    try {
      await mentorApplicationService.requestMentorApplicationInfo(selectedApplication.mentorApplicationId, selectedApplication.note!);

      setNotify({
        type: "success",
        message: "Requested Additional Info",
        description: `You have requested additional info from ${selectedApplication.mentorName}.`,
      });
      await fetchApplications(filters);
      await fetchApplicationDetail(selectedApplication.mentorApplicationId);
    } catch (error: any) {
      setNotify({
        type: "error",
        message: "Failed to request additional info",
        description: error?.response?.data?.error || "Error requesting additional info.",
      });
      return;
    }
  }

  return (
    <div className="bg-gray-800 rounded-lg overflow-hidden shadow-lg p-6">
      <div className="flex flex-col justify-between items-start mb-6">
        <h2 className="text-2xl font-semibold">Mentor Applications</h2>
        <p className="text-slate-300 text-sm mb-6">Review and manage pending mentor applications</p>
        <MentorApplicationFilter onFilterChange={handleFilterChange} />
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 mb-6">
        <div>
          <div className="bg-slate-600/50 backdrop-blur-sm rounded-xl overflow-hidden border border-slate-500/30 shadow-xl">
            <div className="bg-gradient-to-r from-slate-600 to-slate-650 px-3 py-2 border-b border-slate-500/30">
              <div className="flex items-center gap-2">
                <UserOutlined className="text-slate-300" />
                <h2 className="text-white font-semibold">Applications Review</h2>
                <div className="ml-auto bg-orange-500/20 text-orange-300 px-3 py-1 rounded-full text-sm font-medium">
                  {applicationCount} applications
                </div>
              </div>
            </div>

            <List
              className="bg-transparent"
              dataSource={mentorApplications}
              loading={loading}
              renderItem={(mentorApplication) => (
                <List.Item
                  key={mentorApplication.mentorApplicationId}
                  className="!p-4 group !border-b last:!border-b-0 cursor-pointer transition-all duration-300 hover:bg-slate-500/30 hover:scale-[1.01] relative py-5"
                  onClick={() => handleApplicationClick(mentorApplication)}
                >
                  <List.Item.Meta
                    avatar={
                      <div className="relative">
                        {mentorApplication.profilePhotoUrl ? (
                          <Avatar
                            src={mentorApplication.profilePhotoUrl}
                            alt={mentorApplication.mentorName}
                            size={56}
                            className="ring-2 ring-slate-400/20"
                          />
                        ) : (
                          <Avatar
                            src={DefaultAvatar}
                            alt="Default Avatar"
                            size={56}
                            className="ring-2 ring-slate-400/20"
                          />
                        )}
                      </div>
                    }
                    title={
                      <div className="flex items-start justify-between">
                        <h3 className="text-white font-bold text-[18px] leading-tight group-hover:text-blue-300 transition-colors duration-300">
                          {mentorApplication.mentorName}
                        </h3>
                      </div>
                    }
                    description={
                      <div className="space-y-2">
                        <div className="flex items-start gap-2">
                          <TagOutlined className="text-slate-400 mt-1 text-xs" />
                          <p className="text-slate-300 text-sm leading-relaxed group-hover:text-slate-200 transition-colors duration-300">
                            {mentorApplication.expertises.join(", ")}
                          </p>
                        </div>

                        <div className="flex items-center justify-between">
                          <div className="flex items-center gap-1">
                            {renderStatusTag(mentorApplication.status)}
                            <div className="flex items-center gap-1 text-slate-400 text-xs">
                              <CalendarOutlined />
                              <span>Submitted: {formatDate(mentorApplication.submittedAt)}</span>
                            </div>
                          </div>

                          <div className="opacity-0 group-hover:opacity-100 transition-all duration-300 transform translate-x-2 group-hover:translate-x-0">
                            <div className="text-blue-400 text-sm font-medium">Click to review →</div>
                          </div>
                        </div>
                      </div>
                    }
                  />
                </List.Item>
              )}
            />
          </div>

          <div className="mt-6 mb-4 text-center">
            <p className="text-slate-400 text-sm">Click on any application to view detailed information and take action</p>
          </div>

          <Pagination
            align="center"
            pageSize={pagination.pageSize}
            current={pagination.pageIndex}
            total={pagination.totalCount}
            onChange={(page, pageSize) => {
              setFilters({
                ...filters,
                pageIndex: page,
                pageSize: pageSize || filters.pageSize,
              });
            }}
          />
        </div>
        <MentorApplicationDetail
          application={selectedApplication}
          onApprove={handleApprove}
          onReject={handleReject}
          onRequestInfo={handleRequestInfo}
          onNoteChange={(e) => {
            if (selectedApplication) {
              setSelectedApplication({
                ...selectedApplication,
                note: e.target.value,
              });
            }
          }}
        />
      </div>
    </div>
  )
}
