import { VideoCameraOutlined, UserOutlined, HomeOutlined } from "@ant-design/icons"

export const getSessionTypeIcon = (type: "Virtual" | "OneOnOne" | "Onsite") => {
  switch (type) {
    case "Virtual":
      return <VideoCameraOutlined className="text-blue-400" />
    case "OneOnOne":
      return <UserOutlined className="text-green-400" />
    case "Onsite":
      return <HomeOutlined className="text-orange-400" />
    default:
      return <VideoCameraOutlined className="text-blue-400" />
  }
}

export const getSessionTypeText = (type: "Virtual" | "OneOnOne" | "Onsite") => {
  switch (type) {
    case "Virtual":
      return "Virtual"
    case "OneOnOne":
      return "One-on-One"
    case "Onsite":
      return "On-site"
    default:
      return "Virtual"
  }
}

export const parseTimeRange = (timeRange: string): { time: string; duration: string } => {
  // Assuming timeRange format is like "09:00-10:00" or "09:00 - 10:00"
  const cleanTimeRange = timeRange.replace(/\s/g, "")
  const [startTime, endTime] = cleanTimeRange.split("-")

  if (!startTime || !endTime) {
    return { time: timeRange, duration: "1 hour" }
  }

  // Calculate duration
  const start = new Date(`2000-01-01T${startTime}:00`)
  const end = new Date(`2000-01-01T${endTime}:00`)
  const diffMs = end.getTime() - start.getTime()
  const diffHours = diffMs / (1000 * 60 * 60)

  const duration = diffHours === 1 ? "1 hour" : `${diffHours} hours`
  const time = `${startTime} - ${endTime}`

  return { time, duration }
}
