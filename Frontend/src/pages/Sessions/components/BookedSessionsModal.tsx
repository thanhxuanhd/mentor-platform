"use client"

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
import { convertUTCDateTimeToLocal } from "../../../utils/timezoneUtils"

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
  const [userTimezone, setUserTimezone] = useState<string>("")

  useEffect(() => {
    // Get user's timezone
    const timezone = Intl.DateTimeFormat().resolvedOptions().timeZone
    setUserTimezone(timezone)
  }, [])

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
    if (!userTimezone) return

    setLoading(true)
    try {
      const response = await getBookingRequestsByLearner()
      console.log("Fetched Booked Sessions:", response)
      const mappedSessions: BookedSession[] = response.map((item: SessionSlotStatusResponse) => {
        // Convert UTC date and time to local date and time
        const { localDate, localStartTime, localEndTime } = convertUTCDateTimeToLocal(
          item.day,
          item.startTime,
          item.endTime,
          userTimezone,
        )

        return {
          id: item.sessionId,
          mentor: {
            id: item.sessionId,
            name: item.mentorName,
            expertise: item.expertise || [],
            avatar: item.mentorAvatarUrl,
          },
          date: localDate, // This is now the local date
          type: item.sessionType,
          status: item.bookingStatus,
          startTime: localStartTime, // This is now the local time
          endTime: localEndTime, // This is now the local time
          originalDate: item.day, // Store original UTC date
          originalStartTime: item.startTime, // Store original UTC time
          originalEndTime: item.endTime, // Store original UTC time
        }
      })
      setSessions(mappedSessions)
    } catch (error) {
      console.error("Failed to fetch sessions:", error)
      setSessions([])
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    if (open && userTimezone) {
      fetchSessions()
    }
  }, [open, userTimezone])

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

  return (
    <Modal
      title={
        <div>
          <span className="text-white">My Booked Sessions</span>
          {userTimezone && (
            <div className="text-xs text-gray-400 mt-1">All times shown in your local timezone: {userTimezone}</div>
          )}
        </div>
      }
      open={open}
      onCancel={onCancel}
      footer={null}
      width={950}
      className="booked-sessions-modal"
      styles={{
        content: { backgroundColor: "#334155" },
        header: { backgroundColor: "#334155", borderBottom: "1px solid #475569" },
      }}
    >
      <div className="max-h-96 overflow-y-auto">
        {loading ? (
          <div className="text-center py-8">
            <p className="text-gray-400">Loading sessions...</p>
          </div>
        ) : sessions.length === 0 ? (
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
                        <p className="text-gray-400 text-sm">{session.mentor.expertise.join(", ")}</p>
                        <div className="flex items-center space-x-2 mt-1">
                          <span className="text-white text-sm">
                            {dayjs(session.date).format("MMM DD, YYYY")} at {session.startTime.slice(0, 5)} -{" "}
                            {session.endTime.slice(0, 5)}
                          </span>
                          <div className="flex items-center space-x-1 text-gray-400">
                            <Tag icon={getSessionTypeIcon(session.type)}>{formatSessionType(session.type)}</Tag>
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
                          onConfirm={() => handleCancelSession(session.id)}
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
