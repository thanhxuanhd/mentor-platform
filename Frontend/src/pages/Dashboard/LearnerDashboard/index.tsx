"use client"

import { useState, useEffect } from "react"
import { Card, Avatar, List, App } from "antd"
import { CalendarOutlined } from "@ant-design/icons"
import {
  learnerDashboardService,
  type GetLearnerDashboardResponse,
  type LearnerUpcomingSessionResponse,
} from "../../../services/learnerDashboard/learnerDashboardService"
import dayjs from "dayjs"
import type { NotificationProps } from "../../../types/Notification"
import DefaultAvatar from "../../../assets/images/default-account.svg"
import { Modal, Button } from "antd"
import QuickActions from "./components/QuickActions"
import { getSessionTypeIcon, getSessionTypeText, parseTimeRange } from "./utils/LearnerDashboardUtils"
import { convertUTCDateTimeToLocal } from "../../../utils/timezoneUtils"

interface UpcomingSession {
  id: string
  mentorName: string
  mentorAvatar: string
  date: string // Local date
  time: string // Local time
  duration: string
  sessionType: "Virtual" | "OneOnOne" | "Onsite"
  topic: string
  sessionStatus: "Approved" | "Rescheduled"
  // Store original UTC values
  originalDate: string
  originalStartTime: string
  originalEndTime: string
}

