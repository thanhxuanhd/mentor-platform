"use client"

import { useState, useEffect, useCallback, useRef } from "react"
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
  status: "Pending" | "Approved" | "Completed" | "Canceled" | "Rescheduled"
  communicationMethod: "VideoCall" | "AudioCall" | "Chat"
  notes?: string
  studentEmail?: string
  timeSlotId?: string
  learnerId?: string
  type?: number
  lastStatusUpdate?: string
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
        return <CheckCircleOutlined className="text-green-500 text-base" />
      case "error":
        return <CloseCircleOutlined className="text-red-500 text-base" />
      default:
        return <ExclamationCircleOutlined className="text-blue-500 text-base" />
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
  const [currentTime, setCurrentTime] = useState(dayjs())

  const [notification, setNotification] = useState({
    visible: false,
    type: "success" as "success" | "error" | "info",
    message: "",
  })

  const [processingSessionIds, setProcessingSessionIds] = useState<Record<string, boolean>>({})
  const [processingTimeSlots, setProcessingTimeSlots] = useState<Record<string, boolean>>({})

  // Add ref to track if we're already processing overtime sessions
  const processingOvertimeRef = useRef(false)

  const statusColors = {
    Pending: "orange",
    Approved: "blue",
    Completed: "green",
    Canceled: "red",
    Rescheduled: "purple",
  }

  const statusIcons = {
    Pending: <ExclamationCircleOutlined />,
    Approved: <CheckCircleOutlined />,
    Completed: <CheckCircleOutlined />,
    Canceled: <CloseCircleOutlined />,
    Rescheduled: <ClockCircleOutlined />,
  }

  const communicationIcons = {
    VideoCall: "Video Call",
    AudioCall: "Audio Call",
    Chat: "Chat",
  }

  // Update current time every minute
  useEffect(() => {
    const interval = setInterval(() => {
      setCurrentTime(dayjs())
    }, 60000) // Update every minute

    return () => clearInterval(interval)
  }, [])

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
      lastStatusUpdate: apiData.lastStatusUpdate || dayjs().toISOString(),
    }
  }

  const handleOvertimeSessions = useCallback(async () => {
    if (processingOvertimeRef.current) return

    processingOvertimeRef.current = true

    try {
      const now = currentTime
      const overtimeSessions = sessions.filter((session) => {
        const sessionDateTime = dayjs(`${session.date} ${session.startTime}`)
        return sessionDateTime.isBefore(now) && ["Pending", "Approved"].includes(session.status)
      })

      if (overtimeSessions.length > 0) {
        const updatePromises = overtimeSessions.map(async (session) => {
          try {
            await sessionBookingService.updateSessionStatus(session.id, "Canceled")
            return { ...session, status: "Canceled" as const, lastStatusUpdate: now.toISOString() }
          } catch (error) {
            console.error(`Failed to cancel overtime session ${session.id}:`, error)
            return session
          }
        })

        const updatedSessions = await Promise.all(updatePromises)

        setSessions((prevSessions) =>
          prevSessions.map((session) => {
            const updated = updatedSessions.find((u) => u.id === session.id)
            return updated || session
          }),
        )

        if (updatedSessions.some((s) => s.status === "Canceled")) {
          showNotification(
            "info",
            `${updatedSessions.filter((s) => s.status === "Canceled").length} overtime sessions have been cancelled and moved to Past Sessions`,
          )
        }
      }
    } catch (error) {
      console.error("Error handling overtime sessions:", error)
    } finally {
      processingOvertimeRef.current = false
    }
  }, [sessions, currentTime])

  // Check for overtime sessions when current time or sessions change
  useEffect(() => {
    if (sessions.length > 0) {
      handleOvertimeSessions()
    }
  }, [currentTime, handleOvertimeSessions])

  useEffect(() => {
    loadSessions()
  }, [])

  const loadSessions = async () => {
    try {
      setLoading(true)
      const response = await sessionBookingService.getSessionBookings()
      console.log("API sessions:", response)

      if (!response || !Array.isArray(response)) {
        throw new Error("Invalid response format")
      }

      const convertedSessions = response.map(convertApiResponseToSession)
      setSessions(convertedSessions)

      console.log("Sessions loaded:", convertedSessions.length)
    } catch (error: unknown) {
      showNotification("error", `Failed to load sessions: ${error}`)
      console.error("Error loading sessions:", error)
    } finally {
      setLoading(false)
    }
  }

  // Improved session filtering logic
  const upcomingSessions = sessions.filter((session) => {
    const sessionDateTime = dayjs(`${session.date} ${session.startTime}`)
    const isOvertime = sessionDateTime.isBefore(currentTime)

    // Only show sessions that are not overtime and have active statuses
    return !isOvertime && ["Pending", "Approved", "Rescheduled"].includes(session.status)
  })

  const pastSessions = sessions.filter((session) => {
    const sessionDateTime = dayjs(`${session.date} ${session.startTime}`)
    const isOvertime = sessionDateTime.isBefore(currentTime)

    // Show completed/cancelled sessions OR overtime sessions that were active
    return (
      ["Completed", "Canceled"].includes(session.status) ||
      (isOvertime && session.lastStatusUpdate && dayjs(session.lastStatusUpdate).isAfter(sessionDateTime))
    )
  })

  const handleStatusChange = async (sessionId: string, newStatus: Session["status"]) => {
    try {
      const session = sessions.find((s) => s.id === sessionId)
      if (!session) {
        showNotification("error", "Session not found")
        return
      }

      const timeSlotKey = `${session.date}_${session.startTime}_${session.endTime}`

      setProcessingSessionIds((prev) => ({ ...prev, [sessionId]: true }))

      if (newStatus === "Approved") {
        setProcessingTimeSlots((prev) => ({ ...prev, [timeSlotKey]: true }))
      }

      await sessionBookingService.updateSessionStatus(sessionId, newStatus)

      setSessions((prevSessions) =>
        prevSessions.map((s) =>
          s.id === sessionId ? { ...s, status: newStatus, lastStatusUpdate: dayjs().toISOString() } : s,
        ),
      )

      await loadSessions()

      switch (newStatus) {
        case "Approved":
          showNotification("success", "Session approved successfully")
          break
        case "Completed":
          showNotification("success", "Session completed and moved to Past Sessions")
          break
        case "Canceled":
          showNotification("success", "Session cancelled and moved to Past Sessions")
          break
        default:
          showNotification("success", `Status updated to ${newStatus} successfully`)
          break
      }
    } catch (error: unknown) {
      let errorMessage = "Unknown error"

      if (error instanceof Error) {
        errorMessage = error.message
      }

      showNotification("error", `Failed to update session status: ${errorMessage}`)
      console.error("Error updating session status:", error)
    } finally {
      const session = sessions.find((s) => s.id === sessionId)
      if (session) {
        const timeSlotKey = `${session.date}_${session.startTime}_${session.endTime}`
        setProcessingSessionIds((prev) => ({ ...prev, [sessionId]: false }))
        setProcessingTimeSlots((prev) => ({ ...prev, [timeSlotKey]: false }))
      }
    }
  }

  const handleReschedule = async (
    sessionId: string,
    newDate: string,
    newStartTime: string,
    newEndTime: string,
    reason: string,
  ) => {
    const session = sessions.find((s) => s.id === sessionId)
    if (!session) {
      showNotification("error", "Session not found")
      return
    }

    const isSameDate = newDate === session.date
    const isSameStart = newStartTime === session.startTime
    const isSameEnd = newEndTime === session.endTime
    const isEmptyReason = reason.trim() === ""

    if (isSameDate && isSameStart && isSameEnd && isEmptyReason) {
      showNotification("info", "No changes detected. Session remains unchanged")
      setIsModalVisible(false)
      return
    }

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
                lastStatusUpdate: dayjs().toISOString(),
              }
            : session,
        ),
      )

      const message = session.studentEmail
        ? `Session rescheduled successfully. Email notification sent to ${session.studentEmail}`
        : "Session rescheduled successfully"

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
    render: (_, record: Session) => {
      const isProcessing = processingSessionIds[record.id] === true
      const timeSlotKey = `${record.date}_${record.startTime}_${record.endTime}`
      const isTimeSlotProcessing = processingTimeSlots[timeSlotKey] === true

      return (
        <Space>
          {record.status === "Pending" && (
            <>
              <Button
                type="primary"
                size="small"
                onClick={() => handleStatusChange(record.id, "Approved")}
                loading={isProcessing}
                disabled={isProcessing || (isTimeSlotProcessing && !processingSessionIds[record.id])}
              >
                Accept
              </Button>
              <Button
                type="default"
                size="small"
                onClick={() => openRescheduleModal(record)}
                disabled={isProcessing || isTimeSlotProcessing}
              >
                Reschedule
              </Button>
            </>
          )}

          {record.status === "Approved" && (
            <>
              <Button
                type="primary"
                size="small"
                onClick={() => handleStatusChange(record.id, "Completed")}
                loading={isProcessing}
                disabled={isProcessing}
              >
                Mark Complete
              </Button>
              <Button type="default" size="small" onClick={() => openRescheduleModal(record)} disabled={isProcessing}>
                Reschedule
              </Button>
            </>
          )}

          {["Pending", "Approved"].includes(record.status) && (
            <Button
              danger
              size="small"
              onClick={() => handleStatusChange(record.id, "Canceled")}
              loading={isProcessing}
              disabled={isProcessing || (isTimeSlotProcessing && !processingSessionIds[record.id])}
            >
              Cancel
            </Button>
          )}
        </Space>
      )
    },
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
      render: (method: string) => <span>{communicationIcons[method as keyof typeof communicationIcons]}</span>,
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
    const [reason, setReason] = useState("")
    const [submitting, setSubmitting] = useState(false)
    const submitRef = useRef(false)

    useEffect(() => {
      if (selectedSession && isModalVisible) {
        setNewDate(dayjs(selectedSession.date))
        setNewStartTime(dayjs(selectedSession.startTime, "HH:mm:ss"))
        setNewEndTime(dayjs(selectedSession.endTime, "HH:mm:ss"))
        setReason("")
        setSubmitting(false)
        submitRef.current = false
      }
    }, [selectedSession, isModalVisible])

    const roundToNearestHalfHour = (time: dayjs.Dayjs | null) => {
      if (!time) return null
      const minute = time.minute()
      const roundedMinute = minute < 15 ? 0 : minute < 45 ? 30 : 0
      const adjustedHour = minute >= 45 ? time.hour() + 1 : time.hour()
      return time.set("hour", adjustedHour).set("minute", roundedMinute).set("second", 0)
    }

    const handleSubmit = async () => {
      // Prevent multiple submissions
      if (submitting || submitRef.current) {
        return
      }

      if (!newDate || !newStartTime || !newEndTime || !selectedSession) {
        showNotification("error", "Please fill in all the fields")
        return
      }

      if (newEndTime.isSameOrBefore(newStartTime)) {
        showNotification("error", "End time must be after start time")
        return
      }

      if (newDate.isSame(dayjs(), "day") && newStartTime.isBefore(dayjs())) {
        showNotification("error", "Start time must be later than the current time")
        return
      }

      if (reason.length > 100) {
        showNotification("error", "Reason is too long. Please shorten it")
        return
      }

      setSubmitting(true)
      submitRef.current = true

      try {
        await handleReschedule(
          selectedSession.id,
          newDate.format("YYYY-MM-DD"),
          newStartTime.format("HH:mm:ss"),
          newEndTime.format("HH:mm:ss"),
          reason,
        )
      } catch (error) {
        console.error("Error in handleSubmit:", error)
      } finally {
        setSubmitting(false)
        submitRef.current = false
      }
    }

    const handleCancel = () => {
      if (!submitting && !submitRef.current) {
        setIsModalVisible(false)
        setSubmitting(false)
        submitRef.current = false
      }
    }

    return (
      <Modal
        title="Reschedule Session"
        open={isModalVisible}
        onCancel={handleCancel}
        onOk={handleSubmit}
        confirmLoading={submitting}
        maskClosable={!submitting && !submitRef.current}
        closable={!submitting && !submitRef.current}
        destroyOnClose={true}
        okButtonProps={{
          disabled: submitting || submitRef.current,
        }}
        cancelButtonProps={{
          disabled: submitting || submitRef.current,
        }}
      >
        <Space direction="vertical" style={{ width: "100%" }}>
          <DatePicker
            value={newDate}
            onChange={setNewDate}
            style={{ width: "100%" }}
            disabledDate={(d) => d.isBefore(dayjs(), "day")}
            disabled={submitting || submitRef.current}
          />
          <Row gutter={8}>
            <Col span={12}>
              <TimePicker
                style={{ width: "100%" }}
                value={newStartTime}
                onChange={(val) => setNewStartTime(roundToNearestHalfHour(val))}
                format="HH:mm"
                placeholder="Start Time"
                minuteStep={30}
                disabled={submitting || submitRef.current}
              />
            </Col>
            <Col span={12}>
              <TimePicker
                style={{ width: "100%" }}
                value={newEndTime}
                onChange={(val) => setNewEndTime(roundToNearestHalfHour(val))}
                format="HH:mm"
                placeholder="End Time"
                minuteStep={30}
                disabled={submitting || submitRef.current}
              />
            </Col>
          </Row>
          <TextArea
            rows={3}
            value={reason}
            onChange={(e) => setReason(e.target.value)}
            maxLength={100}
            placeholder="Reason for rescheduling"
            showCount
            disabled={submitting || submitRef.current}
          />
        </Space>
      </Modal>
    )
  }

  const pendingSessions = sessions.filter((s) => s.status === "Pending").length
  const approvedSessions = sessions.filter((s) => s.status === "Approved").length
  const completedSessions = sessions.filter((s) => s.status === "Completed").length
  const cancelledSessions = sessions.filter((s) => s.status === "Canceled").length
  const rescheduledSessions = sessions.filter((s) => s.status === "Rescheduled").length
  const totalActiveSessions =
    pendingSessions + completedSessions + approvedSessions + cancelledSessions + rescheduledSessions

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
          <h1 style={{ fontSize: "24px", fontWeight: "bold", marginBottom: "8px" }}>Mentor Sessions Tracking</h1>
          <p style={{ color: "#666", marginBottom: "16px" }}>
            Manage your mentoring sessions and track your activities
          </p>

          <Row gutter={16} style={{ marginBottom: "24px" }}>
            <Col span={4}>
              <Card>
                <Statistic
                  title="Pending Sessions"
                  value={pendingSessions}
                  valueStyle={{ color: "#fa8c16" }}
                  prefix={<ExclamationCircleOutlined />}
                />
              </Card>
            </Col>
            <Col span={4}>
              <Card>
                <Statistic
                  title="Approved Sessions"
                  value={approvedSessions}
                  valueStyle={{ color: "#1890ff" }}
                  prefix={<CheckCircleOutlined />}
                />
              </Card>
            </Col>
            <Col span={4}>
              <Card>
                <Statistic
                  title="Completed Sessions"
                  value={completedSessions}
                  valueStyle={{ color: "#52c41a" }}
                  prefix={<CheckCircleOutlined />}
                />
              </Card>
            </Col>
            <Col span={4}>
              <Card>
                <Statistic
                  title="Cancelled Sessions"
                  value={cancelledSessions}
                  valueStyle={{ color: "#ff4d4f" }}
                  prefix={<CloseCircleOutlined />}
                />
              </Card>
            </Col>
            <Col span={4}>
              <Card>
                <Statistic
                  title="Rescheduled Sessions"
                  value={rescheduledSessions}
                  valueStyle={{ color: "#8a2be2" }}
                  prefix={<ClockCircleOutlined />}
                />
              </Card>
            </Col>
            <Col span={4}>
              <Card>
                <Statistic title="Total Sessions" value={totalActiveSessions} prefix={<CalendarOutlined />} />
              </Card>
            </Col>
          </Row>
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

          <TabPane
            tab={
              <Badge count={pastSessions.length} offset={[10, 0]}>
                <span>Past Sessions</span>
              </Badge>
            }
            key="past"
          >
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
