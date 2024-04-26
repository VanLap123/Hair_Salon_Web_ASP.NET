
using Hair_Salon_Web_ASP.NET.Models;
using Hair_Salon_Web_ASP.NET.ServiceSaveImage;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Hangfire;
using Hangfire.MySql;
using Hangfire.MySql.Core;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var connectionString = builder.Configuration.GetConnectionString("ApDbConnectionString");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
builder.Services.AddDistributedMemoryCache();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(50);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
/*builder.Services.AddHangfire(config => config
    .UseStorage(new MySqlStorage(connectionString))
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
);

builder.Services.AddHangfireServer();*/

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    SeedData.Initialize(services);
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();
//app.UseHangfireDashboard();
//app.UseHangfireServer();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
//RecurringJob.AddOrUpdate<MyJob>("MyJobId", x => x.Execute(), Cron.MinuteInterval(1));
app.Run();
