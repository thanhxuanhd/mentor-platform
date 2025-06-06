import { Card, Typography } from "antd"
import {
  BookOutlined,
  CalendarOutlined,
  TeamOutlined,
  VideoCameraOutlined,
} from "@ant-design/icons"
import type { MentorDashboardResponse } from "../../../../services/mentor/mentorDashboardService";

const { Text } = Typography

interface MentorDashboardStatisticProps {
  metrics?: MentorDashboardResponse;
}

export default function MentorDashboardStatistics({ metrics }: MentorDashboardStatisticProps) {
  return (
    <>
      {/* Total Learners Card */}
      <Card className="bg-slate-600/50 border-slate-500/30 backdrop-blur-sm hover:bg-slate-600/70 transition-all duration-300 hover:scale-105 hover:shadow-xl" id="total-users">
        <div className="flex items-center justify-between mb-4">
          <div className="bg-gradient-to-r from-blue-500 to-blue-600 p-3 rounded-xl">
            <TeamOutlined className="text-white text-xl" />
          </div>
          <div className="text-right">
            <div className="text-2xl font-bold text-white">{metrics?.totalLearners ?? 0}</div>
            <Text className="text-slate-300">Total Learners</Text>
          </div>
        </div>
      </Card>

      {/* Upcoming Sessions */}
      <Card className="bg-slate-600/50 border-slate-500/30 backdrop-blur-sm hover:bg-slate-600/70 transition-all duration-300 hover:scale-105 hover:shadow-xl" id="sessions">
        <div className="flex items-center justify-between mb-4" id="3">
          <div className="bg-gradient-to-r from-green-500 to-green-600 p-3 rounded-xl">
            <CalendarOutlined className="text-white text-xl" />
          </div>
          <div className="text-right">
            <div className="text-2xl font-bold text-white">{metrics?.upcomingSessions ?? 0}</div>
            <Text className="text-slate-300">Pending Sessions</Text>
          </div>
        </div>
      </Card>

      {/* Completed Sessions */}
      <Card className="bg-slate-600/50 border-slate-500/30 backdrop-blur-sm hover:bg-slate-600/70 transition-all duration-300 hover:scale-105 hover:shadow-xl" id="total-resources">
        <div className="flex items-center justify-between mb-4">
          <div className="bg-gradient-to-r from-purple-500 to-purple-600 p-3 rounded-xl">
            <VideoCameraOutlined className="text-white text-xl" />
          </div>
          <div className="text-right">
            <div className="text-2xl font-bold text-white">{metrics?.completedSessions ?? 0}</div>
            <Text className="text-slate-300">Completed Sessions</Text>
          </div>
        </div>
      </Card>

      {/* Total Courses */}
      <Card className="bg-slate-600/50 border-slate-500/30 backdrop-blur-sm hover:bg-slate-600/70 transition-all duration-300 hover:scale-105 hover:shadow-xl" id="pending-approval">
        <div className="flex items-center justify-between mb-4">
          <div className="bg-gradient-to-r from-orange-500 to-orange-600 p-3 rounded-xl">
            <BookOutlined className="text-white text-xl" />
          </div>
          <div className="text-right">
            <div className="text-2xl font-bold text-white">{metrics?.totalCourses ?? 0}</div>
            <Text className="text-slate-300">Total Courses</Text>
          </div>
        </div>
      </Card>
    </>
  )
}