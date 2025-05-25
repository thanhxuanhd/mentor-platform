export const CommunicationMethod = {
  VideoCall: 0,
  AudioCall: 1,
  TextChat: 2,
} as const;

export type CommunicationMethod =
  (typeof CommunicationMethod)[keyof typeof CommunicationMethod];
