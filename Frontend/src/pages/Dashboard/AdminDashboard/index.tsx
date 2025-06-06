import { useState, useEffect, useCallback } from "react"
import { App } from "antd"
import { TrophyOutlined } from "@ant-design/icons"
import { dashboardService } from "../../../services/adminDashboard/adminDashboardService"
import Loading from "../../../components/Loading"
import type { NotificationProps } from "../../../types/Notification"
import ActivityLogTable from "./components/ActivityLogs"
import type { ActivityLogRequest, ActivityLogResponse } from "../../../types/ActivityLogType"
import { getCurrentWeekDates } from "../../../utils/CurrentDate"
import { activityLogService } from "../../../services/adminDashboard/activityLogService"
import QuickActions from "./components/QuickActions"
import type { DashboardMetrics } from "../../../types/DashboardType"
import AdminDashboardStatistics from "./components/AdminDashboardStatistics"
import ReportActions from "./components/ReportActions"

const defaultLogFilter: ActivityLogRequest = {
  startDateTime: getCurrentWeekDates().startDate,
  endDateTime: getCurrentWeekDates().endDate,
  keyword: '',
  pageSize: 5,
  pageIndex: 1,
}

export default function AdminDashboard() {
  const [metrics, setMetrics] = useState<DashboardMetrics | undefined>()
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
      setLoading(true);
      const response = await dashboardService.getAdminDashboard();
      setMetrics(response)
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
    console.log(activityLogFilter)
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
        <AdminDashboardStatistics metrics={metrics} />
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
        <div className="flex flex-col gap-4">
          <QuickActions />
          <ReportActions />
        </div>
      </div>
    </div>
  )
}
