using System;

namespace WebApplication1.Models.ViewModels;

public class PaymentIntentModel
{
    public int Amount { get; set; }
    public string? Currency { get; set; }
    public Guid OrderId { get; set; }
    

}
