"use client"

import { useState, useEffect } from "react"
import {
  Table,
  Button,
  Tag,
  Modal,
  DatePicker,
  TimePicker,
  Input,
  Space,
  Card,
  Tabs,
  Avatar,
  Badge,
  Row,
  Col,
  Statistic,
} from "antd"
import {
  CalendarOutlined,
  ClockCircleOutlined,
  UserOutlined,
  CheckCircleOutlined,
  CloseCircleOutlined,
  ExclamationCircleOutlined,
} from "@ant-design/icons"
import type { ColumnsType } from "antd/es/table"
import dayjs from "dayjs"
import { sessionBookingService } from "../../../services/sessiontracking/sessiontracking"
import type { SessionBookingRequest } from "../../../types/SessionBookingTypes"
import { getStatusString } from "../../../types/SessionBookingTypes"

const { TextArea } = Input
const { TabPane } = Tabs

interface Session {
  id: string
  studentName: string
  studentAvatar?: string
  date: string
  startTime: string
  endTime: string
  status: "Pending" | "Approved" | "Completed" | "Cancelled" | "Rescheduled"
  communicationMethod: "VideoCall" | "AudioCall" | "Chat"
  notes?: string
  studentEmail?: string
  timeSlotId?: string
  learnerId?: string
  type?: number
}

const CustomNotification = ({
  visible,
  type,
  message,
  onClose,
}: {
  visible: boolean
  type: "success" | "error" | "info"
  message: string
  onClose: () => void
}) => {
  if (!visible) return null

  const getIcon = () => {
    switch (type) {
      case "success":
        return <CheckCircleOutlined className="text-green-500 text-lg" />
      case "error":
        return <CloseCircleOutlined className="text-red-500 text-lg" />
      default:
        return <ExclamationCircleOutlined className="text-blue-500 text-lg" />
    }
  }

  const getBgColor = () => {
    switch (type) {
      case "success":
        return "bg-white border-l-4 border-green-500 shadow-lg"
      case "error":
        return "bg-white border-l-4 border-red-500 shadow-lg"
      default:
        return "bg-white border-l-4 border-blue-500 shadow-lg"
    }
  }

  const getTextColor = () => {
    switch (type) {
      case "success":
        return "text-green-800"
      case "error":
        return "text-red-800"
      default:
        return "text-blue-800"
    }
  }

  return (
    <div className="fixed top-4 right-4 z-50 max-w-sm w-full">
      <div
        className={`
        ${getBgColor()}
        p-4 rounded-lg transform transition-all duration-300 ease-in-out
        animate-in slide-in-from-top-2 fade-in
      `}
      >
        <div className="flex items-start space-x-3">
          <div className="flex-shrink-0 mt-0.5">{getIcon()}</div>
          <div className="flex-1 min-w-0">
            <p className={`text-sm font-medium ${getTextColor()} leading-5`}>{message}</p>
          </div>
          <div className="flex-shrink-0">
            <Button
              type="text"
              size="small"
              onClick={onClose}
              className="text-gray-400 hover:text-gray-600 p-0 h-auto min-w-0"
            >
              <CloseCircleOutlined className="text-xs" />
            </Button>
          </div>
        </div>
      </div>
    </div>
  )
}

