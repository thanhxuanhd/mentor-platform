using Application.Exceptions;
using Domain.Enums;

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
                _ => FileType.ExternalWebAddress
            };
        }

        public static string VerifyFileUrl(string userId, string url)
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

            var segments = uri.Segments;

            string userIdString = segments[2].TrimEnd('/');

            if (!userId.ToString().Equals(userIdString, StringComparison.OrdinalIgnoreCase))
            {
                throw new ForbiddenAccessException("You are not allowed to delete this file.");
            }

            return url;
        }
    }
}
