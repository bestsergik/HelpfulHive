namespace HelpfulHive.Services
{
    public class ImageService
    {

        private readonly IWebHostEnvironment _env;

        public ImageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveImageAsync(string base64Image, String imagePath)
        {

            if (string.IsNullOrEmpty(base64Image))
                throw new ArgumentNullException(nameof(base64Image));

            byte[] bytes;
            if (base64Image.Contains(","))
            {
                // Разделение строки на части
                var parts = base64Image.Split(',');
                if (parts.Length != 2)
                {
                    throw new InvalidOperationException("Invalid base64 image string.");
                }

                try
                {
                    bytes = Convert.FromBase64String(parts[1]);
                }
                catch (FormatException ex)
                {
                    throw new InvalidOperationException("Provided string is not a valid Base64 string.", ex);
                }
            }
            else
            {
                try
                {
                    bytes = Convert.FromBase64String(base64Image);
                }
                catch (FormatException ex)
                {
                    throw new InvalidOperationException("Provided string is not a valid Base64 string.", ex);
                }
            }

            var fileName = Guid.NewGuid().ToString() + ".png";

            var filePath = Path.Combine(_env.WebRootPath, imagePath, fileName);
            var relativePath = Path.Combine(imagePath, fileName).Replace("\\", "/");

            if (!Directory.Exists(Path.Combine(_env.WebRootPath, imagePath)))
            {
                Directory.CreateDirectory(Path.Combine(_env.WebRootPath, imagePath));
            }

            await File.WriteAllBytesAsync(filePath, bytes);

            return relativePath;
        }



        public Dictionary<string, List<string>> GetImagesByFolders()
        {
            var imagesFolder = "wwwroot/images";
            var subfolders = new[] { "1", "2", "3", "4", "5" };

            var imagesByFolders = new Dictionary<string, List<string>>();

            foreach (var subfolder in subfolders)
            {
                var folderPath = Path.Combine(imagesFolder, subfolder);
                var folderImages = Directory.GetFiles(folderPath)
                                            .Select(Path.GetFileName)
                                            .ToList();

                imagesByFolders[subfolder] = folderImages;
            }

            return imagesByFolders;
        }


        public List<string> GetProfileImages()
        {
            var profileImagesFolder = Path.Combine("wwwroot", "profileimg");
            if (!Directory.Exists(profileImagesFolder))
            {
                return new List<string>();
            }

            var profileImages = Directory.GetFiles(profileImagesFolder)
                                         .Select(Path.GetFileName)
                                         .ToList();

            return profileImages;
        }


    }



}
