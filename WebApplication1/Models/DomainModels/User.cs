using System;
using System.ComponentModel.DataAnnotations;
using CloudinaryDotNet.Actions;
using WebApplication1.Models.DomainModels;
using WebApplication1.Types;
using Role = WebApplication1.Types.Role;

namespace WebApplication1.Models;

public class User
{
    [Key]
    public Guid Userid { get; set; } = Guid.NewGuid();
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public string? ProfilePicUrl { get; set; }
    public string? Phone {get; set ; }
    public string?Otp {get; set ; }
    public Role Role { get; set; } = Role.User;
    public Subescripetion subescripetion { get; set; } = Subescripetion.Nonsubescribed;

    public Address? Address { get; set; } //for defining relations
    public Cart? Cart { get; set; }
    public Subscription?subscriptions{ get; set; }
    public ICollection<Order>? Orders { get; set; } = [];
   

}
