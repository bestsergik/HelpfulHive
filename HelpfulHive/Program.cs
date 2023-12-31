using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using HelpfulHive;
using HelpfulHive.ViewModels;
using HelpfulHive.Services;
using Microsoft.AspNetCore.Components;
using HelpfulHive.Models;
using Microsoft.AspNetCore.Components.Forms;
using HelpfulHive.Areas.Identity.Pages.Account.Manage;
using Microsoft.Extensions.FileProviders;


var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddRazorPages();
// builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<ImageService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
//    .AddRoles<IdentityRole>() // ��������� ��������� �����
//    .AddEntityFrameworkStores<ApplicationDbContext>();



builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;

    // ��������� ������
    options.Password.RequireDigit = false; // �� ��������� �����
    options.Password.RequiredLength = 6; // ����������� ����� ������
    options.Password.RequireNonAlphanumeric = false; // �� ��������� ������������ �������
    options.Password.RequireUppercase = false; // �� ��������� ��������� �����
    options.Password.RequireLowercase = false; // �� ��������� �������� �����
})
.AddRoles<IdentityRole>() // ��������� ��������� �����
.AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddServerSideBlazor().AddHubOptions(o =>
{
    o.ClientTimeoutInterval = TimeSpan.FromMinutes(2); // ������������� ����� �������� �������
    o.KeepAliveInterval = TimeSpan.FromSeconds(30);    // ������������� �������� Keep-Alive
});

// � Program.cs ��� Startup.cs
builder.Services.AddScoped<IApplicationUserAdapter, ApplicationUserAdapter>();





builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<AnimationService>();
builder.Services.AddSingleton<RecordUpdateService>();
builder.Services.AddTransient<UserPreferencesService>();
builder.Services.AddTransient<UserPreferencesViewModel>();
builder.Services.AddScoped<TabViewModel>();
builder.Services.AddScoped<TabService>();
builder.Services.AddScoped<RecordService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RecordViewModel>();
builder.Services.AddSingleton<LuceneSearchService>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.LogoutPath = "/Identity/Account/Logout";
});

builder.Services.AddSignalR(hubOptions => {
    // ��������� ��������� ����������� ��������� ����������
   // hubOptions.KeepAliveInterval = TimeSpan.FromMinutes(2);

    // ���������� ������������� ������� ������������ ��������� �� 10 ��
    hubOptions.MaximumReceiveMessageSize = 10 * 1024 * 1024; // 10MB

    // ��������� ��������� ������ ��� ��������� �������� ������� (� ���������� ������� ���������)
    hubOptions.EnableDetailedErrors = true;

    // ��������� ����-���� ��� �������
   // hubOptions.ClientTimeoutInterval = TimeSpan.FromMinutes(5); 

    // ��������� ����-���� ��� �����������
    hubOptions.HandshakeTimeout = TimeSpan.FromSeconds(230);
});

var app = builder.Build();


//using (var scope = app.Services.CreateScope())
//{
//    var luceneService = scope.ServiceProvider.GetRequiredService<LuceneSearchService>();
//    await luceneService.InitializeIndex();
//}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    //app.UseDatabaseErrorPage(); // Enable database error page in development
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages(); // Map the Razor pages
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await RoleInitializer.InitializeAsync(userManager, roleManager);
}




app.Run();
