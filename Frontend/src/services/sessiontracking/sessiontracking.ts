import { axiosClient } from "../apiClient"
import type { SessionBookingRequest } from "../../types/SessionBookingTypes"

export const sessionBookingService = {
  getSessionBookings: async (): Promise<SessionBookingRequest[]> => {
    return await axiosClient
      .get(`SessionBooking/request/get`)
      .then((response) => {
        return response.data.value || response.data
      })
      .catch((error) => {
        console.error("Error fetching session bookings:", error)
        throw error
      })
  },
  
  getSessionDetails: async (sessionId: string): Promise<SessionBookingRequest> => {
    return await axiosClient
      .get(`SessionBooking/request/get/${sessionId}`)
      .then((response) => {
        return response.data.value || response.data
      })
      .catch((error) => {
        console.error("Error fetching session details:", error)
        throw error
      })
  },

  updateSessionStatus: async (id: string, status: string): Promise<void> => {
    const statusMap: { [key: string]: number } = {
      Pending: 0,
      Approved: 1,
      Completed: 2,
      Cancelled: 3,
      Rescheduled: 4,
    }

    const numericStatus = statusMap[status]
    if (numericStatus === undefined) {
      throw new Error(`Invalid status: ${status}`)
    }

    return await axiosClient
      .put(`SessionBooking/${id}`, {
        status: numericStatus,
      })
      .then((response) => {
        return response.data.value
      })
      .catch((error) => {
        console.error("Error updating session status:", error)
        throw error
      })
  },

  updateSessionBooking: async (
    id: string,
    updateData: Partial<SessionBookingRequest>,
  ): Promise<SessionBookingRequest> => {
    const statusMap: { [key: string]: number } = {
      Pending: 0,
      Approved: 1,
      Completed: 2,
      Cancelled: 3,
      Rescheduled: 4,
    }

    const requestData = { ...updateData }
    if (requestData.status && typeof requestData.status === "string") {
      const numericStatus = statusMap[requestData.status]
      if (numericStatus !== undefined) {
        requestData.status = numericStatus as any
      }
    }

    return await axiosClient
      .put(`SessionBooking/${id}`, requestData)
      .then((response) => {
        return response.data.value
      })
      .catch((error) => {
        console.error("Error updating session booking:", error)
        throw error
      })
  },

  rescheduleSession: async (
    id: string,
    rescheduleData: {
      date: string
      startTime: string
      endTime: string
      reason: string
    },
  ): Promise<void> => {
    return await axiosClient
      .put(`SessionBooking/update/${id}`, rescheduleData)
      .then((response) => {
        return response.data.value
      })
      .catch((error) => {
        console.error("Error rescheduling session:", error)
        throw error
      })
  },
}
