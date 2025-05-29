import { Avatar, Button, Tag, Input, Card } from "antd"
import { FileTextOutlined, FilePdfOutlined, FileImageOutlined } from "@ant-design/icons"
import type { MentorApplicationDetailItemProp } from "../../../types/MentorApplicationType"
import { formatDate } from "../../../utils/DateFormat"
import DefaultAvatar from "../../../assets/images/default-account.svg"

const { TextArea } = Input

interface ApplicationDetailsProps {
  application: MentorApplicationDetailItemProp | null
  onApprove?: (application: MentorApplicationDetailItemProp) => void
  onReject?: (application: MentorApplicationDetailItemProp) => void
  onRequestInfo?: (application: MentorApplicationDetailItemProp) => void
  onNoteChange?: (e: React.ChangeEvent<HTMLTextAreaElement>) => void
}

export default function MentorApplicationDetail({
  application,
  onApprove,
  onReject,
  onRequestInfo,
  onNoteChange
}: ApplicationDetailsProps) {
  if (!application) {
    return (
      <div className="bg-slate-600/50 backdrop-blur-sm rounded-xl border border-slate-500/30 shadow-xl h-full min-h-[100px] flex flex-col items-center justify-center">
        <div className="text-center space-y-4">
          <p className="text-slate-400 text-lg">Select an application to view details</p>
        </div>
      </div>
    )
  }

  const getFileIcon = (type: string) => {
    switch (type) {
      case "pdf":
        return <FilePdfOutlined className="text-red-400" />
      case "jpg":
        return <FileImageOutlined className="text-blue-400" />
      default:
        return <FileTextOutlined className="text-slate-400" />
    }
  }

  return (
    <div className="bg-slate-600/50 backdrop-blur-sm rounded-xl overflow-hidden border border-slate-500/30 shadow-xl">
      <div className="bg-gradient-to-r from-slate-600 to-slate-650 px-3 py-2 border-b flex items-center justify-between  border-slate-500/30">
        <h2 className="text-white font-semibold">Application Details</h2>

        {application.applicationStatus !== "Approved" && application.applicationStatus !== "Rejected" && (
          <div className="flex gap-2">
            <Button
              className="bg-green-500 hover:bg-green-600 border-green-500 hover:border-green-600"
              onClick={() => onApprove?.(application)}
            >
              Approve
            </Button>
            <Button danger onClick={() => onReject?.(application)}>
              Reject
            </Button>
            <Button
              type="primary"
              onClick={() => onRequestInfo?.(application)}
              disabled={application.applicationStatus === "WaitingInfo"}
            >
              Request Info
            </Button>
          </div>
        )}

      </div>

      <div className="p-6 space-y-6 overflow-y-auto h-fit max-h-[calc(100vh-150px)]">
        {/* Profile Section */}
        <div className="flex items-center gap-4">
          {application.profilePhotoUrl ? (
            <Avatar src={application.profilePhotoUrl} size={80} className="ring-2 ring-slate-400/20" />
          ) : (
            <Avatar src={DefaultAvatar} size={80} className="ring-2 ring-slate-400/20" />
          )}
          <div>
            <h3 className="text-white text-2xl font-bold mb-1">{application.mentorName}</h3>
            <p className="text-slate-300">{application.email}</p>
          </div>
        </div>

        {/* Expertise Areas */}
        <div>
          <h4 className="text-slate-300 text-sm font-medium mb-2 tracking-wide">Expertise Areas</h4>
          <div className="flex flex-wrap gap-2">
            {application.expertises && application.expertises.length > 0 ? (
              application.expertises.map((expertise, index) => (
                <Tag
                  key={index}
                  className="bg-slate-500 text-white border-slate-400 px-3 py-1 rounded-md"
                >
                  {expertise}
                </Tag>
              ))
            ) : (
              <p className="text-slate-400">Not provided</p>
            )}
          </div>
        </div>
        {/* Professional Experience */}
        <div>
          <h4 className="text-slate-300 text-sm font-medium mb-2 tracking-wide">Professional Experience</h4>
          <p className="text-slate-400">{application.experiences || "Not provided"}</p>
        </div>

        {/* Application Timeline */}
        <div>
          <h4 className="text-slate-300 text-sm font-medium mb-2 tracking-wide">Application Timeline</h4>
          <div className="flex items-center gap-3">
            <div className="w-6 h-6 bg-blue-500 rounded-full flex items-center justify-center text-white text-xs font-bold">
              1
            </div>
            <span className="text-white">Submitted on {formatDate(application.submittedAt)}</span>
          </div>
          {application.reviewedAt && application.applicationStatus !== "WaitingInfo" && (
            <div className="flex items-center gap-3 mt-2">
              <div
                className={`w-6 h-6 rounded-full flex items-center justify-center text-white text-xs font-bold ${application.applicationStatus === "Approved"
                  ? "bg-green-500"
                  : application.applicationStatus === "Rejected"
                    ? "bg-red-500"
                    : "bg-blue-500"
                  }`}
              >
                2
              </div>
              <span className="text-white">
                {application.applicationStatus === "Approved"
                  ? "Approved on"
                  : application.applicationStatus === "Rejected"
                    ? "Rejected on"
                    : "Reviewed on"}{" "}
                {formatDate(application.reviewedAt)} by {application.reviewBy || "Not provided"}
              </span>
            </div>
          )}
        </div>

        {/* Uploaded Documents */}
        <div>
          <h4 className="text-slate-300 text-sm font-medium mb-2 tracking-wide">Uploaded Documents</h4>
          <div className="space-y-2">
            {application.documents && application.documents.length > 0 ? (
              application.documents.map((doc) => (
                <Card
                  key={doc.documentId}
                  className="bg-slate-500/30 border-slate-400/30 hover:bg-slate-500/50 transition-colors cursor-pointer"
                  size="small"
                >
                  <div className="flex items-center justify-between">
                    <div className="flex items-center gap-3">
                      {getFileIcon(doc.documentType)}
                      <span className="text-white text-sm">{doc.documentUrl}</span>
                    </div>
                    <Button type="link" className="text-blue-400 hover:text-blue-300 p-0">
                      View
                    </Button>
                  </div>
                </Card>
              ))
            ) : (
              <p className="text-slate-400">No documents uploaded</p>
            )}
          </div>
        </div>
        {/* Admin Notes */}
        <div>
          <h4 className="text-slate-300 text-sm font-medium mb-3 tracking-wide">Admin Notes</h4>
          <TextArea
            placeholder="Add notes about this application..."
            value={application.note || ''}
            className="bg-slate-500/30 border-slate-400/30 text-white placeholder:text-slate-400"
            rows={4}
            maxLength={300}
            onChange={onNoteChange}
            style={{
              backgroundColor: "rgba(71, 85, 105, 0.3)",
              borderColor: "rgba(148, 163, 184, 0.3)",
              color: "white",
            }}
            disabled={application.applicationStatus === "Approved" || application.applicationStatus === "Rejected"}
          />
        </div>
      </div>
    </div>
  )
}
