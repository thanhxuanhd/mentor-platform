"use client"
import { Card, Avatar, List, Statistic, Button } from "antd"
import {
  CalendarOutlined,
  BookOutlined,
  TeamOutlined,
  VideoCameraOutlined,
  AudioOutlined,
  HomeOutlined,
  SettingOutlined,
  ProfileOutlined,
} from "@ant-design/icons"
import { formatDate } from "../../../utils/DateFormat"
import { useEffect, useState } from "react"
import { useNavigate } from "react-router-dom"
import mentorDashboardService, { type MentorDashboardResponse } from "../../../services/mentor/mentorDashboardService"
import { useAuth } from "../../../hooks"
import DefaultAvatar from "../../../assets/images/default-account.svg"
import { userService } from "../../../services/user/userService"
import type { UserDetail } from "../../../types/UserTypes"

// Session Type constants to match backend
const SessionType = {
  Virtual: "Virtual" as const,
  OneOnOne: "OneOnOne" as const,
  Onsite: "Onsite" as const
} as const

type SessionTypeValue = typeof SessionType[keyof typeof SessionType]

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

const parseTimeRange = (timeRange: string) => {
  const [start, end] = timeRange.split(" - ").map((time) => time.trim())
  const startDate = new Date(`1970-01-01T${start}:00`)
  const endDate = new Date(`1970-01-01T${end}:00`)
  const durationMins = (endDate.getTime() - startDate.getTime()) / (1000 * 60)
  return {
    time: start,
    duration: `${durationMins} mins`,
  }
}

