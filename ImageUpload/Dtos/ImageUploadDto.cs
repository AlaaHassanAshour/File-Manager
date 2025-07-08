namespace ImageUpload.Dtos
{
    public class ImageUploadDto
    {
        public required IFormFile file { get; set; }

        public required string path { get; set; }

        public required string customFileName { get; set; }
    }
}
