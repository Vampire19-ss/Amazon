using System;
using WebApplication1.Models.DomainModels;

namespace WebApplication1.Models.ViewModels;

public class OrderView
{
    public Order? Order { get; set; }

    public IEnumerable<Order> Orders { get; set; } = [];

}
