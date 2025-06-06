namespace Domain.Constants
{
    public class FileConstants
    {
        public const long MAX_FILE_SIZE = 1 * 1024 * 1024;
        public static readonly string[] IMAGE_CONTENT_TYPES = { "image/jpeg", "image/png" };
        public static readonly string[] DOCUMENT_CONTENT_TYPES = {
            // PDF
            "application/pdf",
            // Images
            "image/jpeg", "image/png",
            // Video
            "video/mp4", "video/x-msvideo", "video/mpeg",
            // Audio
            "audio/mpeg", "audio/wav", "audio/aac"
        };
    }
}
