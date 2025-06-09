export const SessionType = {
  Virtual: "Virtual" as const,
  OneOnOne: "OneOnOne" as const,
  Onsite: "Onsite" as const,
} as const;

export type SessionTypeValue = (typeof SessionType)[keyof typeof SessionType];