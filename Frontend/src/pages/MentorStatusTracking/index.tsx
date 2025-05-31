import { useCallback, useEffect, useState } from "react";
import { Avatar, List, Card, App, Button } from "antd";
import {
  CalendarOutlined,
  UserOutlined,
  TagOutlined,
  PlusOutlined,
} from "@ant-design/icons";
import MentorStatusTrackingDetail from "./components/MentorTrackingDetail";
import { renderStatusTag } from "../MentorApplication/utils/renderStatusTag";
import { formatDate } from "../../utils/DateFormat";
import type {
  MentorApplicationDetailItemProp,
  MentorApplicationListItemProp,
} from "../../types/MentorApplicationType";
import { mentorApplicationService } from "../../services/mentorAppplications/mentorApplicationService";
import type { NotificationProps } from "../../types/Notification";
import { useAuth } from "../../hooks";
import DefaultAvatar from "../../assets/images/default-account.svg";
import { useNavigate } from "react-router-dom";

export default function MentorStatusTrackingPage() {
  const [selectedApplication, setSelectedApplication] =
    useState<MentorApplicationDetailItemProp | null>(null);
  const [applicationHistory, setApplicationHistory] = useState<
    MentorApplicationListItemProp[]
  >([]);
  const [loading, setLoading] = useState(false);
  const [notify, setNotify] = useState<NotificationProps | null>(null);
  const [applicationCount, setApplicationCount] = useState(0);
  const { notification } = App.useApp();
  const { user } = useAuth();
  const navigate = useNavigate();

  const handleViewDetails = async (applicationId: string) => {
    if (!applicationId) return;
    await fetchApplicationDetails(applicationId);
  };

  const fetchApplicationDetails = useCallback(async (id: string) => {
    try {
      setLoading(true);
      const response =
        await mentorApplicationService.getMentorApplicationById(id);
      setSelectedApplication(response);
    } catch (error: any) {
      setNotify({
        type: "error",
        message: "Failed to fetch application details",
        description:
          error?.response?.data?.error || "Error fetching application details.",
      });
    } finally {
      setLoading(false);
    }
  }, []);

  const handleBackToList = () => {
    setSelectedApplication(null);
  };

  const fetchApplications = useCallback(async (id: string) => {
    try {
      setLoading(true);
      const response =
        await mentorApplicationService.getMentorApplicationByMentorId(id);
      setApplicationHistory(response);
      setApplicationCount(response.length);
    } catch (error: any) {
      setNotify({
        type: "error",
        message: "Failed to fetch applications",
        description:
          error?.response?.data?.error || "Error fetching applications.",
      });
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchApplications(user?.id || "");
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

  if (selectedApplication) {
    return (
      <MentorStatusTrackingDetail
        onBackToList={handleBackToList}
        application={selectedApplication}
      />
    );
  }

  function handleClickAdd(): void {
    navigate("/mentor-application");
  }

  return (
    <div className="bg-gray-800 rounded-lg overflow-hidden shadow-lg p-6">
      {/* Header */}
      <div className="flex justify-between items-center">
        <div className="flex flex-col justify-between items-start mb-6">
          <h2 className="text-2xl font-semibold">
            Application History Tracking
          </h2>
          <p className="text-slate-300 text-sm">
            View your mentor application submissions
          </p>
        </div>

        <div className="flex justify-end mb-4">
          {applicationHistory?.every((app) => app.status === "Rejected") && (
            <Button
              type="primary"
              onClick={() => handleClickAdd()}
              size="large"
              className="bg-orange-500 hover:bg-orange-600 transition-all duration-300 text-base px-6 py-2 rounded-lg"
            >
              <PlusOutlined /> Create New Application
            </Button>
          )}
        </div>
      </div>

      {/* Applications List */}
      <Card className="border-slate-500/30 backdrop-blur-sm">
        <div className="bg-gradient-to-r from-slate-600 to-slate-650 px-4 py-4 border-b rounded-t-xl border-slate-500/30 -mx-6 -mt-6 mb-6">
          <div className="flex items-center gap-2">
            <UserOutlined className="text-slate-300" />
            <h2 className="text-white text-lg font-semibold">
              All Applications
            </h2>
            <div className="ml-auto bg-blue-500/20 text-blue-300 px-3 py-1 rounded-full text-sm font-medium">
              {applicationCount} total
            </div>
          </div>
        </div>

        <List
          className="bg-transparent"
          dataSource={applicationHistory}
          loading={loading}
          renderItem={(mentorApplication) => (
            <List.Item
              key={mentorApplication.mentorApplicationId}
              className="!p-4 group !border-b last:!border-b-0 cursor-pointer transition-all duration-300 hover:bg-slate-500/30 hover:scale-[1.01] relative py-5"
              onClick={() =>
                handleViewDetails(mentorApplication.mentorApplicationId)
              }
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
                          <span>
                            Submitted:{" "}
                            {formatDate(mentorApplication.submittedAt)}
                          </span>
                        </div>
                      </div>

                      <div className="opacity-0 group-hover:opacity-100 transition-all duration-300 transform translate-x-2 group-hover:translate-x-0">
                        <div className="text-blue-400 text-sm font-medium">
                          Click to review →
                        </div>
                      </div>
                    </div>
                  </div>
                }
              />
            </List.Item>
          )}
        />
      </Card>

      <div className="mt-6 text-center">
        <p className="text-slate-400 text-sm">
          Click on any application to view detailed status and timeline
        </p>
      </div>
    </div>
  );
}
