using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using iTravel.Models;

namespace iTravel.Controllers;

public class DestinationController: Controller
{
    private MyContext DATABASE;

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

    public DestinationController(MyContext context)
    {
        DATABASE = context;
    }

    [HttpGet("/create/destination/form")]
    public IActionResult DestinationForm()
    {
        if (notLogged) return RedirectToAction("LogReg", "User");
        return View("DestinationForm");
    }

    [HttpPost("/submit/destination/form")]
    public IActionResult SubmitDestination(Destination newDestination)
    {
        if (notLogged) return RedirectToAction("LogReg", "User");
        if (!ModelState.IsValid) return DestinationForm();

        // Set userId for created destination
        if (id != null) newDestination.UserId = (int)id; // Removes yellow squiggly boy

        DATABASE.Destinations.Add(newDestination);
        DATABASE.SaveChanges();
        return RedirectToAction("Dashboard", "User");
    }
}