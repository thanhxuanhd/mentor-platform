import { useState, useEffect, useCallback } from "react"
import { Card, Typography, Button, App } from "antd"
import {
  UserOutlined,
  BookOutlined,
  CheckSquareOutlined,
  FileOutlined,
  TrophyOutlined,
  CalendarOutlined,
  ArrowRightOutlined,
} from "@ant-design/icons"
import { dashboardService } from "../../../services/adminDashboard/adminDashboardService"
import Loading from "../../../components/Loading"
import type { NotificationProps } from "../../../types/Notification"
import ActivityLogTable from "./components/ActivityLogs"
import type { ActivityLogRequest, ActivityLogResponse } from "../../../types/ActivityLogType"
import { getCurrentWeekDates } from "../../../utils/CurrentDate"
import { activityLogService } from "../../../services/adminDashboard/activityLogService"
import QuickActions from "./components/QuickActions"

const { Text } = Typography

interface DashboardMetrics {
  totalUsers: number
  totalMentors: number
  totalLearners: number
  totalResources: number
  sessionsThisWeek: number
  pendingApplications: number
}

const defaultLogFilter: ActivityLogRequest = {
  startDateTime: getCurrentWeekDates().startDate,
  endDateTime: getCurrentWeekDates().endDate,
  keyword: '',
  pageSize: 5,
  pageIndex: 1,
}

