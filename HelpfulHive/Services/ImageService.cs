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
            Console.WriteLine("Start SaveImageAsync");

            if (string.IsNullOrEmpty(base64Image))
                throw new ArgumentNullException(nameof(base64Image));

            byte[] bytes;
            // Проверяем, содержит ли строка разделитель
            if (base64Image.Contains(","))
            {
                // Разделение строки на части
                var parts = base64Image.Split(',');
                if (parts.Length != 2)
                {
                    Console.WriteLine("Invalid base64 image string. Parts length: " + parts.Length);
                    throw new InvalidOperationException("Invalid base64 image string.");
                }

                // Конвертация base64 строки в массив байтов
                try
                {
                    bytes = Convert.FromBase64String(parts[1]);
                }
                catch (FormatException ex)
                {
                    Console.WriteLine("Provided string is not a valid Base64 string.");
                    throw new InvalidOperationException("Provided string is not a valid Base64 string.", ex);
                }
            }
            else
            {
                // Строка не содержит разделителя, пытаемся конвертировать её целиком
                try
                {
                    bytes = Convert.FromBase64String(base64Image);
                }
                catch (FormatException ex)
                {
                    Console.WriteLine("Provided string is not a valid Base64 string.");
                    throw new InvalidOperationException("Provided string is not a valid Base64 string.", ex);
                }
            }

            // Создание уникального имени файла
            var fileName = Guid.NewGuid().ToString() + ".png";
            Console.WriteLine($"Generated filename: {fileName}");

            // Создание пути к файлу
            var filePath = Path.Combine(_env.WebRootPath, imagePath, fileName);
            Console.WriteLine($"File path: {filePath}");

            // Создание директории, если она не существует
            if (!Directory.Exists(Path.Combine(_env.WebRootPath, imagePath)))
            {
                Directory.CreateDirectory(Path.Combine(_env.WebRootPath, imagePath));
                Console.WriteLine("Directory created");
            }

            // Запись байтов в файл
            await File.WriteAllBytesAsync(filePath, bytes);
            Console.WriteLine("Image saved");

            Console.WriteLine("End SaveImageAsync");
            return filePath;
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
    }



}
