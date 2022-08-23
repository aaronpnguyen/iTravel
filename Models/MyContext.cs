#pragma warning disable CS8618
using Microsoft.EntityFrameworkCore;

namespace iTravel.Models;

public class MyContext: DbContext 
{ 
    public MyContext(DbContextOptions options): base(options) {}

    public DbSet<User> Users {get;set;}

    public DbSet<Destination> Destinations {get;set;}

    public DbSet<Connection> Connections {get;set;}

    public DbSet<Friend> Friends {get;set;}
}