using System;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Types;

namespace WebApplication1.Models.DomainModels;

public class CreateSubscriptionss
{


    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string CreateSubscriptionName { get; set; }
    public required string Description { get; set; }
    public decimal Discount { get; set; } = 0;
    public decimal Total { get; set; } = 0;
}
