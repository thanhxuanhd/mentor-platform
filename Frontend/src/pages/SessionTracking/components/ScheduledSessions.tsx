"use client";

import { useState, useEffect, useCallback, useRef, useContext, useMemo } from "react";
import {
  Table,
  Button,
  Tag,
  Modal,
  DatePicker,
  Input,
  Space,
  Card,
  Tabs,
  Avatar,
  Badge,
  Row,
  Col,
  Statistic,
  Select,
} from "antd";
import {
  CalendarOutlined,
  ClockCircleOutlined,
  UserOutlined,
  CheckCircleOutlined,
  CloseCircleOutlined,
  ExclamationCircleOutlined,
} from "@ant-design/icons";
import type { ColumnsType } from "antd/es/table";
import dayjs from "dayjs";
import { sessionBookingService, type TimeSlot } from "../../../services/sessiontracking/sessiontracking";
import type { SessionBookingRequest } from "../../../types/SessionBookingTypes";
import { getStatusString } from "../../../types/SessionBookingTypes";
import { AuthContext } from "../../../contexts/AuthContext";

const { TextArea } = Input;
const { TabPane } = Tabs;
const { Option } = Select;

interface Session {
  id: string;
  studentName: string;
  studentAvatar?: string;
  date: string;
  startTime: string;
  endTime: string;
  status: "Pending" | "Approved" | "Completed" | "Cancelled" | "Rescheduled";
  communicationMethod: "VideoCall" | "AudioCall" | "Chat";
  studentEmail?: string;
  timeSlotId?: string;
  mentorId?: string;
  learnerId?: string;
  type?: number;
  lastStatusUpdate?: string;
}

const CustomNotification = ({
  visible,
  type,
  message,
  onClose,
}: {
  visible: boolean;
  type: "success" | "error" | "info";
  message: string;
  onClose: () => void;
}) => (visible ? (
  <div className="fixed top-4 right-4 z-50 max-w-sm w-full">
    <div className={`
      ${type === "success" ? "bg-white border-l-4 border-green-500" :
        type === "error" ? "bg-white border-l-4 border-red-500" :
        "bg-white border-l-4 border-blue-500"} shadow-lg p-4 rounded-lg
      transform transition-all duration-300 ease-in-out animate-in slide-in-from-top-2 fade-in
    `}>
      <div className="flex items-start space-x-3">
        <div className="flex-shrink-0 mt-0.5">
          {type === "success" ? <CheckCircleOutlined className="text-green-500 text-base" /> :
           type === "error" ? <CloseCircleOutlined className="text-red-500 text-base" /> :
           <ExclamationCircleOutlined className="text-blue-500 text-base" />}
        </div>
        <div className="flex-1 min-w-0">
          <p className={`text-sm font-medium ${
            type === "success" ? "text-green-800" :
            type === "error" ? "text-red-800" : "text-blue-800"
          } leading-5`}>{message}</p>
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
) : null);

interface RescheduleModalProps {
  visible: boolean;
  onCancel: () => void;
  onSubmit: (sessionId: string, newTimeslotId: string, reason: string) => Promise<void>;
  selectedSession: Session | null;
  availableTimeslots: TimeSlot[];
  loadingTimeslots: boolean;
  loadAvailableTimeslots: (mentorId: string, date: string) => Promise<void>;
  showNotification: (type: "success" | "error" | "info", message: string) => void;
  user: { id: string } | null;
}