export default function ScheduleSession() {
  const [sessions, setSessions] = useState<Session[]>([])
  const [loading, setLoading] = useState(true)
  const [selectedSession, setSelectedSession] = useState<Session | null>(null)
  const [isModalVisible, setIsModalVisible] = useState(false)
  const [activeTab, setActiveTab] = useState("upcoming")

  const [notification, setNotification] = useState({
    visible: false,
    type: "success" as "success" | "error" | "info",
    message: "",
  })

  const statusColors = {
    Pending: "orange",
    Approved: "blue",
    Completed: "green",
    Cancelled: "red",
    Rescheduled: "purple",
  }

  const statusIcons = {
    Pending: <ExclamationCircleOutlined />,
    Approved: <CheckCircleOutlined />,
    Completed: <CheckCircleOutlined />,
    Cancelled: <CloseCircleOutlined />,
    Rescheduled: <ClockCircleOutlined />,
  }

  const communicationIcons = {
    VideoCall: "📹",
    AudioCall: "🎧",
    Chat: "💬",
  }

  const showNotification = (type: "success" | "error" | "info", message: string) => {
    setNotification({
      visible: true,
      type,
      message,
    })

    setTimeout(() => {
      setNotification((prev) => ({ ...prev, visible: false }))
    }, 4000)
  }

  const hideNotification = () => {
    setNotification((prev) => ({ ...prev, visible: false }))
  }

  const convertApiResponseToSession = (apiData: SessionBookingRequest): Session => {
    return {
      id: apiData.id,
      studentName: apiData.fullNameLearner,
      date: apiData.date,
      startTime: apiData.startTime,
      endTime: apiData.endTime,
      status: typeof apiData.status === "number" ? getStatusString(apiData.status) : apiData.status,
      communicationMethod: apiData.preferredCommunicationMethod,
      timeSlotId: apiData.timeSlotId,
      learnerId: apiData.learnerId,
      type: apiData.type,
    }
  }

  useEffect(() => {
    loadSessions()
  }, [])

  const loadSessions = async () => {
    try {
      setLoading(true)
      const response = await sessionBookingService.getSessionBookings()

      if (!response || !Array.isArray(response)) {
        throw new Error("Invalid response format")
      }

      const convertedSessions = response.map(convertApiResponseToSession)
      setSessions(convertedSessions)

      console.log("Sessions loaded:", convertedSessions.length)
    } catch (error: any) {
      showNotification("error", `Failed to load sessions: ${error.message || "Unknown error"}`)
      console.error("Error loading sessions:", error)
    } finally {
      setLoading(false)
    }
  }

  const upcomingSessions = sessions.filter(
    (session) =>
      (["Pending", "Approved"].includes(session.status) && dayjs(session.date).isAfter(dayjs(), "day")) ||
      dayjs(session.date).isSame(dayjs(), "day"),
  )

  const pastSessions = sessions.filter(
    (session) => ["Completed", "Cancelled"].includes(session.status) || dayjs(session.date).isBefore(dayjs(), "day"),
  )

  const handleStatusChange = async (sessionId: string, newStatus: Session["status"]) => {
    try {
      const session = sessions.find((s) => s.id === sessionId)
      if (!session) {
        showNotification("error", "Session not found")
        return
      }

      await sessionBookingService.updateSessionStatus(sessionId, newStatus)

      setSessions((prev) =>
        prev.map((session) => (session.id === sessionId ? { ...session, status: newStatus } : session)),
      )

      switch (newStatus) {
        case "Approved":
          showNotification("success", "Session approved successfully! 🎉")
          break
        case "Completed":
          showNotification("success", "Session completed! It will be moved to Past Sessions 📚")
          break
        case "Cancelled":
          showNotification("success", "Session cancelled successfully ❌")
          break
        default:
          showNotification("success", `Status updated to ${newStatus} successfully`)
          break
      }
    } catch (error: any) {
      showNotification("error", `Failed to update session status: ${error.message || "Unknown error"}`)
      console.error("Error updating session status:", error)
    }
  }

  const handleReschedule = async (
    sessionId: string,
    newDate: string,
    newStartTime: string,
    newEndTime: string,
    reason: string,
  ) => {
    try {
      await sessionBookingService.rescheduleSession(sessionId, {
        date: newDate,
        startTime: newStartTime,
        endTime: newEndTime,
        reason: reason,
      })

      setSessions((prev) =>
        prev.map((session) =>
          session.id === sessionId
            ? {
                ...session,
                date: newDate,
                startTime: newStartTime,
                endTime: newEndTime,
                status: "Rescheduled" as const,
              }
            : session,
        ),
      )

      const session = sessions.find((s) => s.id === sessionId)
      const message = session?.studentEmail
        ? `Session rescheduled successfully! 📅 Email notification sent to ${session.studentEmail}`
        : "Session rescheduled successfully! 📅"

      showNotification("success", message)
      setIsModalVisible(false)
    } catch (error) {
      showNotification("error", "Failed to reschedule session")
      console.error("Error rescheduling session:", error)
    }
  }

  const openRescheduleModal = async (session: Session) => {
    try {
      const sessionDetails = await sessionBookingService.getSessionDetails(session.id)
      setSelectedSession({
        ...session,
        date: sessionDetails.date,
        startTime: sessionDetails.startTime,
        endTime: sessionDetails.endTime,
      })
      setIsModalVisible(true)
    } catch (error) {
      showNotification("error", "Failed to load session details")
      console.error("Error loading session details:", error)
      setSelectedSession(session)
      setIsModalVisible(true)
    }
  }

  const getActionsColumn = (): ColumnsType<Session>[0] => ({
    title: "Actions",
    key: "actions",
    render: (_, record: Session) => (
      <Space>
        {record.status === "Pending" && (
          <>
            <Button type="primary" size="small" onClick={() => handleStatusChange(record.id, "Approved")}>
              Accept
            </Button>
            <Button type="default" size="small" onClick={() => openRescheduleModal(record)}>
              Reschedule
            </Button>
          </>
        )}

        {record.status === "Approved" && (
          <>
            <Button type="primary" size="small" onClick={() => handleStatusChange(record.id, "Completed")}>
              Mark Complete
            </Button>
            <Button type="default" size="small" onClick={() => openRescheduleModal(record)}>
              Reschedule
            </Button>
          </>
        )}

        {["Pending", "Approved"].includes(record.status) && (
          <Button danger size="small" onClick={() => handleStatusChange(record.id, "Cancelled")}>
            Cancel
          </Button>
        )}
      </Space>
    ),
  })

  const getBaseColumns = (): ColumnsType<Session> => [
    {
      title: "Learner",
      dataIndex: "studentName",
      key: "studentName",
      render: (name: string, record: Session) => (
        <Space>
          <Avatar src={record.studentAvatar} icon={<UserOutlined />} size="small" />
          <span>{name}</span>
        </Space>
      ),
    },
    {
      title: "Date & Time",
      key: "datetime",
      render: (_, record: Session) => (
        <Space direction="vertical" size="small">
          <span>
            <CalendarOutlined /> {dayjs(record.date).format("MMM DD, YYYY")}
          </span>
          <span>
            <ClockCircleOutlined /> {record.startTime} - {record.endTime}
          </span>
        </Space>
      ),
    },
    {
      title: "Method",
      dataIndex: "communicationMethod",
      key: "communicationMethod",
      render: (method: string) => (
        <span>
          {communicationIcons[method as keyof typeof communicationIcons]} {method}
        </span>
      ),
    },
    {
      title: "Status",
      dataIndex: "status",
      key: "status",
      render: (status: Session["status"]) => (
        <Tag color={statusColors[status]} icon={statusIcons[status]}>
          {status.toUpperCase()}
        </Tag>
      ),
    },
  ]

  const upcomingColumns = [...getBaseColumns(), getActionsColumn()]
  const pastColumns = [...getBaseColumns()] 

  const RescheduleModal = () => {
    const [newDate, setNewDate] = useState<dayjs.Dayjs | null>(null)
    const [newStartTime, setNewStartTime] = useState<dayjs.Dayjs | null>(null)
    const [newEndTime, setNewEndTime] = useState<dayjs.Dayjs | null>(null)
    const [rescheduleReason, setRescheduleReason] = useState("")
    const [validationError, setValidationError] = useState("")

    useEffect(() => {
      if (selectedSession && isModalVisible) {
        setNewDate(dayjs(selectedSession.date))
        setNewStartTime(dayjs(selectedSession.startTime, "HH:mm:ss"))
        setNewEndTime(dayjs(selectedSession.endTime, "HH:mm:ss"))
        setRescheduleReason("")
        setValidationError("")
      }
    }, [selectedSession, isModalVisible])

    const handleSubmit = () => {
      setValidationError("")

      if (newDate && newStartTime && newEndTime && selectedSession) {
        // Validation: End time must be after start time
        if (newEndTime.isBefore(newStartTime) || newEndTime.isSame(newStartTime)) {
          setValidationError("Start time must be before end time")
          return
        }

        const currentDateTime = dayjs()
        const selectedDateTime = newDate.hour(newStartTime.hour()).minute(newStartTime.minute())

        if (selectedDateTime.isBefore(currentDateTime)) {
          setValidationError("Start time must be later than current time")
          return
        }

        if (rescheduleReason.length > 100) {
          setValidationError("Reason should not exceed 100 characters")
          return
        }

        handleReschedule(
          selectedSession.id,
          newDate.format("YYYY-MM-DD"),
          newStartTime.format("HH:mm:ss"),
          newEndTime.format("HH:mm:ss"),
          rescheduleReason,
        )

        setNewDate(null)
        setNewStartTime(null)
        setNewEndTime(null)
        setRescheduleReason("")
        setValidationError("")
      } else {
        setValidationError("Please fill in all required fields")
      }
    }

    const handleModalClose = () => {
      setIsModalVisible(false)
      setValidationError("")
      setNewDate(null)
      setNewStartTime(null)
      setNewEndTime(null)
      setRescheduleReason("")
    }

    return (
      <Modal
        title="Reschedule Session"
        open={isModalVisible}
        onCancel={handleModalClose}
        onOk={handleSubmit}
        okText="Reschedule"
      >
        <Space direction="vertical" style={{ width: "100%" }}>
          {validationError && (
            <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-2 rounded">{validationError}</div>
          )}

          <div>
            <label>New Date:</label>
            <DatePicker
              style={{ width: "100%", marginTop: 8 }}
              value={newDate}
              onChange={setNewDate}
              disabledDate={(current) => current && current < dayjs().startOf("day")}
            />
          </div>

          <div>
            <label>Time:</label>
            <Row gutter={8} style={{ marginTop: 8 }}>
              <Col span={12}>
                <TimePicker
                  style={{ width: "100%" }}
                  value={newStartTime}
                  onChange={setNewStartTime}
                  format="HH:mm"
                  placeholder="Start Time"
                />
              </Col>
              <Col span={12}>
                <TimePicker
                  style={{ width: "100%" }}
                  value={newEndTime}
                  onChange={setNewEndTime}
                  format="HH:mm"
                  placeholder="End Time"
                />
              </Col>
            </Row>
          </div>

          <div>
            <label>Reason for Reschedule:</label>
            <TextArea
              rows={3}
              style={{ marginTop: 8 }}
              value={rescheduleReason}
              onChange={(e) => setRescheduleReason(e.target.value)}
              placeholder="Please provide a reason for rescheduling..."
              maxLength={100}
              showCount
            />
          </div>
        </Space>
      </Modal>
    )
  }

  const pendingSessions = sessions.filter((s) => s.status === "Pending").length
  const completedSessions = sessions.filter((s) => s.status === "Completed").length
  const totalActiveSessions = pendingSessions + completedSessions

  return (
    <div style={{ padding: "24px", minHeight: "100vh" }}>
      <CustomNotification
        visible={notification.visible}
        type={notification.type}
        message={notification.message}
        onClose={hideNotification}
      />

      <Card>
        <div style={{ marginBottom: "24px" }}>
          <h1 style={{ fontSize: "24px", fontWeight: "bold", marginBottom: "8px" }}>Mentor Dashboard</h1>
          <p style={{ color: "#666", marginBottom: "16px" }}>
            Manage your mentoring sessions and track your activities
          </p>

          {/* Statistics Cards */}
          <Row gutter={16} style={{ marginBottom: "24px" }}>
            <Col span={6}>
              <Card>
                <Statistic
                  title="Pending Sessions"
                  value={pendingSessions}
                  valueStyle={{ color: "#fa8c16" }}
                  prefix={<ExclamationCircleOutlined />}
                />
              </Card>
            </Col>
            <Col span={6}>
              <Card>
                <Statistic
                  title="Approved Sessions"
                  value={sessions.filter((s) => s.status === "Approved").length}
                  valueStyle={{ color: "#1890ff" }}
                  prefix={<CheckCircleOutlined />}
                />
              </Card>
            </Col>
            <Col span={6}>
              <Card>
                <Statistic
                  title="Completed Sessions"
                  value={completedSessions}
                  valueStyle={{ color: "#52c41a" }}
                  prefix={<CheckCircleOutlined />}
                />
              </Card>
            </Col>
            <Col span={6}>
              <Card>
                <Statistic title="Total Sessions" value={totalActiveSessions} prefix={<CalendarOutlined />} />
              </Card>
            </Col>
          </Row>

          <Button type="primary" onClick={loadSessions} loading={loading} style={{ marginBottom: "16px" }}>
            Refresh Sessions
          </Button>
        </div>

        <Tabs activeKey={activeTab} onChange={setActiveTab}>
          <TabPane
            tab={
              <Badge count={upcomingSessions.length} offset={[10, 0]}>
                <span>Upcoming Sessions</span>
              </Badge>
            }
            key="upcoming"
          >
            <Table
              columns={upcomingColumns}
              dataSource={upcomingSessions}
              rowKey="id"
              pagination={{ pageSize: 10 }}
              scroll={{ x: 800 }}
              loading={loading}
            />
          </TabPane>

          <TabPane tab="Past Sessions" key="past">
            <Table
              columns={pastColumns}
              dataSource={pastSessions}
              rowKey="id"
              pagination={{ pageSize: 10 }}
              scroll={{ x: 800 }}
              loading={loading}
            />
          </TabPane>
        </Tabs>
      </Card>

      <RescheduleModal />
    </div>
  )
}
