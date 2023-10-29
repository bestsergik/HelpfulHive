namespace HelpfulHive.Services
{
    public class ImageService
    {
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
    }



}
