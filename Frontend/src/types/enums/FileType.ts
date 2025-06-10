export const FileType = {
  Pdf: "Pdf",
  Video: "Video",
  Audio: "Audio",
  Image: "Image",
} as const;

export type FileType = (typeof FileType)[keyof typeof FileType];
