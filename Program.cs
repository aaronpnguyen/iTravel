using Microsoft.EntityFrameworkCore;
using iTravel.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddControllersWithViews();
builder.Services.AddSession();
// Google Authenticator Service?
// builder.Services.AddAuthentication(options => {
//     options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;})
//     .AddCookie(options => {
//         options.LoginPath = "/Account/google-login";
//     })
//     .AddGoogle(options => {
//         options.ClientId = "792338080611-7lvv6vgvcsu76dka7v0g9ecjs70oasah.apps.googleusercontent.com";
//         options.ClientSecret = "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";
//     });

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
// app.UseAuthentication(); // For Google Authentication
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