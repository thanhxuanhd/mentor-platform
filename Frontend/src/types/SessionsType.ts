import type { SessionStatus } from "./enums/SessionStatus"
import type { SessionType } from "./enums/SessionType"

export interface Mentor {
  id: string
  name: string
  expertise: string[]
  availability: string
  avatar: string
}

export interface BookedSession {
  id: string
  mentor: Mentor
  date: string // This will be the local date
  type: SessionType
  status: SessionStatus
  startTime: string // This will be the local time
  endTime: string // This will be the local time
  // Store original UTC values for backend operations
  originalDate: string
  originalStartTime: string
  originalEndTime: string
  sortableDateTime?: string
}

export interface TimeSlot {
  id: string
  startTime: string // This will be the local time
  endTime: string // This will be the local time
  status?: SessionStatus
  // Store original UTC values
  originalDate?: string
  originalStartTime?: string
  originalEndTime?: string
}
