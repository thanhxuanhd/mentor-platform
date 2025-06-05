export const FileType = {
  Pdf: "Pdf",
  Video: "Video",
  Audio: "Audio",
  Image: "Image",
  ExternalWebAddress: "External Web Address",
} as const;

export type FileType = (typeof FileType)[keyof typeof FileType];

export const getFileTypeString = (
  fileTypeKey: keyof typeof FileType,
): string => {
  return FileType[fileTypeKey];
};
