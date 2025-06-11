import type { SessionStatus } from "./enums/SessionStatus"
import type { SessionType } from "./enums/SessionType"

export interface Mentor {
  id: string;
  name: string;
  expertise: string[];
  availability: string;
  avatar: string;
}

export interface BookedSession {
  id: string
  mentor: Mentor
  date: string
  type: SessionType
  status: SessionStatus
  startTime: string
  endTime: string
}

export interface TimeSlot {
  id: string;
  startTime: string;
  endTime: string;
  status?: SessionStatus;
}