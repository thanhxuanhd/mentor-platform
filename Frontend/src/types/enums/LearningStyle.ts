export const LearningStyle = {
  Visual: 0,
  Auditory: 1,
  ReadingOrWriting: 2,
  Kinesthetic: 3,
} as const;

export type LearningStyle = (typeof LearningStyle)[keyof typeof LearningStyle];
