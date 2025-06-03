"use client"

import { useState } from "react"
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
  Tooltip,
  message,
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
  EyeOutlined,
} from "@ant-design/icons"
import type { ColumnsType } from "antd/es/table"
import dayjs from "dayjs"

const { TextArea } = Input
const { TabPane } = Tabs

interface Session {
  id: string
  studentName: string
  studentAvatar?: string
  subject: string
  date: string
  time: string
  duration: number
  status: "pending" | "approved" | "completed" | "cancelled" | "rescheduled"
  communicationMethod: "video" | "audio" | "chat"
  notes?: string
  studentEmail: string
}

const mockSessions: Session[] = [
  {
    id: "1",
    studentName: "Alice Johnson",
    studentAvatar: "/placeholder.svg?height=32&width=32",
    subject: "React Development",
    date: "2024-01-15",
    time: "10:00",
    duration: 60,
    status: "pending",
    communicationMethod: "video",
    studentEmail: "alice@example.com",
  },
  {
    id: "2",
    studentName: "Bob Smith",
    subject: "JavaScript Fundamentals",
    date: "2024-01-16",
    time: "14:00",
    duration: 45,
    status: "approved",
    communicationMethod: "video",
    studentEmail: "bob@example.com",
  },
  {
    id: "3",
    studentName: "Carol Davis",
    subject: "Node.js Backend",
    date: "2024-01-10",
    time: "09:00",
    duration: 90,
    status: "completed",
    communicationMethod: "video",
    studentEmail: "carol@example.com",
  },
  {
    id: "4",
    studentName: "David Wilson",
    subject: "Database Design",
    date: "2024-01-12",
    time: "16:00",
    duration: 60,
    status: "cancelled",
    communicationMethod: "audio",
    studentEmail: "david@example.com",
  },
]

