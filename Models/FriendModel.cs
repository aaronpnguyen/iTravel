#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iTravel.Models;

public class Friend
{
    [Key]
    public int FriendId {get;set;}
    
    public int UserOneId {get;set;}
    [ForeignKey("UserOneId")]
    public User? UserOne {get;set;}

    public int UserTwoId {get;set;}
    [ForeignKey("UserTwoId")]
    public User? UserTwo {get;set;}
    
    public string Relationship {get;set;}

    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime UpdatedAt {get;set;} = DateTime.Now;
}