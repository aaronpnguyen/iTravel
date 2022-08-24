using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using iTravel.Models;

namespace WeddingPlanner.Controllers;

public class UserController: Controller
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

    public UserController(MyContext context)
    {
        DATABASE = context;
    }
    
    [HttpGet("/")]
    public IActionResult LogReg()
    {
        if (!notLogged) return RedirectToAction("Dashboard");
        return View("LogReg");
    }

    [HttpPost("/create/user")]
    public IActionResult CreateUser(User newUser)
    {
        if (ModelState.IsValid)
        {
            if (DATABASE.Users.Any(user => user.Username == newUser.Username))
            {
                ModelState.AddModelError("Username", "already in use!");
                if (DATABASE.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "already in use!");
                    return LogReg();
                }
                return LogReg();
            }
            if (DATABASE.Users.Any(u => u.Email == newUser.Email))
            {
                ModelState.AddModelError("Email", "already in use!");
                return LogReg();
            }
        }

        if (!ModelState.IsValid) return LogReg();
        PasswordHasher<User> hashed = new PasswordHasher<User>();
        newUser.Password = hashed.HashPassword(newUser, newUser.Password);
        newUser.ProfilePic = ""; // Need a default value for a profile pic
        DATABASE.Users.Add(newUser);
        DATABASE.SaveChanges();

        HttpContext.Session.SetInt32("id", newUser.UserId);
        return RedirectToAction("Dashboard");
    }

    [HttpPost("/login/user")]
    public IActionResult UserLogin(LoginUser logUser)
    {
        if (!ModelState.IsValid) return LogReg();

        User? user = DATABASE.Users.FirstOrDefault(user => user.Username == logUser.LoginUsername);

        if (user == null)
        {
            ModelState.AddModelError("LoginUsername", "may be incorrect");
            ModelState.AddModelError("LoginPassword", "may be incorrect");
            return LogReg();
        }
        
        PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();
        PasswordVerificationResult validator = hasher.VerifyHashedPassword(logUser, user.Password, logUser.LoginPassword);

        if (validator == 0)
        {
            ModelState.AddModelError("LoginEmail", "may be incorrect");
            ModelState.AddModelError("LoginPassword", "may be incorrect");
            return LogReg();
        }
        HttpContext.Session.SetInt32("id", user.UserId);
        return RedirectToAction("Dashboard");
    }

    [HttpGet("/dashboard")]
    public IActionResult Dashboard()
    {
        if (notLogged) return RedirectToAction("LogReg");

        List<Destination> Destinations = DATABASE.Destinations.Include(u => u.Creator).ToList();

        return View("Dashboard", Destinations);
    }

    [HttpGet("/user/{id}")]
    public IActionResult UserProfile(int id = 4)
    {
        if (notLogged) return RedirectToAction("LogReg");
        User? user = DATABASE.Users.FirstOrDefault(u => u.UserId == id);
        
        if (user == null)
        {
            return View("Error");
        }

        return View("UserProfile", user);
    }

    [HttpPost("/clear/id")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("LogReg");
    }
}