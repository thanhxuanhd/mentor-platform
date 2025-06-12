import { Button, Card } from "antd";
import {
  TrophyOutlined,
  ArrowRightOutlined,
  CalendarOutlined,
  ProfileOutlined,
} from "@ant-design/icons"
import { useNavigate } from "react-router-dom";

export default function QuickActions() {
  const navigate = useNavigate();

  return (
    <Card
      title={
        <span className="text-white text-lg font-semibold flex items-center gap-2">
          <TrophyOutlined className="text-purple-400" />
          Quick Actions
        </span>
      }
      className="border-slate-500/30 backdrop-blur-sm lg:col-span-1"
    >
      <div className="grid grid-cols-1 gap-4">
        <Button
          size="large"
          className="bg-blue-500 hover:bg-blue-600 text-white border-blue-500 h-12 flex items-center justify-between"
          onClick={() => navigate('/sessions')}
        >
          <span className="flex items-center gap-2">
            <CalendarOutlined />
            Manage Sessions
          </span>
          <ArrowRightOutlined />
        </Button>
        <Button
          size="large"
          className="bg-green-500 hover:bg-green-600 text-white border-green-500 h-12 flex items-center justify-between"
          onClick={() => navigate('/profile')}
        >
          <span className="flex items-center gap-2">
            <ProfileOutlined />
            View Profile
          </span>
          <ArrowRightOutlined />
        </Button>
      </div>
    </Card>
  )
}