export default function ScheduleSession() {
  const [sessions, setSessions] = useState<Session[]>(mockSessions)
  const [selectedSession, setSelectedSession] = useState<Session | null>(null)
  const [isModalVisible, setIsModalVisible] = useState(false)
  const [modalType, setModalType] = useState<"view" | "edit" | "reschedule">("view")
  const [activeTab, setActiveTab] = useState("upcoming")

  const statusColors = {
    pending: "orange",
    approved: "blue",
    completed: "green",
    cancelled: "red",
    rescheduled: "purple",
  }

  const statusIcons = {
    pending: <ExclamationCircleOutlined />,
    approved: <CheckCircleOutlined />,
    completed: <CheckCircleOutlined />,
    cancelled: <CloseCircleOutlined />,
    rescheduled: <ClockCircleOutlined />,
  }

  const communicationIcons = {
    video: "📹",
    audio: "🎧",
    chat: "💬",
  }

  const upcomingSessions = sessions.filter(
    (session) =>
      (["pending", "approved"].includes(session.status) && dayjs(session.date).isAfter(dayjs(), "day")) ||
      dayjs(session.date).isSame(dayjs(), "day"),
  )

  const pastSessions = sessions.filter(
    (session) => ["completed", "cancelled"].includes(session.status) || dayjs(session.date).isBefore(dayjs(), "day"),
  )

  const handleStatusChange = (sessionId: string, newStatus: Session["status"]) => {
    setSessions((prev) =>
      prev.map((session) => (session.id === sessionId ? { ...session, status: newStatus } : session)),
    )

    const session = sessions.find((s) => s.id === sessionId)
    if (session) {
      // Simulate sending email notification
      message.success(`Status updated to ${newStatus}. Email notification sent to ${session.studentEmail}`)
    }
  }

  const handleReschedule = (sessionId: string, newDate: string, newTime: string) => {
    setSessions((prev) =>
      prev.map((session) =>
        session.id === sessionId
          ? { ...session, date: newDate, time: newTime, status: "rescheduled" as const }
          : session,
      ),
    )

    const session = sessions.find((s) => s.id === sessionId)
    if (session) {
      message.success(`Session rescheduled. Email notification sent to ${session.studentEmail}`)
    }
    setIsModalVisible(false)
  }

  const openModal = (session: Session, type: "view" | "edit" | "reschedule") => {
    setSelectedSession(session)
    setModalType(type)
    setIsModalVisible(true)
  }

  const columns: ColumnsType<Session> = [
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
      title: "Subject",
      dataIndex: "subject",
      key: "subject",
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
            <ClockCircleOutlined /> {record.time} ({record.duration}min)
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
    {
      title: "Actions",
      key: "actions",
      render: (_, record: Session) => (
        <Space>
          <Tooltip title="View Details">
            <Button type="text" icon={<EyeOutlined />} onClick={() => openModal(record, "view")} />
          </Tooltip>

          {record.status === "pending" && (
            <>
              <Button type="primary" size="small" onClick={() => handleStatusChange(record.id, "approved")}>
                Accept
              </Button>
              <Button type="default" size="small" onClick={() => openModal(record, "reschedule")}>
                Reschedule
              </Button>
            </>
          )}

          {record.status === "approved" && (
            <>
              <Button type="primary" size="small" onClick={() => handleStatusChange(record.id, "completed")}>
                Mark Complete
              </Button>
              <Button type="default" size="small" onClick={() => openModal(record, "reschedule")}>
                Reschedule
              </Button>
            </>
          )}

          {["pending", "approved"].includes(record.status) && (
            <Button danger size="small" onClick={() => handleStatusChange(record.id, "cancelled")}>
              Cancel
            </Button>
          )}
        </Space>
      ),
    },
  ]

  const RescheduleModal = () => {
    const [newDate, setNewDate] = useState<dayjs.Dayjs | null>(null)
    const [newTime, setNewTime] = useState<dayjs.Dayjs | null>(null)
    const [rescheduleReason, setRescheduleReason] = useState("")

    const handleSubmit = () => {
      if (newDate && newTime && selectedSession) {
        handleReschedule(selectedSession.id, newDate.format("YYYY-MM-DD"), newTime.format("HH:mm"))
        setNewDate(null)
        setNewTime(null)
        setRescheduleReason("")
      }
    }

    return (
      <Modal
        title="Reschedule Session"
        open={isModalVisible && modalType === "reschedule"}
        onCancel={() => setIsModalVisible(false)}
        onOk={handleSubmit}
        okText="Reschedule"
      >
        <Space direction="vertical" style={{ width: "100%" }}>
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
            <label>New Time:</label>
            <TimePicker style={{ width: "100%", marginTop: 8 }} value={newTime} onChange={setNewTime} format="HH:mm" />
          </div>

          <div>
            <label>Reason for Reschedule:</label>
            <TextArea
              rows={3}
              style={{ marginTop: 8 }}
              value={rescheduleReason}
              onChange={(e) => setRescheduleReason(e.target.value)}
              placeholder="Optional: Explain why you need to reschedule..."
            />
          </div>
        </Space>
      </Modal>
    )
  }

  const SessionDetailsModal = () => (
    <Modal
      title="Session Details"
      open={isModalVisible && modalType === "view"}
      onCancel={() => setIsModalVisible(false)}
      footer={[
        <Button key="close" onClick={() => setIsModalVisible(false)}>
          Close
        </Button>,
      ]}
    >
      {selectedSession && (
        <Space direction="vertical" style={{ width: "100%" }}>
          <Card size="small">
            <Row gutter={16}>
              <Col span={12}>
                <Statistic title="Student" value={selectedSession.studentName} />
              </Col>
              <Col span={12}>
                <Statistic title="Subject" value={selectedSession.subject} />
              </Col>
            </Row>
          </Card>

          <Card size="small">
            <Row gutter={16}>
              <Col span={8}>
                <Statistic title="Date" value={dayjs(selectedSession.date).format("MMM DD, YYYY")} />
              </Col>
              <Col span={8}>
                <Statistic title="Time" value={selectedSession.time} />
              </Col>
              <Col span={8}>
                <Statistic title="Duration" value={`${selectedSession.duration} min`} />
              </Col>
            </Row>
          </Card>

          <Card size="small">
            <Row gutter={16}>
              <Col span={12}>
                <div>
                  <strong>Status:</strong>
                  <Tag
                    color={statusColors[selectedSession.status]}
                    icon={statusIcons[selectedSession.status]}
                    style={{ marginLeft: 8 }}
                  >
                    {selectedSession.status.toUpperCase()}
                  </Tag>
                </div>
              </Col>
              <Col span={12}>
                <div>
                  <strong>Method:</strong> {communicationIcons[selectedSession.communicationMethod]}{" "}
                  {selectedSession.communicationMethod}
                </div>
              </Col>
            </Row>
          </Card>

          {selectedSession.notes && (
            <Card size="small" title="Notes">
              <p>{selectedSession.notes}</p>
            </Card>
          )}
        </Space>
      )}
    </Modal>
  )

  return (
    <div style={{ padding: "24px", minHeight: "100vh"}}>
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
                  value={sessions.filter((s) => s.status === "pending").length}
                  valueStyle={{ color: "#fa8c16" }}
                  prefix={<ExclamationCircleOutlined />}
                />
              </Card>
            </Col>
            <Col span={6}>
              <Card>
                <Statistic
                  title="Approved Sessions"
                  value={sessions.filter((s) => s.status === "approved").length}
                  valueStyle={{ color: "#1890ff" }}
                  prefix={<CheckCircleOutlined />}
                />
              </Card>
            </Col>
            <Col span={6}>
              <Card>
                <Statistic
                  title="Completed Sessions"
                  value={sessions.filter((s) => s.status === "completed").length}
                  valueStyle={{ color: "#52c41a" }}
                  prefix={<CheckCircleOutlined />}
                />
              </Card>
            </Col>
            <Col span={6}>
              <Card>
                <Statistic title="Total Sessions" value={sessions.length} prefix={<CalendarOutlined />} />
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
              columns={columns}
              dataSource={upcomingSessions}
              rowKey="id"
              pagination={{ pageSize: 10 }}
              scroll={{ x: 800 }}
            />
          </TabPane>

          <TabPane tab="Past Sessions" key="past">
            <Table
              columns={columns}
              dataSource={pastSessions}
              rowKey="id"
              pagination={{ pageSize: 10 }}
              scroll={{ x: 800 }}
            />
          </TabPane>
        </Tabs>
      </Card>

      <SessionDetailsModal />
      <RescheduleModal />
    </div>
  )
}
