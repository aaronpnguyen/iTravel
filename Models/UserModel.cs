#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iTravel.Models;

public class User
{
    [Key]
    public int UserId {get;set;}

    [Required(ErrorMessage = "is requried!")]
    [MinLength(2, ErrorMessage = "must contain more than 2 characters!")]
    [Display(Name = "First Name ")]
    public string FirstName {get;set;}

    [Required(ErrorMessage = "is requried!")]
    [MinLength(2, ErrorMessage = "must contain more than 2 characters!")]
    [Display(Name = "Last Name ")]
    public string LastName {get;set;}

    [Required(ErrorMessage = "is required!")]
    [MinLength(5, ErrorMessage = "must be more than 5 characters!")]
    [Display(Name = "Username ")]
    public string Username {get; set;}
    
    [Required(ErrorMessage = "is requried!")]
    [EmailAddress]
    [Display(Name = "Email ")]
    public string Email {get;set;}

    [Display(Name = "Profile Picture: ")]
    public string? ProfilePic {get; set;}

    [Required(ErrorMessage = "is required!")]
    [MinLength(8, ErrorMessage = "must contain more than 8 characters!")]
    [DataType(DataType.Password)]
    [Display(Name = "Password ")]
    public string Password {get;set;}

    [NotMapped]
    [Compare("Password", ErrorMessage = "does not match password!")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password ")]
    public string Confirm {get;set;}

    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime UpdatedAt {get;set;} = DateTime.Now;

    [InverseProperty("UserOne")]
    public List<Friend> FriendsForUserTwo {get;set;} = new List<Friend>();
    
    [InverseProperty("UserTwo")]
    public List<Friend> FriendsForUserOne {get;set;} = new List<Friend>();
    
    public List<Connection> Comments {get;set;} = new List<Connection>();

    public string Full()
    {
        return $"{FirstName} {LastName}";
    }
}