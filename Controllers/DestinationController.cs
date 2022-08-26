using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using iTravel.Models;

namespace iTravel.Controllers;

public class DestinationController: Controller
{
    private MyContext DATABASE;

    private IWebHostEnvironment WEBHOST;

    private int? id
    {
        get
        {
            return HttpContext.Session.GetInt32("id");
        }
    }

    private bool notLogged
    {
        get
        {
            return id == null;
        }
    }

    public DestinationController(MyContext context, IWebHostEnvironment _webHost)
    {
        DATABASE = context;
        WEBHOST = _webHost;
    }

    [HttpGet("/create/destination/form")]
    public IActionResult DestinationForm()
    {
        if (notLogged) return RedirectToAction("LogReg", "User");
        return View("DestinationForm");
    }

    [HttpPost]
    public IActionResult SubmitDestination(IFormFile file, Destination newDestination)
    {
        if (notLogged) return RedirectToAction("LogReg", "User");
        
        if (newDestination.City == null || newDestination.State == null || newDestination.Country == null || newDestination.DestinationMessage == null) return DestinationForm();
        
        if (file != null)
        {
            string wwwPath = this.WEBHOST.WebRootPath;
            string contentPath = this.WEBHOST.ContentRootPath;

            string path = Path.Combine(this.WEBHOST.WebRootPath, "uploads");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            string fileName = Path.GetFileName(file.FileName);

            using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
            file.CopyTo(stream);
            string uploadFile = fileName;
            newDestination.Image = fileName;
        } 
        else
        {
            newDestination.Image = "x"; // Set Default value
        }
        newDestination.UserId = (int)id;

        DATABASE.Destinations.Add(newDestination);
        DATABASE.SaveChanges();
        
        return RedirectToAction("Dashboard", "User");
    }
}