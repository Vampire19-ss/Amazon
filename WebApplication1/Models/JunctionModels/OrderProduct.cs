using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Models.DomainModels;

namespace WebApplication1.Models.JunctionModels;

public class OrderProduct
{
     public  Guid OrderProductId { get; set; } = Guid.NewGuid();   // pk


    public required Guid OrderId { get; set; } // Fk
    [ForeignKey("OrderId")]
    public Order? Order { get; set; } // navigation property //  belonging to an order


    public required Guid ProductId { get; set; } // Fk
    [ForeignKey("ProductId")]    // here is question why i dont ask to sir??????????
    public Product? Product { get; set; } // navigation property //  belonging to a product
    public required int Quantity { get; set; } = 1;

    
    public string? Size { get; set; }
    public string? Color { get; set; }
    public string? Weight { get; set; }



    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;



}
