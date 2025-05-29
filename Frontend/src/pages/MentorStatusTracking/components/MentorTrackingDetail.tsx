import { Avatar, Button, Tag, Timeline } from "antd"
import {
  CheckCircleOutlined,
  ClockCircleOutlined,
  ExclamationCircleOutlined,
  FileTextOutlined,
  CalendarOutlined,
  UserOutlined,
  InfoCircleOutlined,
} from "@ant-design/icons"
import type { MentorApplicationDetailItemProp } from "../../../types/MentorApplicationType";
import { formatDate } from "../../../utils/DateFormat";
import DefaultAvatar from "../../../assets/images/default-account.svg"
import { renderStatusTag } from "../../MentorApplication/utils/renderStatusTag";

export default function MentorStatusTrackingDetail({ onBackToList, application }: {
  onBackToList: () => void;
  application: MentorApplicationDetailItemProp
}) {
  const getTimelineIcon = (type: string) => {
    switch (type) {
      case "success":
        return <CheckCircleOutlined className="text-green-500" />
      case "processing":
        return <ClockCircleOutlined className="text-blue-500" />
      case "error":
        return <ExclamationCircleOutlined className="text-red-500" />
      case "warning":
        return <InfoCircleOutlined className="text-yellow-500" />
      default:
        return <ClockCircleOutlined className="text-gray-500" />
    }
  }

  // Generate a basic timeline based on available data
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
    key: string
    date: string
    status: string
    description: string
    type: "success" | "processing" | "error" | "warning"
  }>

  return (
    <div className="bg-gray-800 rounded-lg overflow-hidden shadow-lg p-6">
      {/* Header */}
      <div className="flex flex-col justify-between items-start mb-6">
        <h2 className="text-2xl font-semibold">Application Status</h2>
        <p className="text-slate-300 text-sm">Track your mentor application progress</p>
      </div>


      {/* Status Overview Section */}
      <div className="mb-4 border border-slate-500/30 rounded-lg backdrop-blur-sm p-6">
        <div className="flex items-center justify-between mb-4">
          <div className="flex items-center gap-4">
            <Avatar src={application.profilePhotoUrl || DefaultAvatar} size={80} className="ring-2 ring-slate-400/20" />
            <div>
              <h2 className="text-white text-xl font-bold mb-1">
                {application.mentorName}
              </h2>
              <p className="text-slate-300 mb-2">{application.email || "No email provided"}</p>
            </div>
          </div>
          <div className="text-right">
            {renderStatusTag(application.applicationStatus)}
            <p className="text-slate-400 text-sm mt-2">Last reviewed: {formatDate(application.reviewedAt || application.submittedAt)}</p>
          </div>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div className="rounded-lg p-4">
            <div className="flex items-center gap-2 mb-2">
              <CalendarOutlined className="text-blue-400" />
              <span className="text-slate-300 font-medium">Submitted</span>
            </div>
            <p className="text-white text-base">
              {formatDate(application.submittedAt)}
            </p>
          </div>

          <div className="rounded-lg p-4">
            <div className="flex items-center gap-2 mb-2">
              <UserOutlined className="text-green-400" />
              <span className="text-slate-300 font-medium">Professional Experiences</span>
            </div>
            <p className="text-white text-base">
              {application.experiences || "No experience provided"}
            </p>
          </div>

          <div className="rounded-lg p-4">
            <div className="flex items-center gap-2 mb-2">
              <FileTextOutlined className="text-purple-400" />
              <span className="text-slate-300 font-medium">Uploaded Documents</span>
            </div>
            <p className="text-white text-base">
              {application.documents?.length || 0} files uploaded
            </p>
          </div>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
        {/* Application Timeline */}
        <div className="border border-slate-500/30 rounded-lg backdrop-blur-sm p-6">
          <h3 className="text-white text-lg font-semibold mb-4">Application Timeline</h3>
          <Timeline className="mt-4">
            {timeline.map((item) => (
              <Timeline.Item key={item.key} dot={getTimelineIcon(item.type)} className="pb-6 bg-transparent">
                <div className="ml-2">
                  <div className="flex items-center justify-between mb-1">
                    <h4 className="text-white font-semibold">{item.status}</h4>
                    <span className="text-slate-400 text-sm">{formatDate(item.date)}</span>
                  </div>
                  <p className="text-slate-300 text-sm leading-relaxed">{item.description}</p>
                </div>
              </Timeline.Item>
            ))}
          </Timeline>
        </div>

        {/* Application Details */}
        <div className="space-y-4">
          {/* Education */}
          <div className="border border-slate-500/30 rounded-lg backdrop-blur-sm p-6">
            <h3 className="text-white text-lg font-semibold mb-4">Education</h3>
            <p className="text-slate-200 text-base">
              {application.education || "No education information provided"}
            </p>
          </div>

          {/* Certifications */}
          <div className="border border-slate-500/30 rounded-lg backdrop-blur-sm p-6">
            <h3 className="text-white text-lg font-semibold mb-4">Certifications</h3>
            <p className="text-slate-200 text-base">
              {application.certifications || "No certifications provided"}
            </p>
          </div>

          {/* Statement */}
          <div className="border border-slate-500/30 rounded-lg backdrop-blur-sm p-6">
            <h3 className="text-white text-lg font-semibold mb-4">Statement</h3>
            <p className="text-slate-200 text-base leading-relaxed">
              {application.statement || "No statement provided"}
            </p>
          </div>

          {/* Uploaded Documents */}
          <div className="border border-slate-500/30 rounded-lg backdrop-blur-sm p-6">
            <h3 className="text-white text-lg font-semibold mb-4">Uploaded Documents</h3>
            <div className="space-y-3">
              {application.documents?.map((doc) => (
                <div key={doc.documentId} className="flex items-center justify-between rounded-lg p-3">
                  <div className="flex items-center gap-3">
                    <FileTextOutlined className="text-blue-400" />
                    <div>
                      <p className="text-white font-medium text-base">
                        {doc.documentUrl}
                      </p>
                      <p className="text-slate-400 text-sm">Uploaded: {formatDate(application.submittedAt)}</p>
                    </div>
                  </div>
                  <Tag color="green" className="border-0 text-sm">
                    Verified
                  </Tag>
                </div>
              )) || <p className="text-slate-400">No documents uploaded</p>}
            </div>
          </div>

          {/* Next Steps */}
          {application.note && (
            <div className="border border-slate-500/30 rounded-lg backdrop-blur-sm p-6">
              <h3 className="text-white text-lg font-semibold mb-4">Next Steps</h3>
              <div className="border border-blue-500/20 rounded-lg p-4">
                <div className="flex items-start gap-3">
                  <InfoCircleOutlined className="text-blue-400 mt-1" />
                  <p className="text-slate-200 leading-relaxed text-base">
                    {application.note}
                  </p>
                </div>
              </div>
            </div>
          )}
        </div>
      </div>
      <div className="mt-6 flex justify-end">
        <Button type="primary" onClick={onBackToList} className="bg-blue-600 hover:bg-blue-700">
          Back
        </Button>
      </div>
    </div>
  )
}