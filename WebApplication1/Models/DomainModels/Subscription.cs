using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Types;

namespace WebApplication1.Models.DomainModels;

public class Subscription
{

    [Key]
    public Guid Subid { get; set; } = Guid.NewGuid();

    public required Guid UserId { get; set; }  // Fk 
    [ForeignKey("UserId")]
    public User? Buyer { get; set; }

    public decimal Total { get; set; } = 0;
    
    public Subescripetion subescripetion { get; set; } = Subescripetion.Nonsubescribed;

     public  Guid Id { get; set; }  // Fk 
    [ForeignKey("Id")]
    public CreateSubscription? createSubscription  { get; set; }
}
