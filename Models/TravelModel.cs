#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iTravel.Models;

public class Travel
{
    [Key]
    public int TravelId {get;set;}

    public int UserId {get;set;}
    public User? User {get;set;}

    public int DestinationId {get;set;}
    public Destination? Destination {get;set;}
    
}