const RescheduleModal: React.FC<RescheduleModalProps> = ({
  visible,
  onCancel,
  onSubmit,
  selectedSession,
  availableTimeslots,
  loadingTimeslots,
  loadAvailableTimeslots,
  showNotification,
  user,
}) => {
  const [newDate, setNewDate] = useState<dayjs.Dayjs | null>(null);
  const [selectedTimeSlotId, setSelectedTimeSlotId] = useState<string | undefined>(undefined);
  const [reason, setReason] = useState("");
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    if (visible && selectedSession) {
      const sessionDate = dayjs(selectedSession.date);
      setNewDate(sessionDate);
      setSelectedTimeSlotId(selectedSession.timeSlotId || undefined);
      setReason("");

      if (user?.id) {
        loadAvailableTimeslots(user.id, sessionDate.format("YYYY-MM-DD"));
      }
    } else if (!visible) { 
      setNewDate(null);
      setSelectedTimeSlotId(undefined);
      setReason("");
    }
  }, [visible, selectedSession, user?.id, loadAvailableTimeslots]);

  const handleSubmit = async () => {
    if (submitting || !newDate || !selectedTimeSlotId || !selectedSession) {
      showNotification("error", "Please select a valid date and time slot.");
      return;
    }

    const selectedSlot = availableTimeslots.find(slot => slot.id === selectedTimeSlotId);
    if (!selectedSlot) {
      showNotification("error", "Invalid time slot selected.");
      return;
    }

    const selectedDateTime = dayjs(`${newDate.format("YYYY-MM-DD")}T${selectedSlot.startTime}`);
    if (selectedDateTime.isBefore(dayjs())) {
      showNotification("error", "Selected time slot is in the past. Please choose a future time.");
      return;
    }

    if (reason.length > 100) {
      showNotification("error", "Reason is too long. Please shorten it.");
      return;
    }

    setSubmitting(true);
    try {
      await onSubmit(selectedSession.id, selectedTimeSlotId, reason);
    } catch (error) {
      showNotification("error", "Failed to reschedule session.");
      console.error("Error in handleSubmit:", error);
    } finally {
      setSubmitting(false);
    }
  };

  const handleCancel = () => {
    if (!submitting) {
      onCancel();
    }
  };

  const handleDateChange = async (date: dayjs.Dayjs | null) => {
    setNewDate(date);
    setSelectedTimeSlotId(undefined);
    
    if (date && user?.id) {
      const dateStr = date.format("YYYY-MM-DD");
      await loadAvailableTimeslots(user.id, dateStr);
    } else {
      if (availableTimeslots.length > 0) {
        showNotification("info", "Date changed. Please select a new time slot.");
      }
    }
  };

  const isTimeSlotDisabled = submitting || loadingTimeslots || !newDate || availableTimeslots.length === 0;

  return (
    <Modal
      title="Reschedule Session"
      open={visible}
      onCancel={handleCancel}
      onOk={handleSubmit}
      confirmLoading={submitting}
      maskClosable={!submitting}
      closable={!submitting}
      okButtonProps={{
        disabled: submitting || !selectedTimeSlotId || !newDate,
      }}
      cancelButtonProps={{ disabled: submitting }}
    >
      <Space direction="vertical" style={{ width: "100%" }}>
        <div>
          <label style={{ display: "block", marginBottom: "8px", fontWeight: 500 }}>Date</label>
          <DatePicker
            value={newDate}
            onChange={handleDateChange}
            style={{ width: "100%" }}
            disabledDate={d => d.isBefore(dayjs(), "day")}
            disabled={submitting}
          />
        </div>
        <div>
          <label style={{ display: "block", marginBottom: "8px", fontWeight: 500 }}>
            Time Slot 
            {availableTimeslots.length > 0 && 
              <span style={{ color: "#52c41a", marginLeft: "8px" }}>
                ({availableTimeslots.length} available)
              </span>
            }
          </label>
          <Select
            style={{ width: "100%" }}
            value={selectedTimeSlotId}
            onChange={setSelectedTimeSlotId}
            placeholder={
              loadingTimeslots 
                ? "Loading time slots..." 
                : !newDate 
                  ? "Please select a date first"
                  : availableTimeslots.length === 0 
                    ? "No available time slots for this date"
                    : "Select time slot"
            }
            disabled={isTimeSlotDisabled}
            showSearch
            allowClear
            loading={loadingTimeslots}
            notFoundContent={
              !newDate
                ? "Please select a date first"
                : loadingTimeslots
                  ? "Loading..."
                  : "No available time slots for this date"
            }
            filterOption={(input, option) =>
              (option?.children as unknown as string)?.toLowerCase().includes(input.toLowerCase())
            }
          >
            {availableTimeslots.map(slot => (
              <Option key={slot.id} value={slot.id}>
                {`${slot.startTime} - ${slot.endTime}`}
              </Option>
            ))}
          </Select>
        </div>
        <div>
          <label style={{ display: "block", marginBottom: "8px", fontWeight: 500 }}>Reason for rescheduling</label>
          <TextArea
            rows={3}
            value={reason}
            onChange={e => setReason(e.target.value)}
            maxLength={100}
            placeholder="Reason for rescheduling"
            showCount
            disabled={submitting}
          />
        </div>
      </Space>
    </Modal>
  );
};


