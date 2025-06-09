export interface SessionBookingRequest {
  mentorId: string | undefined
  id: string
  timeSlotId: string
  learnerId: string
  status: number | "Pending" | "Approved" | "Completed" | "Canceled" | "Rescheduled"
  type: number
  startTime: string
  endTime: string
  date: string
  fullNameLearner: string
  preferredCommunicationMethod: "VideoCall" | "AudioCall" | "Chat"
  lastStatusUpdate: string
}

export interface UpdateSessionStatusRequest {
  id: string
  status: number 
}

export interface SessionBookingResponse {
  value: SessionBookingRequest[]
  isSuccess: boolean
  message?: string
}

export const getStatusString = (
  numericStatus: number,
): "Pending" | "Approved" | "Completed" | "Canceled" | "Rescheduled" => {
  const statusMap: { [key: number]: "Pending" | "Approved" | "Completed" | "Canceled" | "Rescheduled" } = {
    0: "Pending",
    1: "Approved",
    2: "Completed",
    3: "Canceled",
    4: "Rescheduled",
  }
  return statusMap[numericStatus] || "Pending"
}

export const getStatusNumber = (stringStatus: string): number => {
  const statusMap: { [key: string]: number } = {
    Pending: 0,
    Approved: 1,
    Completed: 2,
    Canceled: 3,
    Rescheduled: 4,
  }
  return statusMap[stringStatus] ?? 0
}
