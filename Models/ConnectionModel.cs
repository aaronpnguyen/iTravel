#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iTravel.Models;

public class Connection
{
    [Key]
    public int ConnectionId {get;set;}

    public int UserId {get;set;}
    public User? User {get;set;}
    
    public int DestinationId {get;set;}
    public Destination? Destination {get;set;}

    public string CommentMessage {get;set;}

    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime UpdatedAt {get;set;} = DateTime.Now;
}