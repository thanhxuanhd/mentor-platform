export const getFileNameFromUrl = (url: string): string => {
  const parts = url.split("/");
  return parts[parts.length - 1];
};
export const downloadFile = (url: string, filename: string) => {
  fetch(url, {
    method: "GET",
    headers: {
      "Content-Type": "application/octet-stream",
    },
  })
    .then((response) => response.blob())
    .then((blob) => {
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement("a");
      a.href = url;
      a.download = filename || "download";
      document.body.appendChild(a);
      a.click();
      window.URL.revokeObjectURL(url);
      document.body.removeChild(a);
    })
    .catch((error) => console.error("Error downloading file:", error));
};

export const getBinaryFileFromUrl = async (
  url: string,
): Promise<File | null> => {
  try {
    const response = await fetch(url, {
      method: "GET",
      headers: {
        "Content-Type": "application/octet-stream",
      },
    });
    const blob = await response.blob();
    const contentType = response.headers.get("Content-Type") || "";
    const fileName = getFileNameFromUrl(url);
    return new File([blob], fileName, { type: contentType });
  } catch (error) {
    console.error("Error getting binary file from URL:", error);
    return null;
  }
};
