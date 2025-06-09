"use client";

import { useState, useEffect, useCallback, useRef } from "react";
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
  status: "Pending" | "Approved" | "Completed" | "Canceled" | "Rescheduled";
  communicationMethod: "VideoCall" | "AudioCall" | "Chat";
  studentEmail?: string;
  timeSlotId?: string;
  learnerId?: string;
  type?: number;
  lastStatusUpdate?: string;
  mentorId?: string;
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

const ScheduleSession = () => {
  const [sessions, setSessions] = useState<Session[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedSession, setSelectedSession] = useState<Session | null>(null);
  const [isModalVisible, setIsModalVisible] = useState(false);
  const [activeTab, setActiveTab] = useState("upcoming");
  const [currentTime, setCurrentTime] = useState(dayjs());
  const [availableTimeslots, setAvailableTimeslots] = useState<TimeSlot[]>([]);
  const [loadingTimeslots, setLoadingTimeslots] = useState(false);
  const [notification, setNotification] = useState({ visible: false, type: "success" as const, message: "" });
  const [processingSessionIds, setProcessingSessionIds] = useState<Record<string, boolean>>({});
  const [processingTimeSlots, setProcessingTimeSlots] = useState<Record<string, boolean>>({});
  const processingOvertimeRef = useRef(false);

  const statusStyles = {
    colors: { Pending: "orange", Approved: "blue", Completed: "green", Canceled: "red", Rescheduled: "purple" },
    icons: {
      Pending: <ExclamationCircleOutlined />,
      Approved: <CheckCircleOutlined />,
      Completed: <CheckCircleOutlined />,
      Canceled: <CloseCircleOutlined />,
      Rescheduled: <ClockCircleOutlined />,
    },
  };

  const communicationIcons = { VideoCall: "Video Call", AudioCall: "Audio Call", Chat: "Chat" };

  useEffect(() => {
    const interval = setInterval(() => setCurrentTime(dayjs()), 60000);
    return () => clearInterval(interval);
  }, []);

  const showNotification = (type: "success" | "error" | "info", message: string) => {
    setNotification({ visible: true, type, message });
    setTimeout(() => setNotification(prev => ({ ...prev, visible: false })), 4000);
  };

  const hideNotification = () => setNotification(prev => ({ ...prev, visible: false }));

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
              return { ...session, status: "Canceled" as const, lastStatusUpdate: now.toISOString() };
            } catch (error) {
              console.error(`Failed to cancel overtime session ${session.id}:`, error);
              return session;
            }
          })
        );

        setSessions(prev => prev.map(session => 
          updatedSessions.find(u => u.id === session.id) || session
        ));

        if (updatedSessions.some(s => s.status === "Canceled")) {
          showNotification("info", `${updatedSessions.filter(s => s.status === "Canceled").length} overtime sessions have been cancelled and moved to Past Sessions`);
        }
      }
    } catch (error) {
      console.error("Error handling overtime sessions:", error);
    } finally {
      processingOvertimeRef.current = false;
    }
  }, [sessions, currentTime]);

  useEffect(() => {
    if (sessions.length > 0) handleOvertimeSessions();
  }, [currentTime, handleOvertimeSessions]);

  useEffect(() => {
    loadSessions();
  }, []);

  const loadSessions = async () => {
    try {
      setLoading(true);
      const response = await sessionBookingService.getSessionBookings();
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
  };

  const loadAvailableTimeslots = async (mentorId: string, date?: string) => {
    try {
      setLoadingTimeslots(true);
      const timeslots = await sessionBookingService.getAvailableTimeslotsByDate(mentorId, date);
      const enrichedTimeslots = timeslots.map(slot => ({
        ...slot,
        startTime: slot.startTime || (selectedSession?.startTime || "N/A"),
        endTime: slot.endTime || (selectedSession?.endTime || "N/A"),
      }));
      setAvailableTimeslots(enrichedTimeslots);
      console.log("Enriched available timeslots:", enrichedTimeslots);
    } catch (error) {
      console.error("Error loading available timeslots:", error);
      showNotification("error", "Failed to load available time slots");
      setAvailableTimeslots([]);
    } finally {
      setLoadingTimeslots(false);
    }
  };

  const getSessionSessions = (statusFilter: Session["status"][]) =>
    sessions.filter(session => {
      const sessionDateTime = dayjs(`${session.date} ${session.startTime}`);
      const isOvertime = sessionDateTime.isBefore(currentTime);
      return statusFilter.includes(session.status) || (isOvertime && session.lastStatusUpdate && dayjs(session.lastStatusUpdate).isAfter(sessionDateTime));
    });

  const upcomingSessions = getSessionSessions(["Pending", "Approved", "Rescheduled"]).filter(
    session => !dayjs(`${session.date} ${session.startTime}`).isBefore(currentTime)
  );
  const pastSessions = getSessionSessions(["Completed", "Canceled"]);

  const handleStatusChange = async (sessionId: string, newStatus: Session["status"]) => {
    try {
      const session = sessions.find(s => s.id === sessionId);
      if (!session) {
        showNotification("error", "Session not found");
        return;
      }

      setProcessingSessionIds(prev => ({ ...prev, [sessionId]: true }));
      if (newStatus === "Approved") {
        setProcessingTimeSlots(prev => ({ ...prev, [`${session.date}_${session.startTime}_${session.endTime}`]: true }));
      }

      await sessionBookingService.updateSessionStatus(sessionId, newStatus);
      setSessions(prev => prev.map(s => s.id === sessionId ? { ...s, status: newStatus, lastStatusUpdate: dayjs().toISOString() } : s));
      await loadSessions();

      const messages = {
        Approved: "Session approved successfully",
        Completed: "Session completed and moved to Past Sessions",
        Canceled: "Session cancelled and moved to Past Sessions",
        default: `Status updated to ${newStatus} successfully`,
      };
      showNotification("success", messages[newStatus] || messages.default);
    } catch (error) {
      showNotification("error", `Failed to update session status: ${error instanceof Error ? error.message : "Unknown error"}`);
      console.error("Error updating session status:", error);
    } finally {
      const session = sessions.find(s => s.id === sessionId);
      if (session) {
        setProcessingSessionIds(prev => ({ ...prev, [sessionId]: false }));
        setProcessingTimeSlots(prev => ({ ...prev, [`${session.date}_${session.startTime}_${session.endTime}`]: false }));
      }
    }
  };

  const handleReschedule = async (sessionId: string, newTimeslotId: string, reason: string) => {
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
    } catch (error) {
      showNotification("error", "Failed to reschedule session");
      console.error("Error rescheduling session:", error);
    }
  };

  const openRescheduleModal = async (session: Session) => {
    try {
      const sessionDetails = await sessionBookingService.getSessionDetails(session.id);
      const sessionWithDetails = {
        ...session,
        timeSlotId: sessionDetails.timeSlotId,
        startTime: sessionDetails.startTime,
        endTime: sessionDetails.endTime,
        mentorId: sessionDetails.mentorId || session.mentorId,
      };
      setSelectedSession(sessionWithDetails);
      setIsModalVisible(true);
      if (sessionWithDetails.mentorId) await loadAvailableTimeslots(sessionWithDetails.mentorId, sessionWithDetails.date);
    } catch (error) {
      showNotification("error", "Failed to load session details");
      console.error("Error loading session details:", error);
      setSelectedSession(session);
      setIsModalVisible(true);
    }
  };

  const getActionsColumn = (): ColumnsType<Session>[0] => ({
    title: "Actions",
    key: "actions",
    render: (_, record) => {
      const isProcessing = processingSessionIds[record.id] || false;
      const timeSlotKey = `${record.date}_${record.startTime}_${record.endTime}`;
      const isTimeSlotProcessing = processingTimeSlots[timeSlotKey] || false;

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
              onClick={() => handleStatusChange(record.id, "Canceled")}
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
      render: (status) => (
        <Tag color={statusStyles.colors[status]} icon={statusStyles.icons[status]}>
          {status.toUpperCase()}
        </Tag>
      ),
    },
  ];

  const upcomingColumns = [...getBaseColumns(), getActionsColumn()];
  const pastColumns = [...getBaseColumns()];

  const RescheduleModal = () => {
    const [newDate, setNewDate] = useState<dayjs.Dayjs | null>(null);
    const [selectedTimeSlot, setSelectedTimeSlot] = useState<string>("");
    const [reason, setReason] = useState("");
    const [submitting, setSubmitting] = useState(false);
    const submitRef = useRef(false);

    useEffect(() => {
      if (selectedSession && isModalVisible) {
        if (!newDate) setNewDate(dayjs(selectedSession.date));
        setSelectedTimeSlot(selectedSession.timeSlotId || ""); 
        setSelectedTimeSlot(`${selectedSession.startTime} - ${selectedSession.endTime}`);
        setReason("");
        setSubmitting(false);
        submitRef.current = false;

        if (selectedSession.mentorId) {
          loadAvailableTimeslots(selectedSession.mentorId, selectedSession.date).then(() => {
            if (selectedSession.timeSlotId && !availableTimeslots.some(slot => slot.id === selectedSession.timeSlotId)) {
              const currentTimeSlot: TimeSlot = {
                id: selectedSession.timeSlotId,
                startTime: selectedSession.startTime || "",
                endTime: selectedSession.endTime || "",
                date: selectedSession.date,
                mentorId: selectedSession.mentorId,
                mentorName: "", 
                isBooked: true,
              };
              setAvailableTimeslots(prev => {
                if (!prev.some(slot => slot.id === currentTimeSlot.id)) {
                  return [...prev, currentTimeSlot];
                }
                return prev;
              });
              console.log("Added current timeslot:", currentTimeSlot);
            }
          });
        }
      }
    }, [selectedSession, isModalVisible]); 

    const getAvailableTimeslotsForDate = (date: dayjs.Dayjs | null) =>
      date ? availableTimeslots.filter(slot => dayjs(slot.date).format("YYYY-MM-DD") === date.format("YYYY-MM-DD")) : [];

    const availableTimeslotsForDate = getAvailableTimeslotsForDate(newDate);

    const handleSubmit = async () => {
      if (submitting || submitRef.current || !newDate || !selectedTimeSlot || !selectedSession) {
        showNotification("error", "Please fill in all fields");
        return;
      }

      const selectedSlot = availableTimeslots.find(slot => slot.id === selectedTimeSlot);
      if (!selectedSlot) {
        showNotification("error", "Invalid time slot selected");
        return;
      }

      if (newDate.isSame(dayjs(), "day") && dayjs(selectedSlot.startTime, "HH:mm:ss").isBefore(dayjs())) {
        showNotification("error", "Start time must be later than current time");
        return;
      }

      if (reason.length > 100) {
        showNotification("error", "Reason is too long. Please shorten it.");
        return;
      }

      setSubmitting(true);
      submitRef.current = true;
      try {
        await handleReschedule(selectedSession.id, selectedTimeSlot, reason);
      } catch (error) {
        showNotification("error", "Failed to reschedule session");
        console.error("Error in handleSubmit:", error);
      } finally {
        setSubmitting(false);
        submitRef.current = false;
      }
    };

    const handleCancel = () => {
      if (!submitting && !submitRef.current) {
        setIsModalVisible(false);
        setSubmitting(false);
        submitRef.current = false;
        setAvailableTimeslots([]);
        setNewDate(null); 
        setSelectedTimeSlot(""); 
        setReason("");
      }
    };
    const handleDateChange = async (date: dayjs.Dayjs | null) => {
      setNewDate(date);
      setSelectedTimeSlot("");
      if (date && selectedSession?.mentorId) {
        await loadAvailableTimeslots(selectedSession.mentorId, date.format("YYYY-MM-DD"));
      }
    };

    return (
      <Modal
        title="Reschedule Session"
        open={isModalVisible}
        onCancel={handleCancel}
        onOk={handleSubmit}
        confirmLoading={submitting}
        maskClosable={!submitting && !submitRef.current}
        closable={!submitting && !submitRef.current}
        destroyOnClose
        okButtonProps={{ disabled: submitting || submitRef.current || !selectedTimeSlot || !newDate || availableTimeslotsForDate.length === 0 }}
        cancelButtonProps={{ disabled: submitting || submitRef.current }}
      >
        <Space direction="vertical" style={{ width: "100%" }}>
          <div>
            <label style={{ display: "block", marginBottom: "8px", fontWeight: 500 }}>Date</label>
            <DatePicker
              value={newDate}
              onChange={handleDateChange}
              style={{ width: "100%" }}
              disabledDate={d => d.isBefore(dayjs(), "day")}
              disabled={submitting || submitRef.current}
            />
          </div>
          <div>
            <label style={{ display: "block", marginBottom: "8px", fontWeight: 500 }}>Time Slot</label>
            <Select
              style={{ width: "100%" }}
              value={selectedTimeSlot}
              onChange={setSelectedTimeSlot}
              placeholder={loadingTimeslots ? "Loading time slots..." : "Select time slot"}
              disabled={submitting || submitRef.current || loadingTimeslots || !newDate}
              showSearch
              allowClear
              loading={loadingTimeslots}
              notFoundContent={!newDate ? "Please select a date first" : "No data"}
              filterOption={(input, option) =>
                (option?.children as unknown as string)?.toLowerCase().includes(input.toLowerCase())
              }
            >
              {availableTimeslotsForDate.map(slot => (
                <Option key={slot.id} value={slot.id}>
                  {slot.startTime && slot.endTime ? `${slot.startTime} - ${slot.endTime}` : `${slot.id} (Invalid time)`}
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
              disabled={submitting || submitRef.current}
            />
          </div>
        </Space>
      </Modal>
    );
  };

  const sessionStats = {
    pending: sessions.filter(s => s.status === "Pending").length,
    approved: sessions.filter(s => s.status === "Approved").length,
    completed: sessions.filter(s => s.status === "Completed").length,
    cancelled: sessions.filter(s => s.status === "Canceled").length,
    rescheduled: sessions.filter(s => s.status === "Rescheduled").length,
    total: sessions.length,
  };

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
              { title: "Total Sessions", value: sessionStats.total, color: "#000", icon: <CalendarOutlined /> },
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
      <RescheduleModal />
    </div>
  );
};

export default ScheduleSession;