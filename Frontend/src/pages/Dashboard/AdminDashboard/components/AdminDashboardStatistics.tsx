import { Button, Card, Typography } from "antd"
import {
  UserOutlined,
  BookOutlined,
  FileOutlined,
  CalendarOutlined,
  CheckSquareOutlined,
  ArrowRightOutlined,
} from "@ant-design/icons"
import { useNavigate } from "react-router-dom"
import type { DashboardMetrics } from "../../../../types/DashboardType"
import { getFileTypeString } from "../../../../types/enums/FileType"

const { Text } = Typography

interface AdminDashboardStatisticProps {
  metrics?: DashboardMetrics;
}

export default function AdminDashboardStatistics({ metrics }: AdminDashboardStatisticProps) {
  const navigate = useNavigate();

  return (
    <>
      {/* Total Users Card */}
      <Card className="bg-slate-600/50 border-slate-500/30 backdrop-blur-sm hover:bg-slate-600/70 transition-all duration-300 hover:scale-105 hover:shadow-xl">
        <div className="flex items-center justify-between mb-4">
          <div className="bg-gradient-to-r from-blue-500 to-blue-600 p-3 rounded-xl">
            <UserOutlined className="text-white text-xl" />
          </div>
          <div className="text-right">
            <div className="text-2xl font-bold text-white">{metrics?.totalUsers.toLocaleString()}</div>
            <Text className="text-slate-300">Total Users</Text>
          </div>
        </div>
        <div className="space-y-2">
          <div className="flex justify-between items-center">
            <span className="text-slate-400 text-sm">Mentors</span>
            <span className="text-blue-300 font-medium">{metrics?.totalMentors}</span>
          </div>
          <div className="flex justify-between items-center">
            <span className="text-slate-400 text-sm">Learners</span>
            <span className="text-green-300 font-medium">{metrics?.totalLearners}</span>
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
            <div className="text-2xl font-bold text-white">{metrics?.sessionsThisWeek}</div>
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
            <div className="text-2xl font-bold text-white">{metrics?.totalResources}</div>
            <Text className="text-slate-300">Total Resources</Text>
          </div>
        </div>
        <div className="space-y-2">
          {metrics?.resourceTypeCounts.map((resource, index) => (
            <div key={index} className="flex justify-between text-sm">
              <span className="text-slate-400">{getFileTypeString(resource.resourceType)}</span>
              <span className="text-purple-300">{resource.count}</span>
            </div>
          ))}
        </div>
      </Card>

      {/* Pending Applications */}
      <Card className="bg-slate-600/50 border-slate-500/30 backdrop-blur-sm hover:bg-slate-600/70 transition-all duration-300 hover:scale-105 hover:shadow-xl">
        <div className="flex items-center justify-between mb-4">
          <div className="bg-gradient-to-r from-orange-500 to-orange-600 p-3 rounded-xl">
            <CheckSquareOutlined className="text-white text-xl" />
          </div>
          <div className="text-right">
            <div className="text-2xl font-bold text-white">{metrics?.pendingApplications}</div>
            <Text className="text-slate-300">Pending Approvals</Text>
          </div>
        </div>
        <Button
          type="primary"
          className="w-full bg-orange-500 hover:bg-orange-600 border-orange-500 flex items-center justify-center gap-2"
          icon={<ArrowRightOutlined />}
          onClick={() => navigate('/applications')}
        >
          Review Applications
        </Button>
      </Card>
    </>
  )
}