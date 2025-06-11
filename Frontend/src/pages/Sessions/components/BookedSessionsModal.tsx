import { App, Avatar, Button, List, Modal, Popconfirm, Tag } from "antd"
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
import { useEffect, useState } from "react"
import { cancelBooking, getBookingRequestsByLearner } from "../../../services/session-booking/sessionBookingService"
import { formatSessionType } from "./SessionTypeSelector"
import type { NotificationProps } from "../../../types/Notification"
import type { BookedSession } from "../../../types/SessionsType"

interface SessionSlotStatusResponse {
  sessionId: string
  mentorName: string
  expertise?: string[]
  mentorAvatarUrl?: string | null
  sessionType: SessionType
  day: string
  startTime: string
  endTime: string
  bookingStatus: SessionStatus
}

interface BookedSessionsModalProps {
  open: boolean
  onCancel: () => void
  sessions: BookedSession[]
  onCancelSession: (sessionId: string) => void
}

export default function BookedSessionsModal({ open, onCancel, onCancelSession }: BookedSessionsModalProps) {
  const [sessions, setSessions] = useState<BookedSession[]>([])
  const [loading, setLoading] = useState(false)
  const [notify, setNotify] = useState<NotificationProps | null>(null)
  const { notification } = App.useApp()
  const [statusFilter, setStatusFilter] = useState<string>("All")

  useEffect(() => {
    if (notify) {
      notification[notify.type]({
        message: notify.message,
        description: notify.description,
        placement: "topRight",
      })
      setNotify(null)
    }
  }, [notify, notification])

  const fetchSessions = async () => {
    setLoading(true)
    try {
      const response = await getBookingRequestsByLearner()
      console.log("Fetched Booked Sessions:", response)
      const mappedSessions: BookedSession[] = response.map((item: SessionSlotStatusResponse) => ({
        id: item.sessionId,
        mentor: {
          id: item.sessionId,
          name: item.mentorName,
          expertise: item.expertise || [],
          avatar: item.mentorAvatarUrl,
        },
        date: dayjs(item.day).format("YYYY-MM-DD"),
        type: item.sessionType,
        status: item.bookingStatus,
        startTime: item.startTime,
        endTime: item.endTime,
      }))
      setSessions(mappedSessions)
    } catch (error) {
      console.error("Failed to fetch sessions:", error)
      setSessions([])
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    if (open) {
      fetchSessions()
    }
  }, [open])

  const handleCancelSession = async (sessionId: string) => {
    try {
      const response = await cancelBooking(sessionId)
      if (response.statusCode === 200) {
        setNotify({
          type: "success",
          message: "Cancel Session Successful",
          description: "Session cancelled successfully.",
        })
        await fetchSessions()
        onCancelSession(sessionId)
      } else {
        setNotify({
          type: "info",
          message: "Message",
          description: "Messaging functionality not implemented yet.",
        })
      }
    } catch (error: any) {
      setNotify({
        type: "error",
        message: "Cancel Session Failed",
        description: error.response?.data?.error || "An error occurred while cancelling the session. Please try again.",
      })
    }
  }

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

  const getSessionTypeIcon = (type: SessionType) => {
    switch (type) {
      case "Virtual":
        return <VideoCameraOutlined />
      case "OneOnOne":
        return <UserOutlined />
      case "Onsite":
        return <HomeOutlined />
      default:
        return <VideoCameraOutlined />
    }
  }

  const getFilteredSessions = () => {
    if (statusFilter === "All") {
      return sessions
    }
    return sessions.filter((session) => session.status === statusFilter)
  }

  return (
    <Modal
      title={
        <span className="text-white text-xl font-semibold">My Booked Sessions</span>
      }
      open={open}
      onCancel={onCancel}
      footer={null}
      width={950}
      className="booked-sessions-modal"
      styles={{
        content: {
          backgroundColor: "#1e293b",
          borderRadius: "16px",
          padding: "24px",
        },
        header: {
          backgroundColor: "#1e293b",
          borderBottom: "1px solid #475569",
          borderRadius: "16px 16px 0 0",
          padding: "20px 24px",
        },
      }}
    >
      <div>
        <div className="mb-6">
          <h3 className="text-gray-300 text-sm font-medium mb-3">Filter by Status</h3>
          <div className="flex flex-wrap gap-2">
            <Button
              type={statusFilter === "All" ? "primary" : "default"}
              onClick={() => setStatusFilter("All")}
              className={`rounded-full px-4 py-1 h-8 text-sm font-medium transition-all duration-200 ${statusFilter === "All"
                ? "bg-blue-600 border-blue-600 text-white shadow-lg"
                : "bg-slate-700 border-slate-600 text-gray-300 hover:bg-slate-600 hover:border-slate-500"
                }`}
            >
              All Sessions
            </Button>
            <Button
              type={statusFilter === "Pending" ? "primary" : "default"}
              icon={<ClockCircleOutlined />}
              onClick={() => setStatusFilter("Pending")}
              className={`rounded-full px-4 py-1 h-8 text-sm font-medium transition-all duration-200 ${statusFilter === "Pending"
                ? "bg-orange-600 border-orange-600 text-white shadow-lg"
                : "bg-slate-700 border-slate-600 text-gray-300 hover:bg-slate-600 hover:border-slate-500"
                }`}
            >
              Pending
            </Button>
            <Button
              type={statusFilter === "Approved" ? "primary" : "default"}
              icon={<CheckCircleOutlined />}
              onClick={() => setStatusFilter("Approved")}
              className={`rounded-full px-4 py-1 h-8 text-sm font-medium transition-all duration-200 ${statusFilter === "Approved"
                ? "bg-blue-600 border-blue-600 text-white shadow-lg"
                : "bg-slate-700 border-slate-600 text-gray-300 hover:bg-slate-600 hover:border-slate-500"
                }`}
            >
              Approved
            </Button>
            <Button
              type={statusFilter === "Completed" ? "primary" : "default"}
              icon={<CheckCircleOutlined />}
              onClick={() => setStatusFilter("Completed")}
              className={`rounded-full px-4 py-1 h-8 text-sm font-medium transition-all duration-200 ${statusFilter === "Completed"
                ? "bg-green-600 border-green-600 text-white shadow-lg"
                : "bg-slate-700 border-slate-600 text-gray-300 hover:bg-slate-600 hover:border-slate-500"
                }`}
            >
              Completed
            </Button>
            <Button
              type={statusFilter === "Cancelled" ? "primary" : "default"}
              icon={<CloseCircleOutlined />}
              onClick={() => setStatusFilter("Cancelled")}
              className={`rounded-full px-4 py-1 h-8 text-sm font-medium transition-all duration-200 ${statusFilter === "Cancelled"
                ? "bg-red-600 border-red-600 text-white shadow-lg"
                : "bg-slate-700 border-slate-600 text-gray-300 hover:bg-slate-600 hover:border-slate-500"
                }`}
            >
              Cancelled
            </Button>
            <Button
              type={statusFilter === "Rescheduled" ? "primary" : "default"}
              icon={<ClockCircleOutlined />}
              onClick={() => setStatusFilter("Rescheduled")}
              className={`rounded-full px-4 py-1 h-8 text-sm font-medium transition-all duration-200 ${statusFilter === "Rescheduled"
                ? "bg-purple-600 border-purple-600 text-white shadow-lg"
                : "bg-slate-700 border-slate-600 text-gray-300 hover:bg-slate-600 hover:border-slate-500"
                }`}
            >
              Rescheduled
            </Button>
          </div>
        </div>
        <div className="max-h-96 overflow-y-auto">
          {loading ? (
            <div className="text-center py-12">
              <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-500 mx-auto mb-4"></div>
              <p className="text-gray-400 text-lg">Loading sessions...</p>
            </div>
          ) : sessions.length === 0 ? (
            <div className="text-center py-16">
              <div className="bg-slate-700/50 rounded-full w-20 h-20 flex items-center justify-center mx-auto mb-6">
                <CalendarOutlined className="text-4xl text-gray-400" />
              </div>
              <h3 className="text-white text-xl font-semibold mb-2">No Sessions Yet</h3>
              <p className="text-gray-400 text-base">You haven't booked any sessions yet. Start by finding a mentor!</p>
            </div>
          ) : getFilteredSessions().length === 0 ? (
            <div className="text-center py-16">
              <div className="bg-slate-700/50 rounded-full w-20 h-20 flex items-center justify-center mx-auto mb-6">
                <CalendarOutlined className="text-4xl text-gray-400" />
              </div>
              <h3 className="text-white text-xl font-semibold mb-2">No {statusFilter} Sessions</h3>
              <p className="text-gray-400 text-base">
                No {statusFilter !== "All" ? statusFilter.toLowerCase() : ""} sessions found. Try a different filter.
              </p>
            </div>
          ) : (
            <List
              dataSource={getFilteredSessions()}
              renderItem={(session) => (
                <List.Item className="!border-none !p-0 mb-4">
                  <div className="w-full bg-slate-700/50 rounded-xl p-5 border border-slate-600/50 hover:bg-slate-700/70 transition-all duration-200">
                    <div className="flex items-start justify-between">
                      <div className="flex items-start space-x-4">
                        <div className="relative">
                          <Avatar size={56} src={session.mentor.avatar} className="border-2 border-slate-600" />
                          <div className="absolute -bottom-1 -right-1 w-4 h-4 bg-green-500 rounded-full border-2 border-slate-700"></div>
                        </div>
                        <div className="flex-1">
                          <h4 className="text-white font-semibold text-lg mb-1">{session.mentor.name}</h4>
                          <p className="text-gray-400 text-sm mb-3 line-clamp-1">
                            {session.mentor.expertise.join(", ")}
                          </p>
                          <div className="flex flex-col space-y-2">
                            <div className="flex items-center space-x-2">
                              <CalendarOutlined className="text-gray-400 text-sm" />
                              <span className="text-gray-300 text-sm font-medium">
                                {dayjs(session.date).format("MMM DD, YYYY")}
                              </span>
                            </div>
                            <div className="flex items-center space-x-2">
                              <ClockCircleOutlined className="text-gray-400 text-sm" />
                              <span className="text-gray-300 text-sm">
                                {session.startTime.slice(0, 5)} - {session.endTime.slice(0, 5)}
                              </span>
                            </div>
                            <div className="flex items-center space-x-2">
                              <Tag
                                icon={getSessionTypeIcon(session.type)}
                                className="bg-slate-600 border-slate-500 text-gray-200 rounded-lg px-3 py-1"
                              >
                                {formatSessionType(session.type)}
                              </Tag>
                            </div>
                          </div>
                        </div>
                      </div>
                      <div className="flex flex-col items-end space-y-3">
                        <Tag
                          color={getStatusColor(session.status)}
                          icon={getStatusIcon(session.status)}
                          className="capitalize font-medium px-3 py-1 rounded-lg text-sm"
                        >
                          {session.status}
                        </Tag>
                        {session.status === "Pending" && (
                          <Popconfirm
                            title={<span className="text-gray-800">Cancel Session</span>}
                            description={
                              <span className="text-gray-600">Are you sure you want to cancel this session?</span>
                            }
                            onConfirm={() => handleCancelSession(session.id)}
                            okText="Yes, Cancel"
                            cancelText="No"
                            okButtonProps={{ danger: true, className: "rounded-lg" }}
                            cancelButtonProps={{ className: "rounded-lg" }}
                          >
                            <Button
                              type="text"
                              danger
                              icon={<DeleteOutlined />}
                              size="small"
                              className="text-red-400 hover:text-red-300 hover:bg-red-500/10 rounded-lg px-3 py-1 transition-all duration-200"
                            >
                              Cancel
                            </Button>
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
      </div>
    </Modal>
  )
}