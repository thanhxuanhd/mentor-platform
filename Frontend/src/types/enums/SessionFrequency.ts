export const SessionFrequency = {
  Weekly: 0,
  EveryTwoWeeks: 1,
  Monthly: 2,
  AsNeeded: 3,
} as const;

export type SessionFrequency =
  (typeof SessionFrequency)[keyof typeof SessionFrequency];
