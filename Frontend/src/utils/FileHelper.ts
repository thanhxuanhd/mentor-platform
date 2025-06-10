import { axiosClient } from "../services/apiClient";

export const getFileNameFromUrl = (url: string): string => {
  const parts = url.split("/");
  console.log(parts);
  return parts[parts.length - 1];
};
export const downloadFile = async (
  courseResourceId: string,
  fileName: string,
) => {
  try {
    const response = await axiosClient.get(`Resources/download`, {
      params: { courseResourceId, fileName },
      responseType: "blob", // This is crucial for file downloads
    });

    // Create a download link and trigger it
    const url = window.URL.createObjectURL(new Blob([response.data]));
    const link = document.createElement("a");
    link.href = url;
    link.setAttribute("download", fileName); // Set the filename
    document.body.appendChild(link);
    link.click();

    // Clean up
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
  } catch (error) {
    console.error("Download failed:", error);
    throw error;
  }
};

export const getBinaryFileFromUrl = async (
  courseResourceId: string,
  fileName: string,
): Promise<File> => {
  // Removed `| null` since we're throwing errors
  try {
    const response = await axiosClient.get(`Resources/download`, {
      params: { courseResourceId, fileName },
      responseType: "blob", // Important for binary data
    });

    // Get content type from headers (fallback to empty string)
    const contentType = response.headers["content-type"] || "";

    // Create and return a File object
    return new File([response.data], fileName, { type: contentType });
  } catch (error) {
    console.error("Failed to fetch file:", error);
    throw error; // Re-throw to let the caller handle it
  }
};
