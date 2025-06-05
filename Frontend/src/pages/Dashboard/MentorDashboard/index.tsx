"use client"
import { Card, Avatar, Tag, List, Statistic } from "antd"
import {
  CalendarOutlined,
  BookOutlined,
  TeamOutlined,
  VideoCameraOutlined,
  AudioOutlined,
  PlayCircleOutlined,
  FileTextOutlined,
  EditOutlined,
} from "@ant-design/icons"
import { formatDate } from "../../../utils/DateFormat"
import { useEffect, useState } from "react"
import mentorDashboardService, { type MentorDashboardResponse } from "../../../services/mentor/mentorDashboardService"
import { useAuth } from "../../../hooks"
import type { UserDetail } from "../../../types/UserTypes"

interface UpcomingSession {
  id: string
  learnerName: string
  learnerAvatar: string
  date: string
  time: string
  duration: string
  sessionType: "video_call" | "audio_call"
  status: "confirmed" | "pending" | "cancelled"
  topic: string
}

interface CourseMaterial {
  id: string
  name: string
  type: "video" | "document" | "assignment" | "quiz"
  size?: string
  duration?: string
}

interface Course {
  id: string
  title: string
  description: string
  category: string
  status: "active" | "completed" | "draft"
  materials: CourseMaterial[]
}

// Hardcoded mentor and course data (API doesn't provide these)
const mentorData = {
  name: "Sarah Johnson",
  email: "sarah.j@mentor.com",
  avatar: "/placeholder.svg?height=80&width=80",
  title: "Senior Data Science Mentor",
  rating: 4.9,
  joinDate: "January 2023",
}

const courses: Course[] = [
  {
    id: "1",
    title: "Introduction to Data Science",
    description: "Complete beginner course covering Python, statistics, and basic ML",
    category: "Data Science",
    status: "active",
    materials: [
      { id: "1", name: "Course Introduction", type: "video", duration: "15 mins" },
      { id: "2", name: "Python Basics Handbook", type: "document", size: "2.5 MB" },
      { id: "3", name: "Week 1 Assignment", type: "assignment" },
      { id: "4", name: "Statistics Quiz", type: "quiz" },
      { id: "5", name: "Data Visualization Tutorial", type: "video", duration: "32 mins" },
    ],
  },
  {
    id: "2",
    title: "Advanced Machine Learning",
    description: "Deep dive into ML algorithms and real-world applications",
    category: "Machine Learning",
    status: "active",
    materials: [
      { id: "1", name: "ML Algorithms Overview", type: "video", duration: "45 mins" },
      { id: "2", name: "Neural Networks Guide", type: "document", size: "5.2 MB" },
      { id: "3", name: "Model Training Exercise", type: "assignment" },
      { id: "4", name: "Deep Learning Resources", type: "document", size: "1.8 MB" },
    ],
  },
]

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
  const [mentorDashboardData, setMentorDashboardData] = useState<MentorDashboardResponse | undefined>()
  const [loading, setLoading] = useState(true)
  const [userDetail, setUserDetail] = useState<UserDetail | undefined>()

  useEffect(() => {
    fetchMentorDashboardData()
  }, [])

  useEffect(() => {
    // get user detail
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

  const upcomingSessions: UpcomingSession[] = mentorDashboardData?.upcomingSessionsList?.map((session) => {
    const { time, duration } = parseTimeRange(session.timeRange)
    return {
      id: session.sessionId,
      learnerName: session.learnerName,
      learnerAvatar: "/placeholder.svg?height=48&width=48",
      date: formatDate(session.scheduledDate),
      time,
      duration,
      sessionType: session.type === "OneOnOne" ? "video_call" : "audio_call", // Map API type to sessionType
      status: "confirmed",
      topic: "Mentoring Session",
    }
  }) || []

  const getSessionTypeIcon = (type: string) => {
    switch (type) {
      case "video_call":
        return <VideoCameraOutlined className="text-blue-400" />
      case "audio_call":
        return <AudioOutlined className="text-green-400" />
      default:
        return <CalendarOutlined className="text-gray-400" />
    }
  }

  const getSessionTypeText = (type: string) => {
    switch (type) {
      case "video_call":
        return "Video Call"
      case "audio_call":
        return "Audio Call"
      default:
        return "Session"
    }
  }

  const getStatusColor = (status: string) => {
    switch (status) {
      case "confirmed":
        return "green"
      case "pending":
        return "orange"
      case "cancelled":
        return "red"
      default:
        return "gray"
    }
  }

  const getMaterialIcon = (type: string) => {
    switch (type) {
      case "video":
        return <PlayCircleOutlined className="text-red-400" />
      case "document":
        return <FileTextOutlined className="text-blue-400" />
      case "assignment":
        return <EditOutlined className="text-green-400" />
      case "quiz":
        return <BookOutlined className="text-purple-400" />
      default:
        return <FileTextOutlined className="text-gray-400" />
    }
  }

  return (
    <div className="bg-gray-800 rounded-lg overflow-hidden shadow-lg p-6">
      {/* Header */}
      <div className="flex items-center justify-between mb-8">
        <div className="flex items-center gap-4">
          <Avatar src={mentorData.avatar} size={80} className="ring-2 ring-blue-400/20" />
          <div>
            <h2 className="text-2xl font-bold">Welcome back, {mentorData.name}!</h2>
            <p className="text-slate-300 text-lg">{mentorData.title}</p>
            <span className="text-slate-300">Member since {mentorData.joinDate}</span>
          </div>
        </div>
      </div>

      {/* Stats Cards */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
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
      <div className="grid grid-cols-1 xl:grid-cols-2 gap-6">
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
          loading={loading}
        >
          <List
            dataSource={upcomingSessions}
            locale={{ emptyText: "No upcoming sessions" }}
            renderItem={(session) => (
              <List.Item className="border-b border-slate-500/20 last:border-b-0 py-4">
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
                      <Tag color={getStatusColor(session.status)} className="capitalize">
                        {session.status}
                      </Tag>
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

        {/* Course Materials */}
        <Card
          title={
            <span className="text-white text-xl font-semibold flex items-center gap-2">
              <BookOutlined className="text-purple-400" />
              My Courses & Materials
            </span>
          }
        >
          <div className="space-y-6">
            {courses.map((course) => (
              <div key={course.id} className="bg-slate-500/30 rounded-lg p-4 hover:bg-slate-500/50 transition-colors">
                <div className="flex items-start justify-between mb-4">
                  <div className="flex-1">
                    <div className="flex items-center gap-2 mb-2">
                      <h3 className="text-white font-semibold text-lg">{course.title}</h3>
                      <Tag
                        color={
                          course.status === "active" ? "green" : course.status === "completed" ? "blue" : "orange"
                        }
                      >
                        {course.status}
                      </Tag>
                    </div>
                    <p className="text-slate-300 text-sm mb-2">{course.description}</p>
                    <Tag className="bg-blue-500/20 text-blue-300 border-blue-400/30">{course.category}</Tag>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </Card>
      </div>
    </div>
  )
}