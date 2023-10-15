namespace HelpfulHive.Services
{
    public class ImageService
    {
        public List<string> GetImageNames()
        {
            var imagesFolder = "wwwroot/images";
            var images = Directory.GetFiles(imagesFolder)
                                  .Select(Path.GetFileName)
                                  .ToList();
            return images;
        }
    }

}
