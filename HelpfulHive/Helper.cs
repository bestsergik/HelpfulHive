using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
namespace HelpfulHive
{
    public class Helper
    {



        // Первичная инициализация

        // dotnet ef migrations add InitialCreate -c HelpfulHive.ApplicationDbContext  

        // добавление миграции
        //     dotnet ef migrations add DeleteiscanedittabsInAspnetusers -c HelpfulHive.ApplicationDbContext  

        // обновление после добавления миграции
        //  dotnet ef database update -c HelpfulHive.ApplicationDbContext

        // полный откат
        // dotnet ef database update 0 --context HelpfulHive.ApplicationDbContexts

        // *************************************************

        // cd C:\Users\user\source\repos\HelpfulHive
        // git add .
        // git commit -m "????"
        // git push origin development

        // Глобальный скрипт для переноса
        // dotnet ef migrations script -c HelpfulHive.ApplicationDbContext 

    }
}
