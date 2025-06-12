export const getFileNameFromDisposition = (
  contentDisposition?: string,
): string | null => {
  if (!contentDisposition) return null;

  const filenameStarMatch = contentDisposition.match(
    /filename\*\s*=\s*UTF-8''([^;\n]*)/i,
  );
  if (filenameStarMatch && filenameStarMatch[1]) {
    try {
      return decodeURIComponent(filenameStarMatch[1]);
    } catch {
      return filenameStarMatch[1];
    }
  }

  const filenameMatch = contentDisposition.match(/filename\s*=\s*([^;\n]*)/i);
  if (filenameMatch && filenameMatch[1]) {
    return filenameMatch[1].replace(/['"]/g, "").trim();
  }

  return null;
};
export const downloadBlobFile = (
  blob: Blob,
  fileName: string = "downloaded-file",
  mimeType: string = "application/octet-stream",
) => {
  const fileBlob = new Blob([blob], { type: mimeType });
  const url = window.URL.createObjectURL(fileBlob);

  const link = document.createElement("a");
  link.href = url;
  link.download = fileName;

  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
  window.URL.revokeObjectURL(url);
};