const ScheduleSession = () => {
  const { user } = useContext(AuthContext);
  const [sessions, setSessions] = useState<Session[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedSession, setSelectedSession] = useState<Session | null>(null);
  const [isModalVisible, setIsModalVisible] = useState(false);
  const [activeTab, setActiveTab] = useState("upcoming");
  const [currentTime, setCurrentTime] = useState(dayjs());
  const [availableTimeslots, setAvailableTimeslots] = useState<TimeSlot[]>([]);
  const [loadingTimeslots, setLoadingTimeslots] = useState(false);
  const [notification, setNotification] = useState<{ visible: boolean; type: "success" | "error" | "info"; message: string }>({ visible: false, type: "success", message: "" });
  const [processingSessionIds, setProcessingSessionIds] = useState<Record<string, boolean>>({});
  const [processingTimeSlots, setProcessingTimeSlots] = useState<Record<string, boolean>>({});
  const processingOvertimeRef = useRef(false);

  const statusStyles = {
    colors: { Pending: "orange", Approved: "blue", Completed: "green", Cancelled: "red", Rescheduled: "purple" },
    icons: {
      Pending: <ExclamationCircleOutlined />,
      Approved: <CheckCircleOutlined />,
      Completed: <CheckCircleOutlined />,
      Cancelled: <CloseCircleOutlined />,
      Rescheduled: <ClockCircleOutlined />,
    },
  };

  const communicationIcons = { VideoCall: "Video Call", AudioCall: "Audio Call", Chat: "Chat" };

  useEffect(() => {
    const interval = setInterval(() => setCurrentTime(dayjs()), 60000);
    return () => clearInterval(interval);
  }, []);

  const showNotification = useCallback((type: "success" | "error" | "info", message: string) => {
    setNotification({ visible: true, type, message });
    setTimeout(() => setNotification(prev => ({ ...prev, visible: false })), 4000);
  }, []);

  const hideNotification = useCallback(() => setNotification(prev => ({ ...prev, visible: false })), []);

  const convertApiResponseToSession = (apiData: SessionBookingRequest): Session => ({
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
    mentorId: apiData.mentorId,
  });

  const handleOvertimeSessions = useCallback(async () => {
    if (processingOvertimeRef.current) return;
    processingOvertimeRef.current = true;

    try {
      const now = currentTime;
      const overtimeSessions = sessions.filter(session => {
        const sessionDateTime = dayjs(`${session.date} ${session.startTime}`);
        return sessionDateTime.isBefore(now) && ["Pending", "Approved"].includes(session.status);
      });

      if (overtimeSessions.length > 0) {
        const updatedSessions = await Promise.all(
          overtimeSessions.map(async session => {
            try {
              await sessionBookingService.updateSessionStatus(session.id, "Canceled");
              return { ...session, status: "Cancelled" as const, lastStatusUpdate: now.toISOString() };
            } catch (error) {
              console.error(`Failed to cancel overtime session ${session.id}:`, error);
              return session;
            }
          })
        );

        setSessions(prev => prev.map(session =>
          updatedSessions.find(u => u.id === session.id) || session
        ));

        if (updatedSessions.some(s => s.status === "Cancelled")) {
          showNotification("info", `${updatedSessions.filter(s => s.status === "Cancelled").length} overtime sessions have been cancelled and moved to Past Sessions`);
        }
      }
    } catch (error) {
      console.error("Error handling overtime sessions:", error);
    } finally {
      processingOvertimeRef.current = false;
    }
  }, [sessions, currentTime, showNotification]);

  useEffect(() => {
    if (sessions.length > 0) handleOvertimeSessions();
  }, [currentTime, handleOvertimeSessions, sessions.length]);

  const loadSessions = useCallback(async () => {
    try {
      setLoading(true);
      const response = await sessionBookingService.getSessionBookings(user?.id);
      if (!response || !Array.isArray(response)) throw new Error("Invalid response format");
      const convertedSessions = response.map(convertApiResponseToSession);
      setSessions(convertedSessions);
      console.log("Sessions loaded:", convertedSessions.length);
    } catch (error) {
      showNotification("error", `Failed to load sessions: ${error}`);
      console.error("Error loading sessions:", error);
    } finally {
      setLoading(false);
    }
  }, [showNotification]);

  useEffect(() => {
    loadSessions();
  }, [loadSessions]);

  const loadAvailableTimeslots = useCallback(async (mentorId: string, date: string) => {
    try {
      setLoadingTimeslots(true);
      const timeslots = await sessionBookingService.getAvailableTimeslotsByDate(mentorId, date);
      const enrichedTimeslots = timeslots.map(slot => ({
        ...slot,
        startTime: slot.startTime || "00:00",
        endTime: slot.endTime || "00:00",
        date: slot.date || date, 
      }));
      setAvailableTimeslots(enrichedTimeslots); 
      
      if (enrichedTimeslots.length === 0) {
        showNotification("info", "No available time slots for the selected date.");
      } else {
        showNotification("success", `Found ${enrichedTimeslots.length} available time slots.`);
      }
    } catch (error) {
      console.error("Error loading available timeslots:", error);
      showNotification("error", "Failed to load available time slots");
      setAvailableTimeslots([]);
    } finally {
      setLoadingTimeslots(false);
    }
  }, [showNotification]);

  const getSessionSessions = (statusFilter: Session["status"][]) =>
    sessions.filter(session => {
      const sessionDateTime = dayjs(`${session.date} ${session.startTime}`);
      const isOvertime = sessionDateTime.isBefore(currentTime);
      return statusFilter.includes(session.status) || (isOvertime && session.lastStatusUpdate && dayjs(session.lastStatusUpdate).isAfter(sessionDateTime));
    });

  const upcomingSessions = useMemo(() => getSessionSessions(["Pending", "Approved", "Rescheduled"]).filter(
    session => !dayjs(`${session.date} ${session.startTime}`).isBefore(currentTime)
  ), [sessions, currentTime]);

  const pastSessions = useMemo(() => getSessionSessions(["Completed", "Cancelled"]), [sessions]);

  const handleStatusChange = async (sessionId: string, newStatus: Session["status"]) => {
    try {
      const session = sessions.find(s => s.id === sessionId);
      if (!session) {
        showNotification("error", "Session not found");
        return;
      }

      setProcessingSessionIds(prev => ({ ...prev, [sessionId]: true }));
 
      if (newStatus === "Approved" && session.timeSlotId) {
        setProcessingTimeSlots(prev => ({ ...prev, [session.timeSlotId!]: true }));
      }

      await sessionBookingService.updateSessionStatus(sessionId, newStatus);
      setSessions(prev => prev.map(s => s.id === sessionId ? { ...s, status: newStatus, lastStatusUpdate: dayjs().toISOString() } : s));
      await loadSessions();

      const messages: Record<string, string> = {
        Approved: "Session approved successfully",
        Completed: "Session completed and moved to Past Sessions",
        Canceled: "Session cancelled and moved to Past Sessions",
        default: `Status updated to ${newStatus} successfully`,
      };
      showNotification("success", messages[newStatus] ?? messages.default);
    } catch (error) {
      showNotification("error", `Failed to update session status: ${error instanceof Error ? error.message : "Unknown error"}`);
      console.error("Error updating session status:", error);
    } finally {
      const session = sessions.find(s => s.id === sessionId);
      if (session) {
        setProcessingSessionIds(prev => ({ ...prev, [sessionId]: false }));
        if (session.timeSlotId) {
          setProcessingTimeSlots(prev => ({ ...prev, [session.timeSlotId!]: false }));
        }
      }
    }
  };

  const handleRescheduleSubmit = async (sessionId: string, newTimeslotId: string, reason: string) => {
    const session = sessions.find(s => s.id === sessionId);
    if (!session) {
      showNotification("error", "Session not found");
      return;
    }

    try {
      await sessionBookingService.rescheduleSession(sessionId, { timeslotId: newTimeslotId, reason });
      setSessions(prev => prev.map(s => s.id === sessionId ? { ...s, status: "Rescheduled" as const, lastStatusUpdate: dayjs().toISOString() } : s));
      showNotification("success", session.studentEmail ? `Session rescheduled successfully. Email notification sent to ${session.studentEmail}` : "Session rescheduled successfully");
      setIsModalVisible(false);
      await loadSessions();
    } catch (error) {
      showNotification("error", "Failed to reschedule session");
      console.error("Error rescheduling session:", error);
      throw error;
    }
  };

  const openRescheduleModal = async (session: Session) => {
    try {

      const sessionDetails = await sessionBookingService.getSessionDetails(session.id);
      const sessionWithDetails: Session = {
        ...session,
        timeSlotId: sessionDetails.timeSlotId,
        startTime: sessionDetails.startTime,
        endTime: sessionDetails.endTime,
        mentorId: sessionDetails.mentorId || session.mentorId,
      };
      setSelectedSession(sessionWithDetails);
      setIsModalVisible(true);

    } catch (error) {
      showNotification("error", "Failed to load session details for rescheduling. Please try again.");
      console.error("Error loading session details for reschedule:", error);
      setSelectedSession(session);
      setIsModalVisible(true);
    }
  };

  const getActionsColumn = (): ColumnsType<Session>[0] => ({
    title: "Actions",
    key: "actions",
    render: (_, record) => {
      const isProcessing = processingSessionIds[record.id] || false;
      const isTimeSlotProcessing = record.timeSlotId ? processingTimeSlots[record.timeSlotId] || false : false;

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
              <Button
                type="default"
                size="small"
                onClick={() => openRescheduleModal(record)}
                disabled={isProcessing}
              >
                Reschedule
              </Button>
            </>
          )}
          {["Pending", "Approved"].includes(record.status) && (
            <Button
              danger
              size="small"
              onClick={() => handleStatusChange(record.id, "Cancelled")}
              loading={isProcessing}
              disabled={isProcessing || (isTimeSlotProcessing && !processingSessionIds[record.id])}
            >
              Cancel
            </Button>
          )}
        </Space>
      );
    },
  });

  const getBaseColumns = (): ColumnsType<Session> => [
    {
      title: "Learner",
      dataIndex: "studentName",
      key: "studentName",
      render: (name, record) => (
        <Space>
          <Avatar src={record.studentAvatar} icon={<UserOutlined />} size="small" />
          <span>{name}</span>
        </Space>
      ),
    },
    {
      title: "Date & Time",
      key: "datetime",
      render: (_, record) => (
        <Space direction="vertical" size="small">
          <span><CalendarOutlined /> {dayjs(record.date).format("MMM DD,YYYY")}</span>
          <span><ClockCircleOutlined /> {record.startTime} - {record.endTime}</span>
        </Space>
      ),
    },
    {
      title: "Method",
      dataIndex: "communicationMethod",
      key: "communicationMethod",
      render: (method) => <span>{communicationIcons[method as keyof typeof communicationIcons]}</span>,
    },
    {
      title: "Status",
      dataIndex: "status",
      key: "status",
      render: (status: Session["status"]) => (
        <Tag color={statusStyles.colors[status]} icon={statusStyles.icons[status]}>
          {status.toUpperCase()}
        </Tag>
      ),
    },
  ];

  const upcomingColumns = [...getBaseColumns(), getActionsColumn()];
  const pastColumns = [...getBaseColumns()];

  const sessionStats = useMemo(() => ({
    pending: sessions.filter(s => s.status === "Pending").length,
    approved: sessions.filter(s => s.status === "Approved").length,
    completed: sessions.filter(s => s.status === "Completed").length,
    cancelled: sessions.filter(s => s.status === "Cancelled").length,
    rescheduled: sessions.filter(s => s.status === "Rescheduled").length,
    total: sessions.length,
  }), [sessions]);

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
          <p style={{ color: "#666", marginBottom: "16px" }}>Manage your mentoring sessions and track your activities</p>
          <Row gutter={16} style={{ marginBottom: "24px" }}>
            {[
              { title: "Pending Sessions", value: sessionStats.pending, color: "#fa8c16", icon: <ExclamationCircleOutlined /> },
              { title: "Approved Sessions", value: sessionStats.approved, color: "#1890ff", icon: <CheckCircleOutlined /> },
              { title: "Completed Sessions", value: sessionStats.completed, color: "#52c41a", icon: <CheckCircleOutlined /> },
              { title: "Cancelled Sessions", value: sessionStats.cancelled, color: "#ff4d4f", icon: <CloseCircleOutlined /> },
              { title: "Rescheduled Sessions", value: sessionStats.rescheduled, color: "#8a2be2", icon: <ClockCircleOutlined /> },
              { title: "Total Sessions", value: sessionStats.total, color: "white", icon: <CalendarOutlined /> },
            ].map((stat, index) => (
              <Col span={4} key={index}>
                <Card>
                  <Statistic
                    title={stat.title}
                    value={stat.value}
                    valueStyle={{ color: stat.color }}
                    prefix={stat.icon}
                  />
                </Card>
              </Col>
            ))}
          </Row>
        </div>
        <Tabs activeKey={activeTab} onChange={setActiveTab}>
          <TabPane tab={<Badge count={upcomingSessions.length} offset={[10, 0]}><span>Upcoming Sessions</span></Badge>} key="upcoming">
            <Table
              columns={upcomingColumns}
              dataSource={upcomingSessions}
              rowKey="id"
              pagination={{ pageSize: 10 }}
              scroll={{ x: 800 }}
              loading={loading}
            />
          </TabPane>
          <TabPane tab={<Badge count={pastSessions.length} offset={[10, 0]}><span>Past Sessions</span></Badge>} key="past">
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
      <RescheduleModal
        visible={isModalVisible}
        onCancel={() => setIsModalVisible(false)}
        onSubmit={handleRescheduleSubmit}
        selectedSession={selectedSession}
        availableTimeslots={availableTimeslots}
        loadingTimeslots={loadingTimeslots}
        loadAvailableTimeslots={loadAvailableTimeslots}
        showNotification={showNotification}
        user={user}
      />
    </div>
  );
};

export default ScheduleSession;