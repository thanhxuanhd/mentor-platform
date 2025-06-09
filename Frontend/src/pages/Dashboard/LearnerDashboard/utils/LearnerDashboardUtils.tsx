import {
  CalendarOutlined,
  VideoCameraOutlined,
  AudioOutlined,
  HomeOutlined,
} from "@ant-design/icons"
import { SessionType, type SessionTypeValue } from "../../../../types/enums/SessionTye"

export const parseTimeRange = (timeRange: string) => {
  const [start, end] = timeRange.split(" - ").map((time) => time.trim())
  const startDate = new Date(`1970-01-01T${start}:00`)
  const endDate = new Date(`1970-01-01T${end}:00`)
  const durationMins = (endDate.getTime() - startDate.getTime()) / (1000 * 60)
  return {
    time: start,
    duration: `${durationMins} mins`,
  }
}

export const getSessionTypeIcon = (type: SessionTypeValue) => {
  switch (type) {
    case SessionType.Virtual:
      return <VideoCameraOutlined className="text-blue-400" />
    case SessionType.OneOnOne:
      return <AudioOutlined className="text-green-400" />
    case SessionType.Onsite:
      return <HomeOutlined className="text-purple-400" />
    default:
      return <CalendarOutlined className="text-gray-400" />
  }
}

export const getSessionTypeText = (type: SessionTypeValue) => {
  switch (type) {
    case SessionType.Virtual:
      return "Virtual Session"
    case SessionType.OneOnOne:
      return "One-on-One Session"
    case SessionType.Onsite:
      return "On-site Session"
    default:
      return "Session"
  }
}