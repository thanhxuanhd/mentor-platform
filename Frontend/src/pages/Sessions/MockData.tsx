import type { Mentor } from "./components/MentorSelectionModal"
import {
  VideoCameraOutlined,
  UserOutlined,
  HomeOutlined,
} from "@ant-design/icons"

export const mentors: Mentor[] = [
  {
    id: "1",
    name: "John Doe, PhD",
    title: "Senior Leadership Coach",
    expertise: "Leadership, Strategy",
    availability: "Available from M-F, 9:00-17:00",
    avatar: "/placeholder.svg?height=60&width=60",
    isOnline: true,
  },
  {
    id: "2",
    name: "Sarah Johnson, MBA",
    title: "Product Strategy Expert",
    expertise: "Product Management, UX Strategy",
    availability: "Available from T-S, 10:00-18:00",
    avatar: "/placeholder.svg?height=60&width=60",
    isOnline: true,
  },
  {
    id: "3",
    name: "Michael Chen, CTO",
    title: "Technology Leadership",
    expertise: "Engineering, Team Building",
    availability: "Available from M-W, 14:00-20:00",
    avatar: "/placeholder.svg?height=60&width=60",
    isOnline: false,
  },
  {
    id: "4",
    name: "Emily Rodriguez, VP",
    title: "Marketing Strategy",
    expertise: "Brand Strategy, Growth Marketing",
    availability: "Available from W-F, 9:00-15:00",
    avatar: "/placeholder.svg?height=60&width=60",
    isOnline: true,
  },
]

export const timeSlots = [
  "9:00 AM",
  "9:30 AM",
  "10:00 AM",
  "10:30 AM",
  "11:00 AM",
  "11:30 AM",
  "12:00 PM",
  "12:30 PM",
  "1:00 PM",
  "1:30 PM",
  "2:00 PM",
  "2:30 PM",
]

export const sessionTypes = [
  {
    id: "virtual",
    title: "Virtual Session",
    icon: <VideoCameraOutlined className="text-2xl text-gray-400" />,
    description: "Online video call",
  },
  {
    id: "in-person",
    title: "In-Person Session",
    icon: <UserOutlined className="text-2xl text-gray-400" />,
    description: "Face-to-face meeting",
  },
  {
    id: "on-site",
    title: "On-Site Session",
    icon: <HomeOutlined className="text-2xl text-gray-400" />,
    description: "At your location",
  },
]
