#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iTravel.Models;

public class Destination
{
    [Key]
    public int DestinationId {get;set;}

    [Required]
    public string City {get;set;}

    [Required]
    [Display(Name = "State/Province")]
    public string State {get;set;}

    [Required]
    public string Country {get;set;}

    [Display(Name = "Caption ")]
    [Required]
    public string DestinationMessage {get;set;}

    [Display(Name = "Image: ")]
    public string? Image {get; set;}

    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime UpdatedAt {get;set;} = DateTime.Now;

    public int UserId {get;set;}
    public User? Creator {get;set;}



    public string Place()
    {
        return $"{City}, {Country}";
    }

    public string Query()
    {
        return $"{City}+{State}+{Country}";
    }
}