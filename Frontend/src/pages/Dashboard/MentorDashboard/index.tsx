"use client"
import { Card, Avatar, List, App } from "antd"
import {
  CalendarOutlined,
} from "@ant-design/icons"
import { useEffect, useState } from "react"
import mentorDashboardService, { type MentorDashboardResponse } from "../../../services/mentor/mentorDashboardService"
import { useAuth } from "../../../hooks"
import DefaultAvatar from "../../../assets/images/default-account.svg"
import QuickActions from "./components/QuickActions"
import MentorDashboardStatistics from "./components/MentorDashboardStatistics"
import { type SessionTypeValue } from "../../../types/enums/SessionType"
import { getSessionTypeIcon, getSessionTypeText, parseTimeRange } from "./utils/MentorDashboardUtils"
import type { NotificationProps } from "../../../types/Notification"
import { convertUTCDateTimeToLocal } from "../../../utils/timezoneUtils"

interface UpcomingSession {
  id: string
  learnerName: string
  learnerAvatar: string
  date: string
  time: string
  duration: string
  sessionType: SessionTypeValue
  topic: string
}

export default function MentorDashboard() {
  const { user } = useAuth()
  const [mentorDashboardData, setMentorDashboardData] = useState<MentorDashboardResponse | undefined>()
  const [loading, setLoading] = useState(true)
  const [notify, setNotify] = useState<NotificationProps | null>(null);
  const { notification } = App.useApp();

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

  useEffect(() => {
    fetchMentorDashboardData()
  }, [])

  const fetchMentorDashboardData = async () => {
    try {
      setLoading(true)
      const response = await mentorDashboardService.getDashboardData(user?.id || "")
      setMentorDashboardData(response)
    } catch (error: any) {
      setNotify({
        type: "error",
        message: "Failed to load activity log data",
        description:
          error?.response?.data?.error || "Error loading activity log data",
      });
    } finally {
      setLoading(false)
    }
  }

  const upcomingSessions: UpcomingSession[] = mentorDashboardData?.upcomingSessionsList?.map((session) => {
    const { time, duration } = parseTimeRange(session.timeRange)
    return {
      id: session.sessionId,
      learnerName: session.learnerName,
      learnerAvatar: session.learnerProfilePhotoUrl || DefaultAvatar,
      date: session.scheduledDate,
      time,
      duration,
      sessionType: session.type as SessionTypeValue,
      topic: "Mentoring Session",
    }
  }) || []

  return (
    <div className="bg-gray-800 rounded-lg overflow-hidden shadow-lg p-6">
      {/* Header */}
      <div className="mb-8">
        <div className="flex items-center gap-4 mb-4">
          <div>
            <h1 className="text-2xl font-semibold">Mentor Dashboard</h1>
            <p className="text-slate-300 text-sm">
              Organize your sessions and statistics overview
            </p>
          </div>
        </div>
      </div>

      {/* Main Stats Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 mb-4">
        <MentorDashboardStatistics metrics={mentorDashboardData} />
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
                {mentorDashboardData?.upcomingSessions ?? 0} sessions
              </span>
            </div>
          }
          className="lg:col-span-2"
          loading={loading}
        >
          <List
            dataSource={upcomingSessions}
            locale={{ emptyText: "No upcoming sessions" }}
            renderItem={(session, index) => (
              <List.Item
                className={`border-b border-slate-500/20 px-4 last:border-b-0 py-4 ${index === 0 && upcomingSessions.length > 0 ? 'bg-blue-500/10 border-blue-400/30 rounded-lg' : ''
                  }`}
                style={{
                  paddingLeft: 12,
                  paddingRight: 12
                }}
              >
                <List.Item.Meta
                  avatar={
                    <div className="relative">
                      <Avatar src={session.learnerAvatar} size={48} />
                      <div className="absolute -top-1 -right-1">{getSessionTypeIcon(session.sessionType)}</div>
                    </div>
                  }
                  title={
                    <div className="flex items-center justify-between">
                      <span className="text-white font-semibold text-lg">{session.learnerName}</span>
                      {index === 0 && upcomingSessions.length > 0 && (
                        <span className="bg-yellow-500/20 text-yellow-300 px-2 py-1 rounded text-xs font-medium">
                          Next Session
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
                        <span>📅 {convertUTCDateTimeToLocal(session.date, session.time, "", user?.timezone || "utc").localDate}</span>
                        <span>🕐 {convertUTCDateTimeToLocal(session.date, session.time, "", user?.timezone || "utc").localStartTime}</span>
                        <span>⏱️ {session.duration}</span>
                      </div>
                    </div>
                  }
                />
              </List.Item>
            )}
          />
        </Card>

        <div className="flex flex-col gap-4">
          {/* Quick Actions */}
          <QuickActions />
        </div>
      </div>
    </div>
  )
}