export default function MentorDashboard() {
  const { user } = useAuth()
  const navigate = useNavigate()
  const [mentorDashboardData, setMentorDashboardData] = useState<MentorDashboardResponse | undefined>()
  const [userDetails, setUserDetails] = useState<UserDetail | undefined>()
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    fetchMentorDashboardData()
    fetchUserDetails()
  }, [])

  const fetchMentorDashboardData = async () => {
    try {
      setLoading(true)
      const response = await mentorDashboardService.getDashboardData(user?.id || "")
      setMentorDashboardData(response)
    } catch (error) {
      console.error("Error fetching dashboard data:", error)
    } finally {
      setLoading(false)
    }
  }

  const fetchUserDetails = async () => {
    try {
      if (user?.id) {
        const response = await userService.getUserDetail(user.id)
        setUserDetails(response)
      }
    } catch (error) {
      console.error("Error fetching user details:", error)
    }
  }
    const upcomingSessions: UpcomingSession[] = mentorDashboardData?.upcomingSessionsList?.map((session) => {
    const { time, duration } = parseTimeRange(session.timeRange)
    return {
      id: session.sessionId,
      learnerName: session.learnerName,
      learnerAvatar: session.learnerProfilePhotoUrl || DefaultAvatar,
      date: formatDate(session.scheduledDate),
      time,
      duration,
      sessionType: session.type as SessionTypeValue,
      topic: "Mentoring Session",
    }
  }) || []
  const getSessionTypeIcon = (type: SessionTypeValue) => {
    switch (type) {
      case SessionType.Virtual:
        return <VideoCameraOutlined className="text-blue-400" />
      case SessionType.OneOnOne:
        return <AudioOutlined className="text-green-400" />
      case SessionType.Onsite:
        return <HomeOutlined className="text-purple-400" />
      default:
        return <CalendarOutlined className="text-gray-400" />
    }
  }

  const getSessionTypeText = (type: SessionTypeValue) => {
    switch (type) {
      case SessionType.Virtual:
        return "Virtual Session"
      case SessionType.OneOnOne:
        return "One-on-One Session"
      case SessionType.Onsite:
        return "On-site Session"
      default:
        return "Session"
    }
  }
  const quickActions = [
    {
      title: "Manage Availability",
      description: "Set your available time slots",
      icon: <CalendarOutlined className="text-blue-400" />,
      onClick: () => navigate("/availability"),
    },
    {
      title: "View Profile",
      description: "Edit your mentor profile",
      icon: <ProfileOutlined className="text-green-400" />,
      onClick: () => navigate("/profile"),
    },
    {
      title: "My Courses",
      description: "Manage your courses",
      icon: <BookOutlined className="text-purple-400" />,
      onClick: () => navigate("/courses"),
    },
  ]

  return (
    <div className="bg-gray-800 rounded-lg overflow-hidden shadow-lg p-6">      {/* Header */}
      <div className="flex items-center justify-between mb-8">
        <div className="flex items-center gap-4">
          <Avatar src={userDetails?.profilePhotoUrl || DefaultAvatar} size={80} className="ring-2 ring-blue-400/20" />
          <div>
            <h2 className="text-2xl font-bold">Welcome back, {userDetails?.fullName || user?.fullName || 'Mentor'}!</h2>
            <span className="text-slate-300 block">Member Since: {userDetails?.joinedDate || 'N/A'}</span>
          </div>
        </div>
      </div>{/* Stats Cards */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-6">
        <Card className="bg-slate-600/50 border-slate-500/30 backdrop-blur-sm">
          <Statistic
            title={<span className="text-slate-300">Total Learners</span>}
            value={mentorDashboardData?.totalLearners ?? 0}
            prefix={<TeamOutlined className="text-blue-400" />}
            valueStyle={{ color: "white", fontSize: "2rem" }}
            loading={loading}
          />
        </Card>

        <Card className="bg-slate-600/50 border-slate-500/30 backdrop-blur-sm">
          <Statistic
            title={<span className="text-slate-300">Pending Sessions</span>}
            value={mentorDashboardData?.totalPendingSessions ?? 0}
            prefix={<CalendarOutlined className="text-orange-400" />}
            valueStyle={{ color: "white", fontSize: "2rem" }}
            loading={loading}
          />
        </Card>

        <Card className="bg-slate-600/50 border-slate-500/30 backdrop-blur-sm">
          <Statistic
            title={<span className="text-slate-300">Sessions Completed</span>}
            value={mentorDashboardData?.completedSessions ?? 0}
            prefix={<VideoCameraOutlined className="text-green-400" />}
            valueStyle={{ color: "white", fontSize: "2rem" }}
            loading={loading}
          />
        </Card>

        <Card className="bg-slate-600/50 border-slate-500/30 backdrop-blur-sm">
          <Statistic
            title={<span className="text-slate-300">Total Courses</span>}
            value={mentorDashboardData?.totalCourses ?? 0}
            prefix={<BookOutlined className="text-purple-400" />}
            valueStyle={{ color: "white", fontSize: "2rem" }}
            loading={loading}
          />
        </Card>
      </div>

      {/* Main Content */}
      <div className="grid grid-cols-1 xl:grid-cols-2 gap-6">        {/* Upcoming Sessions */}
        <Card
          title={
            <div className="flex items-center justify-between">
              <span className="text-white text-lg font-semibold flex items-center gap-2">
                <CalendarOutlined className="text-blue-400" />
                Upcoming Sessions
              </span>              <span className="bg-blue-500/20 text-blue-300 px-3 py-1 rounded-full text-sm font-medium">
                {mentorDashboardData?.upcomingSessions ?? 0} sessions
              </span>
            </div>
          }
          loading={loading}
        >
          <List
            dataSource={upcomingSessions}
            locale={{ emptyText: "No upcoming sessions" }}
            renderItem={(session, index) => (
              <List.Item 
                className={`border-b border-slate-500/20 last:border-b-0 py-4 ${
                  index === 0 && upcomingSessions.length > 0 ? 'bg-blue-500/10 border-blue-400/30 rounded-lg' : ''
                }`}
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
        </Card>

        {/* Quick Actions */}
        <Card
          title={
            <span className="text-white text-xl font-semibold flex items-center gap-2">
              <SettingOutlined className="text-green-400" />
              Quick Actions
            </span>
          }
        >
          <div className="space-y-4">
            {quickActions.map((action, index) => (
              <Button
                key={index}
                onClick={action.onClick}
                className="w-full bg-slate-500/30 border-slate-400/30 hover:bg-slate-500/50 text-left h-auto p-4"
                style={{ height: 'auto' }}
              >
                <div className="flex items-center gap-3">
                  <div className="text-2xl">{action.icon}</div>
                  <div className="text-left">
                    <div className="text-white font-semibold text-base">{action.title}</div>
                    <div className="text-slate-300 text-sm">{action.description}</div>
                  </div>
                </div>
              </Button>
            ))}
          </div>
        </Card>
      </div>
    </div>
  )
}