export default function LearnerDashboard() {
  const [sessions, setSessions] = useState<UpcomingSession[]>([])
  const [loading, setLoading] = useState<boolean>(true)
  const [error, setError] = useState<string | null>(null)
  const [isModalVisible, setIsModalVisible] = useState<boolean>(false)
  const [selectedSession, setSelectedSession] = useState<UpcomingSession | null>(null)
  const [notify, setNotify] = useState<NotificationProps | null>(null)
  const [userTimezone, setUserTimezone] = useState<string>("")
  const { notification } = App.useApp()

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
        showProgress: true,
        duration: 3,
        pauseOnHover: true,
      })
      setNotify(null)
    }
  }, [notify, notification])

  useEffect(() => {
    if (userTimezone) {
      fetchSessions()
    }
  }, [userTimezone])

  const fetchSessions = async () => {
    if (!userTimezone) return

    try {
      setLoading(true)
      const response: GetLearnerDashboardResponse = await learnerDashboardService.getLearnerDashboard()

      const upcomingSessions: UpcomingSession[] =
        response.upcomingSessions?.map((session: LearnerUpcomingSessionResponse) => {
          // Parse the original UTC time range
          const { time: utcTime, duration } = parseTimeRange(session.timeRange)

          // Extract start and end times from the UTC time range
          const [utcStartTime, utcEndTime] = utcTime.split(" - ")

          // Convert UTC date and time to local
          const { localDate, localStartTime, localEndTime } = convertUTCDateTimeToLocal(
            session.scheduledDate,
            utcStartTime,
            utcEndTime,
            userTimezone,
          )

          // Format local time range
          const localTimeRange = `${localStartTime} - ${localEndTime}`

          return {
            id: session.sessionId,
            mentorName: session.mentorName,
            mentorAvatar: session.mentorProfilePictureUrl || DefaultAvatar,
            date: dayjs(localDate).format("MMM D, YYYY"),
            time: localTimeRange,
            duration,
            sessionType: session.type as "Virtual" | "OneOnOne" | "Onsite",
            topic: "Mentoring Session",
            sessionStatus: session.status as "Approved" | "Rescheduled",
            // Store original UTC values
            originalDate: session.scheduledDate,
            originalStartTime: utcStartTime,
            originalEndTime: utcEndTime,
          }
        }) || []

      setSessions(upcomingSessions)
    } catch (err) {
      setError("Failed to fetch sessions. Please try again later.")
      console.error("Error fetching learner dashboard:", err)
    } finally {
      setLoading(false)
    }
  }

  const handleSessionClick = (session: UpcomingSession) => {
    if (session.sessionStatus === "Rescheduled") {
      setSelectedSession(session)
      setIsModalVisible(true)
    }
  }

  const handleAgree = async () => {
    if (!selectedSession) return
    try {
      await learnerDashboardService.acceptSessionBooking(selectedSession.id)
      setSessions((prev) => prev.map((s) => (s.id === selectedSession.id ? { ...s, sessionStatus: "Approved" } : s)))
      setIsModalVisible(false)
      setSelectedSession(null)
      setNotify({
        type: "success",
        message: "Success",
        description: "Rescheduled successfully",
      })
    } catch (err: any) {
      setNotify({
        type: "error",
        message: "Error",
        description: err?.response?.data?.error || "Failed to accept rescheduled session",
      })
    }
  }

  const handleDecline = async () => {
    if (!selectedSession) return

    try {
      await learnerDashboardService.cancelSessionBooking(selectedSession.id)
      setSessions((prev) => prev.filter((s) => s.id !== selectedSession.id))
      setIsModalVisible(false)
      setSelectedSession(null)
      setNotify({
        type: "success",
        message: "Success",
        description: "Canceled reschedule successfully",
      })
    } catch (err: any) {
      setNotify({
        type: "error",
        message: "Error",
        description: err?.response?.data?.error || "Rescheduled error",
      })
    }
  }

  return (
    <div className="bg-gray-800 rounded-lg overflow-hidden shadow-lg p-6">
      {/* Header */}
      <div className="mb-8">
        <div className="flex items-center gap-4 mb-4">
          <div>
            <h1 className="text-2xl font-semibold">Learner Dashboard</h1>
            <p className="text-slate-300 text-sm">Track your learning journey and connect with mentors</p>
            {userTimezone && (
              <p className="text-slate-400 text-xs mt-1">All times shown in your timezone: {userTimezone}</p>
            )}
          </div>
        </div>
      </div>

      {/* Main Content */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-4">
        {/* Upcoming Sessions */}
        <Card
          title={
            <div className="flex items-center justify-between">
              <span className="text-white text-lg font-semibold flex items-center gap-2">
                <CalendarOutlined className="text-blue-400" />
                Upcoming Sessions
              </span>
              <span className="bg-blue-500/20 text-blue-300 px-3 py-1 rounded-full text-sm font-medium">
                {sessions.length} sessions
              </span>
            </div>
          }
          className="lg:col-span-2"
          loading={loading}
        >
          {error ? (
            <div className="text-center py-8">
              <p className="text-red-400">{error}</p>
            </div>
          ) : (
            <List
              dataSource={sessions}
              locale={{ emptyText: "No upcoming sessions" }}
              renderItem={(session, index) => (
                <List.Item
                  className={`border-b border-slate-500/20 last:border-b-0 py-4 cursor-pointer hover:bg-slate-700/30 transition-colors ${index === 0 && sessions.length > 0 ? "bg-blue-500/10 border-blue-400/30 rounded-lg" : ""
                    } ${session.sessionStatus === "Rescheduled" ? "bg-yellow-500/10 border-yellow-400/30" : ""}`}
                  onClick={() => handleSessionClick(session)}
                >
                  <List.Item.Meta
                    avatar={
                      <div className="relative">
                        <Avatar src={session.mentorAvatar} size={48} />
                        <div className="absolute -top-1 -right-1">{getSessionTypeIcon(session.sessionType)}</div>
                        {session.sessionStatus === "Rescheduled" && (
                          <div className="absolute -bottom-1 -right-1 w-4 h-4 bg-yellow-500 rounded-full border-2 border-gray-800"></div>
                        )}
                      </div>
                    }
                    title={
                      <div className="flex items-center justify-between">
                        <span className="text-white font-semibold text-lg">{session.mentorName}</span>
                        {index === 0 && sessions.length > 0 && session.sessionStatus === "Approved" && (
                          <span className="bg-yellow-500/20 text-yellow-300 px-2 py-1 rounded text-xs font-medium">
                            Next Session
                          </span>
                        )}
                        {session.sessionStatus === "Rescheduled" && (
                          <span className="bg-yellow-500/20 text-yellow-300 px-2 py-1 rounded text-xs font-medium">
                            Rescheduled
                          </span>
                        )}
                      </div>
                    }
                    description={
                      <div className="space-y-2">
                        <p className="text-slate-300 font-medium">{session.topic}</p>
                        <div className="flex items-center gap-4 text-sm text-slate-400">
                          <span className="flex items-center gap-1">
                            {getSessionTypeIcon(session.sessionType)}
                            {getSessionTypeText(session.sessionType)}
                          </span>
                          <span>📅 {session.date}</span>
                          <span>🕐 {session.time}</span>
                          <span>⏱️ {session.duration}</span>
                        </div>
                      </div>
                    }
                  />
                </List.Item>
              )}
            />
          )}
        </Card>

        <div className="flex flex-col gap-4">
          {/* Quick Actions */}
          <QuickActions />
        </div>
      </div>

      {/* Reschedule Modal */}
      <Modal
        title={<span className="text-white">Confirm Reschedule</span>}
        open={isModalVisible}
        onCancel={() => {
          setIsModalVisible(false)
          setSelectedSession(null)
        }}
        footer={[
          <Button key="decline" onClick={handleDecline} className="bg-red-500 hover:bg-red-600 text-white border-none">
            Cancel
          </Button>,
          <Button
            key="agree"
            type="primary"
            onClick={handleAgree}
            className="bg-green-500 hover:bg-green-600 border-none"
          >
            Accept
          </Button>,
        ]}
        className="[&_.ant-modal-content]:bg-slate-800 [&_.ant-modal-content]:border [&_.ant-modal-content]:border-slate-700"
      >
        <p className="text-slate-300">Your mentor has rescheduled the session. Do you agree to reschedule?</p>
        {selectedSession && (
          <div className="mt-4 p-3 bg-slate-700 rounded-lg">
            <p className="text-slate-300 text-sm">
              <strong>Session Details:</strong>
            </p>
            <p className="text-slate-400 text-sm">📅 {selectedSession.date}</p>
            <p className="text-slate-400 text-sm">🕐 {selectedSession.time}</p>
            <p className="text-slate-400 text-sm">⏱️ {selectedSession.duration}</p>
          </div>
        )}
      </Modal>
    </div>
  )
}
