import { Avatar, Button, List, Modal, Popconfirm, Tag } from "antd"
import type { Mentor } from "./MentorSelectionModal"
import {
  CalendarOutlined,
  VideoCameraOutlined,
  UserOutlined,
  HomeOutlined,
  ClockCircleOutlined,
  CheckCircleOutlined,
  CloseCircleOutlined,
  DeleteOutlined,
} from "@ant-design/icons"
import dayjs from "dayjs"
import type { SessionType } from "../../../types/enums/SessionType"
import type { SessionStatus } from "../../../types/enums/SessionStatus"

export interface BookedSession {
  id: string
  mentor: Mentor
  date: string
  time: string
  sessionType: SessionType
  status: SessionStatus
}

interface BookedSessionsModalProps {
  open: boolean
  onCancel: () => void
  sessions: BookedSession[]
  onCancelSession: (sessionId: string) => void
}

export default function BookedSessionsModal({ open, onCancel, sessions, onCancelSession }: BookedSessionsModalProps) {
  const getStatusColor = (status: string) => {
    switch (status) {
      case "Pending":
        return "orange"
      case "Approved":
        return "blue"
      case "Completed":
        return "green"
      case "Cancelled":
        return "red"
      case "Rescheduled":
        return "purple"
      default:
        return "default"
    }
  }

  const getStatusIcon = (status: string) => {
    switch (status) {
      case "Pending":
        return <ClockCircleOutlined />
      case "Approved":
        return <CheckCircleOutlined />
      case "Completed":
        return <CheckCircleOutlined />
      case "Cancelled":
        return <CloseCircleOutlined />
      case "Rescheduled":
        return <ClockCircleOutlined />
      default:
        return <ClockCircleOutlined />
    }
  }

  const getSessionTypeIcon = (type: string) => {
    switch (type) {
      case "virtual":
        return <VideoCameraOutlined />
      case "in-person":
        return <UserOutlined />
      case "on-site":
        return <HomeOutlined />
      default:
        return <VideoCameraOutlined />
    }
  }

  return (
    <Modal
      title={<span className="text-white">My Booked Sessions</span>}
      open={open}
      onCancel={onCancel}
      footer={null}
      width={900}
      className="booked-sessions-modal"
      styles={{
        content: { backgroundColor: "#334155" },
        header: { backgroundColor: "#334155", borderBottom: "1px solid #475569" },
      }}
    >
      <div className="max-h-96 overflow-y-auto">
        {sessions.length === 0 ? (
          <div className="text-center py-8">
            <CalendarOutlined className="text-4xl text-gray-400 mb-4" />
            <p className="text-gray-400">No sessions booked yet</p>
          </div>
        ) : (
          <List
            dataSource={sessions}
            renderItem={(session) => (
              <List.Item className="border-b border-slate-600 last:border-b-0">
                <div className="w-full">
                  <div className="flex items-start justify-between">
                    <div className="flex items-center space-x-4">
                      <Avatar size={50} src={session.mentor.avatar} />
                      <div className="ml-3">
                        <h4 className="text-white font-medium">{session.mentor.name}</h4>
                        <p className="text-gray-400 text-sm">{session.mentor.expertise}</p>
                        <div className="flex items-center space-x-2 mt-1">
                          <span className="text-white text-sm">
                            {dayjs(session.date).format("MMM DD, YYYY")} at {session.time}
                          </span>
                          <div className="flex items-center space-x-1 text-gray-400">
                            {getSessionTypeIcon(session.sessionType)}
                            <span className="text-xs capitalize">{session.sessionType.replace("-", " ")}</span>
                          </div>
                        </div>
                      </div>
                    </div>
                    <div className="flex items-center space-x-2">
                      <Tag
                        color={getStatusColor(session.status)}
                        icon={getStatusIcon(session.status)}
                        className="capitalize"
                      >
                        {session.status}
                      </Tag>
                      {session.status === "Pending" && (
                        <Popconfirm
                          title="Cancel Session"
                          description="Are you sure you want to cancel this session?"
                          onConfirm={() => onCancelSession(session.id)}
                          okText="Yes, Cancel"
                          cancelText="No"
                          okButtonProps={{ danger: true }}
                        >
                          <Button
                            type="text"
                            danger
                            icon={<DeleteOutlined />}
                            size="small"
                            className="text-red-400 hover:text-red-300"
                          />
                        </Popconfirm>
                      )}
                    </div>
                  </div>
                </div>
              </List.Item>
            )}
          />
        )}
      </div>
    </Modal>
  )
}