import { Avatar, Button, Timeline } from "antd";
import {
  CheckCircleOutlined,
  ClockCircleOutlined,
  ExclamationCircleOutlined,
  FileTextOutlined,
  CalendarOutlined,
  UserOutlined,
  InfoCircleOutlined,
  EditOutlined,
} from "@ant-design/icons";
import type { MentorApplicationDetailItemProp } from "../../../types/MentorApplicationType";
import { formatDate } from "../../../utils/DateFormat";
import DefaultAvatar from "../../../assets/images/default-account.svg";
import { renderStatusTag } from "../../MentorApplication/utils/renderStatusTag";
import { normalizeServerFiles } from "../../../utils/InputNormalizer";
import { useNavigate } from "react-router-dom";

export default function MentorStatusTrackingDetail({
  onBackToList,
  application,
}: {
  onBackToList: () => void;
  application: MentorApplicationDetailItemProp;
}) {
  const navigate = useNavigate();
  const getTimelineIcon = (type: string) => {
    switch (type) {
      case "success":
        return <CheckCircleOutlined className="text-green-500" />;
      case "processing":
        return <ClockCircleOutlined className="text-blue-500" />;
      case "error":
        return <ExclamationCircleOutlined className="text-red-500" />;
      case "warning":
        return <InfoCircleOutlined className="text-yellow-500" />;
      default:
        return <ClockCircleOutlined className="text-gray-500" />;
    }
  };

  const viewFileInBrowser = (fileUrl: string): void => {
    window.open(fileUrl, "_blank");
  };

  const handleEdit = (application: MentorApplicationDetailItemProp) => {
    navigate("/mentor-application/edit", {
      state: { application, isEditMode: true },
    });
  };

  const timeline = [
    {
      key: "1",
      date: application.submittedAt,
      status: "Application Submitted",
      description: "Your mentor application has been successfully submitted.",
      type: "success" as const,
    },
    application.reviewedAt && {
      key: "2",
      date: application.reviewedAt,
      status: `Reviewed by ${application.reviewBy || "Admin"}`,
      description: "Your application has been reviewed.",
      type: "processing" as const,
    },
  ].filter(Boolean) as Array<{
    key: string;
    date: string;
    status: string;
    description: string;
    type: "success" | "processing" | "error" | "warning";
  }>;

  return (
    <div className="bg-gray-800 rounded-lg overflow-hidden shadow-lg p-6">
      {/* Header */}
      <div className="flex justify-between items-center">
        <div className="flex flex-col justify-between items-start mb-6">
          <h2 className="text-2xl font-semibold">Application Status</h2>
          <p className="text-gray-400 text-sm mt-1">
            Track your mentor application progress
          </p>
        </div>

        <div className="flex justify-end mb-4">
          {application.applicationStatus === "WaitingInfo" && (
            <Button
              type="primary"
              onClick={() => handleEdit(application)}
              className="bg-orange-500 hover:bg-orange-600 transition-all duration-300 text-base px-6 py-2 rounded-lg"
            >
              <EditOutlined /> Edit
            </Button>
          )}
        </div>
      </div>

      {/* Status Overview Section */}
      <div className="mb-4 bg-gray-800/50 border border-gray-700 rounded-xl p-6 backdrop-blur-sm">
        <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between mb-4">
          <div className="flex items-center gap-4">
            <Avatar
              src={application.profilePhotoUrl || DefaultAvatar}
              size={90}
              className="ring-4 ring-gray-600/20 transition-all duration-300"
            />
            <div>
              <h2 className="text-2xl font-semibold text-white">
                {application.mentorName}
              </h2>
              <p className="text-gray-300 text-sm">
                {application.email || "No email provided"}
              </p>
            </div>
          </div>
          <div className="mt-4 sm:mt-0 text-right">
            {renderStatusTag(application.applicationStatus)}
            <p className="text-gray-400 text-sm mt-2">
              Last reviewed:{" "}
              {formatDate(application.reviewedAt || application.submittedAt)}
            </p>
          </div>
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
          <div className="bg-gray-700/30 rounded-lg p-4 hover:bg-gray-700/50">
            <div className="flex items-center gap-2 mb-2">
              <CalendarOutlined className="text-blue-400" />
              <span className="text-gray-200 font-medium">Submitted</span>
            </div>
            <p className="text-white text-base">
              {formatDate(application.submittedAt)}
            </p>
          </div>

          <div className="bg-gray-700/30 rounded-lg p-4 hover:bg-gray-700/50">
            <div className="flex items-center gap-2 mb-2">
              <UserOutlined className="text-green-400" />
              <span className="text-gray-200 font-medium">
                Professional Experiences
              </span>
            </div>
            <p className="text-white text-base">
              {application.experiences || "No experience provided"}
            </p>
          </div>

          <div className="bg-gray-700/30 rounded-lg p-4 hover:bg-gray-700/50">
            <div className="flex items-center gap-2 mb-2">
              <FileTextOutlined className="text-purple-400" />
              <span className="text-gray-200 font-medium">
                Uploaded Documents
              </span>
            </div>
            <p className="text-white text-base">
              {application.documents?.length || 0} files uploaded
            </p>
          </div>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
        {/* Application Timeline */}
        <div className="space-y-4">
          <div className="bg-gray-800/50 border border-gray-700 rounded-xl p-6 backdrop-blur-sm">
            <h3 className="text-xl font-semibold text-white mb-4">
              Application Timeline
            </h3>
            <Timeline className="mt-4">
              {timeline.map((item) => (
                <Timeline.Item
                  key={item.key}
                  dot={getTimelineIcon(item.type)}
                  className="pb-6 bg-transparent"
                >
                  <div className="ml-4">
                    <div className="flex items-center justify-between mb-1">
                      <h4 className="text-white font-semibold">
                        {item.status}
                      </h4>
                      <span className="text-gray-400 text-sm">
                        {formatDate(item.date)}
                      </span>
                    </div>
                    <p className="text-gray-300 text-sm leading-relaxed">
                      {item.description}
                    </p>
                  </div>
                </Timeline.Item>
              ))}
            </Timeline>
          </div>

          {/* Admin Notes */}
          {application.note && (
            <div className="bg-gray-800/50 border border-gray-700 rounded-xl p-6 backdrop-blur-sm">
              <h3 className="text-xl font-semibold text-white mb-4">
                Admin Notes
              </h3>
              <div className="bg-gray-700/30 border border-blue-500/20 rounded-lg p-4">
                <div className="flex items-start gap-3">
                  <InfoCircleOutlined className="text-blue-400 mt-1" />
                  <p className="text-gray-200 leading-relaxed text-base">
                    {application.note}
                  </p>
                </div>
              </div>
            </div>
          )}
        </div>

        {/* Application Details */}
        <div className="space-y-4">
          {/* Education */}
          <div className="bg-gray-800/50 border border-gray-700 rounded-xl p-6 backdrop-blur-sm">
            <h3 className="text-xl font-semibold text-white mb-4">Education</h3>
            <p className="text-gray-200 text-base leading-relaxed">
              {application.education || "No education information provided"}
            </p>
          </div>

          {/* Certifications */}
          <div className="bg-gray-800/50 border border-gray-700 rounded-xl p-6 backdrop-blur-sm">
            <h3 className="text-xl font-semibold text-white mb-4">
              Certifications
            </h3>
            <p className="text-gray-200 text-base leading-relaxed">
              {application.certifications || "No certifications provided"}
            </p>
          </div>

          {/* Statement */}
          <div className="bg-gray-800/50 border border-gray-700 rounded-xl p-6 backdrop-blur-sm">
            <h3 className="text-xl font-semibold text-white mb-4">Statement</h3>
            <p className="text-gray-200 text-base leading-relaxed">
              {application.statement || "No statement provided"}
            </p>
          </div>

          {/* Uploaded Documents */}
          <div className="bg-gray-800/50 border border-gray-700 rounded-xl p-6 backdrop-blur-sm">
            <h3 className="text-xl font-semibold text-white mb-4">
              Uploaded Documents
            </h3>
            <div className="space-y-3">
              {application.documents?.map((doc) => (
                <div
                  key={doc.documentId}
                  className="flex items-center justify-between bg-gray-700/30 rounded-lg p-3 transition-all hover:bg-gray-700/50"
                >
                  <div className="flex items-center gap-3">
                    <FileTextOutlined className="text-blue-400" />
                    <div>
                      <p className="text-white font-medium text-base">
                        {normalizeServerFiles(doc.documentUrl)}
                      </p>
                      <p className="text-gray-400 text-sm">
                        Uploaded: {formatDate(application.submittedAt)}
                      </p>
                    </div>
                  </div>
                  <Button
                    type="link"
                    className="text-blue-400 hover:text-blue-300 p-0"
                    onClick={() => viewFileInBrowser(doc.documentUrl)}
                  >
                    View
                  </Button>
                </div>
              )) || <p className="text-gray-400">No documents uploaded</p>}
            </div>
          </div>
        </div>
      </div>

      {/* Footer with Back and Edit Buttons */}
      <div className="mt-8 flex justify-end gap-4">
        <Button
          type="primary"
          onClick={onBackToList}
          className="bg-blue-600 hover:bg-blue-700 transition-all duration-300 text-base px-6 py-2 rounded-lg"
        >
          Back
        </Button>
      </div>
    </div>
  );
}
