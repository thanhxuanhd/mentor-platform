using Domain.Enums;
using Microsoft.AspNetCore.StaticFiles;

namespace Application.Helpers
{
    public static class FileHelper
    {
        public static FileType GetFileTypeFromUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("URL cannot be null or empty", nameof(url));
            }

            if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri))
            {
                throw new InvalidOperationException("Invalid document URL format.");
            }

            var path = uri.IsAbsoluteUri ? uri.LocalPath : url;
            var fileName = Path.GetFileName(path);

            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            return extension switch
            {
                ".pdf" => FileType.Pdf,
                ".jpg" or ".jpeg" or ".png" => FileType.Image,
                ".mp4" or ".avi" or ".mpeg" => FileType.Video,
                ".mp3" or ".wav" or ".aac" => FileType.Audio,
                _ => throw new ArgumentException($"Unsupported file extension: {extension}", nameof(url))
            };
        }

        public static string GetFileContentTypeFromExtension(string fileExtension)
        {
            // Ensure the extension starts with a dot
            if (!fileExtension.StartsWith("."))
            {
                fileExtension = "." + fileExtension;
            }

            var provider = new FileExtensionContentTypeProvider();

            if (provider.TryGetContentType(fileExtension, out string mimeType))
            {
                return mimeType;
            }

            return "application/octet-stream"; // default for unknown types
        }
    }
}
