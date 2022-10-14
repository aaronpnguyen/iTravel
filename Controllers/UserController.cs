using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using iTravel.Models;

namespace iTravel.Controllers;

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
    
    [HttpGet("/login/register")]
    public IActionResult LogReg()
    {
        if (!notLogged) return RedirectToAction("Dashboard");
        List<User> users = DATABASE.Users.ToList();
        ViewBag.Users = users;
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
        newUser.ProfilePic = "profile-default-svgrepo-com.svg"; // Need a default value for a profile pic
        DATABASE.Users.Add(newUser);
        DATABASE.SaveChanges();

        HttpContext.Session.SetInt32("id", newUser.UserId);
        HttpContext.Session.SetString("username", newUser.Username);
        List<User> users = DATABASE.Users.ToList();
        ViewBag.Users = users;

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
        HttpContext.Session.SetString("username", user.Username);
        List<User> users = DATABASE.Users.ToList();
        ViewBag.Users = users;

        return RedirectToAction("Dashboard");
    }

    [HttpGet("/")]
    public IActionResult Dashboard()
    {
        if(!notLogged)
        {
            HttpContext.Session.SetInt32("notLogged", 1);
        }
        else 
        {
            HttpContext.Session.SetInt32("notLogged", 0);
        }

        List<User> users = DATABASE.Users.ToList();
        ViewBag.Users = users;
        
        List<Destination> destinations = DATABASE.Destinations
            .Include(u => u.Creator)
            .OrderByDescending(d => d.CreatedAt)
            .ToList();

        return View("Dashboard", destinations);
    }

    [HttpGet("/user/{pid}")]
    public IActionResult UserProfile(int pid)
    {
        User? user = DATABASE.Users.FirstOrDefault(u => u.UserId == pid);
        if (user == null) return View("Error");

        Friend? friend = DATABASE.Friends
        .FirstOrDefault(f => (f.UserOneId == pid  && f.UserTwoId == id) || (f.UserOneId == id && f.UserTwoId == pid));

        if (friend == null) ViewBag.Friend = null;

        List<Friend> requests = DATABASE.Friends
            .Include(f => f.UserOne)
            .Where(f => f.UserTwoId == id && f.Relationship == "Pending")
            .ToList();

        List<Friend> listOfFriends = DATABASE.Friends
            .Include(f => f.UserOne)
            .Include(f => f.UserTwo)
            .Where(f => (f.UserOneId == pid && f.Relationship == "Friends") || (f.UserTwoId == pid && f.Relationship == "Friends"))
            .ToList();
        
        List<Destination> destinations = DATABASE.Destinations
            .Include(d => d.Creator)
            .Where(u => u.UserId == pid)
            .ToList();
        List<User> users = DATABASE.Users.ToList();
        ViewBag.Users = users;
        ViewBag.User = user;
        ViewBag.Friend = friend;
        ViewBag.Requests = requests;
        ViewBag.ListOfFriends = listOfFriends;
        ViewBag.Destinations = destinations;
        return View("UserProfile");
    }

    [HttpPost("/friend/request/{pid}")]
    public IActionResult SendFriendRequest(int pid, Friend newFriend)
    {
        if (notLogged) return RedirectToAction("LogReg", "User");
        newFriend.UserOneId = (int)id; // This sets the friend requester to user one
        newFriend.UserTwoId = pid; // UserTwoId is user we want to be friends with
        newFriend.Relationship = "Pending"; // Relationship status
        DATABASE.Friends.Add(newFriend);
        DATABASE.SaveChanges();
        List<User> users = DATABASE.Users.ToList();
        ViewBag.Users = users;
        return RedirectToAction("UserProfile", new {pid = pid});
    }

    [HttpPost("/friend/accept/{pid}")]
    public IActionResult AcceptRequest(int pid, Friend newFriend)
    {
        if (notLogged) return RedirectToAction("LogReg", "User");
        Friend? original = DATABASE.Friends
            .FirstOrDefault(f => f.UserOneId == pid && f.UserTwoId == id);

        original.Relationship = "Friends";
        DATABASE.Friends.Update(original);
        DATABASE.SaveChanges();
        List<User> users = DATABASE.Users.ToList();
        ViewBag.Users = users;
        return RedirectToAction("UserProfile", new {pid = id});
    }

    [HttpPost("/friend/cancel/{pid}")]
    public IActionResult CancelRequest(int pid, Friend newFriend)
    {
        if (notLogged) return RedirectToAction("LogReg", "User");
        Friend? original = DATABASE.Friends
            .FirstOrDefault(f => (f.UserOneId == id && f.UserTwoId == pid) || (f.UserOneId == pid && f.UserTwoId == id));
    
        DATABASE.Friends.Remove(original);
        DATABASE.SaveChanges();
        List<User> users = DATABASE.Users.ToList();
        ViewBag.Users = users;
        return RedirectToAction("UserProfile", new {pid = pid});
    }

    [HttpPost("/clear/id")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Dashboard");
    }

    [HttpGet("/edit/user/{uid}")]
    public IActionResult EditUser(int uid)
    {
        if (notLogged) return RedirectToAction("LogReg", "User");
        if (uid != id) return RedirectToAction("UserProfile", new {pid = id});
        User? user = DATABASE.Users.FirstOrDefault(u => u.UserId == id);
        List<User> users = DATABASE.Users.ToList();
        ViewBag.Users = users;
        return View("EditForm", user);
    }
}