export default function AdminDashboard() {
  const [metrics, setMetrics] = useState<DashboardMetrics | null>(null)
  const [loading, setLoading] = useState<boolean>(true)
  const [notify, setNotify] = useState<NotificationProps | null>(null);
  const { notification } = App.useApp();
  const [logLoading, setLogLoading] = useState<boolean>(true)
  const [activityLog, setActivityLog] = useState<ActivityLogResponse[]>([]);
  const [activityLogFilter, setActivityLogFilter] = useState<ActivityLogRequest>(defaultLogFilter);
  const [pagination, setPagination] = useState({
    pageIndex: activityLogFilter.pageIndex || 1,
    totalCount: 0
  });

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

  const fetchData = useCallback(async () => {
    try {
      setLoading(true)
      const data = await dashboardService.getAdminDashboard()
      setMetrics(data)
    } catch (error: any) {
      setNotify({
        type: "error",
        message: "Failed to load dashboard data",
        description:
          error?.response?.data?.error || "Error loading dashboard data",
      });
    } finally {
      setLoading(false)
    }
  }, []);

  const fetchActivityLog = useCallback(async (request: ActivityLogRequest) => {
    try {
      setLogLoading(true);
      console.log(request)
      const response = await activityLogService.getActivityLogs(request);
      setActivityLog(response.items);
      setPagination({
        ...pagination,
        pageIndex: response.pageIndex,
        totalCount: response.totalCount,
      })
    } catch (error: any) {
      setNotify({
        type: "error",
        message: "Failed to load activity log data",
        description:
          error?.response?.data?.error || "Error loading activity log data",
      });
    } finally {
      setLogLoading(false)
    }
  }, [activityLogFilter])

  useEffect(() => {
    fetchData()
  }, [fetchData])

  useEffect(() => {
    fetchActivityLog(activityLogFilter)
  }, [fetchActivityLog])

  const handleFilterChange = (filter: ActivityLogRequest) => {
    setActivityLogFilter(filter)
    setPagination((prev) => ({
      ...prev,
      pageIndex: filter.pageIndex || 1,
    }))
  }

  if (loading) {
    return <Loading />
  }

  return (
    <div className="bg-gray-800 rounded-lg overflow-hidden shadow-lg p-6">
      {/* Header */}
      <div className="mb-8">
        <div className="flex items-center gap-4 mb-4">
          <div className="bg-gradient-to-r from-blue-500 to-purple-600 p-3 rounded-xl">
            <TrophyOutlined className="text-white text-xl" />
          </div>
          <div>
            <h2 className="text-2xl font-semibold">Admin Dashboard</h2>
            <p className="text-slate-300 text-sm">
              Platform metrics and statistics overview
            </p>
          </div>
        </div>
      </div>

      {/* Main Stats Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 mb-4">
        {/* Total Users Card */}
        <Card className="bg-slate-600/50 border-slate-500/30 backdrop-blur-sm hover:bg-slate-600/70 transition-all duration-300 hover:scale-105 hover:shadow-xl">
          <div className="flex items-center justify-between mb-4">
            <div className="bg-gradient-to-r from-blue-500 to-blue-600 p-3 rounded-xl">
              <UserOutlined className="text-white text-xl" />
            </div>
            <div className="text-right">
              <div className="text-2xl font-bold text-white">{metrics.totalUsers.toLocaleString()}</div>
              <Text className="text-slate-300">Total Users</Text>
            </div>
          </div>
          <div className="space-y-2">
            <div className="flex justify-between items-center">
              <span className="text-slate-400 text-sm">Mentors</span>
              <span className="text-blue-300 font-medium">{metrics.totalMentors}</span>
            </div>
            <div className="flex justify-between items-center">
              <span className="text-slate-400 text-sm">Learners</span>
              <span className="text-green-300 font-medium">{metrics.totalLearners}</span>
            </div>
          </div>
        </Card>

        {/* Sessions This Week */}
        <Card className="bg-slate-600/50 border-slate-500/30 backdrop-blur-sm hover:bg-slate-600/70 transition-all duration-300 hover:scale-105 hover:shadow-xl">
          <div className="flex items-center justify-between mb-4">
            <div className="bg-gradient-to-r from-green-500 to-green-600 p-3 rounded-xl">
              <BookOutlined className="text-white text-xl" />
            </div>
            <div className="text-right">
              <div className="text-2xl font-bold text-white">{metrics.sessionsThisWeek}</div>
              <Text className="text-slate-300">Sessions This Week</Text>
            </div>
          </div>
          <div className="flex items-center gap-2">
            <CalendarOutlined className="text-slate-400" />
            <span className="text-slate-400 text-sm">Avg. 22 sessions/day</span>
          </div>
        </Card>

        {/* Resources */}
        <Card className="bg-slate-600/50 border-slate-500/30 backdrop-blur-sm hover:bg-slate-600/70 transition-all duration-300 hover:scale-105 hover:shadow-xl">
          <div className="flex items-center justify-between mb-4">
            <div className="bg-gradient-to-r from-purple-500 to-purple-600 p-3 rounded-xl">
              <FileOutlined className="text-white text-xl" />
            </div>
            <div className="text-right">
              <div className="text-2xl font-bold text-white">{metrics.totalResources}</div>
              <Text className="text-slate-300">Total Resources</Text>
            </div>
          </div>
          <div className="space-y-1">
            <div className="flex justify-between text-sm">
              <span className="text-slate-400">Videos</span>
              <span className="text-purple-300">156</span>
            </div>
            <div className="flex justify-between text-sm">
              <span className="text-slate-400">Documents</span>
              <span className="text-purple-300">186</span>
            </div>
          </div>
        </Card>

        {/* Pending Applications */}
        <Card className="bg-slate-600/50 border-slate-500/30 backdrop-blur-sm hover:bg-slate-600/70 transition-all duration-300 hover:scale-105 hover:shadow-xl">
          <div className="flex items-center justify-between mb-4">
            <div className="bg-gradient-to-r from-orange-500 to-orange-600 p-3 rounded-xl">
              <CheckSquareOutlined className="text-white text-xl" />
            </div>
            <div className="text-right">
              <div className="text-2xl font-bold text-white">{metrics.pendingApplications}</div>
              <Text className="text-slate-300">Pending Approvals</Text>
            </div>
          </div>
          <Button
            type="primary"
            className="w-full bg-orange-500 hover:bg-orange-600 border-orange-500 flex items-center justify-center gap-2"
            icon={<ArrowRightOutlined />}
          >
            Review Applications
          </Button>
        </Card>
      </div>

      {/* Additional Info Cards */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-4">
        <ActivityLogTable
          items={activityLog}
          loading={logLoading}
          filter={activityLogFilter}
          pagination={pagination}
          onFilter={handleFilterChange || (() => { })}
        />
        <QuickActions />
      </div>
    </div>
  )
}
