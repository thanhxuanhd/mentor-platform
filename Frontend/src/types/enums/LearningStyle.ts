export const learningStyleDisplayMap: { [key: number]: string } = {
  0: "Visual",
  1: "Auditory",
  2: "Reading/Writing",
  3: "Kinesthetic",
};

export const learningStyleValues = [0, 1, 2, 3];

export const LearningStyle = {
  Visual: 0,
  Auditory: 1,
  ReadingWriting: 2,
  Kinesthetic: 3,
} as const;

export type LearningStyle = (typeof LearningStyle)[keyof typeof LearningStyle];
