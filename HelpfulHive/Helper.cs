using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
namespace HelpfulHive
{
    public class Helper
    {

        // добавление миграции
        //     dotnet ef migrations add AddTabs -c HelpfulHive.ApplicationDbContext  

        // обновление после добавления миграции
        //  dotnet ef database update -c HelpfulHive.ApplicationDbContext


        // полный откат
        // dotnet ef database update 0 --context HelpfulHive.ApplicationDbContext

        // *************************************************
    }
}
