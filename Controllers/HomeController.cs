using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using iTravel.Models;

namespace iTravel.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private MyContext DATABASE;

    public HomeController(ILogger<HomeController> logger, MyContext context)
    {
        _logger = logger;
        DATABASE = context;
    }

    public IActionResult Index()
    {
        // List<User> users = DATABASE.Users.ToList();
        // ViewBag.Users = users;
        // Console.WriteLine(users.Count());
        // foreach (User user in users)
        // {
        //     Console.WriteLine(user.Username);
        // }
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
