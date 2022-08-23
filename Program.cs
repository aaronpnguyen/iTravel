using Microsoft.EntityFrameworkCore;
using iTravel.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddControllersWithViews();
builder.Services.AddSession();

builder.Services.AddDbContext<MyContext>(options => {
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();

/*
    dotnet add package Pomelo.EntityFrameworkCore.MySql --version 6.0.1
    dotnet add package Microsoft.EntityFrameworkCore.Design --version 6.0.3
*/