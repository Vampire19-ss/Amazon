using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Models.DomainModels;

namespace WebApplication1.Models.JunctionModels;

public class CartProduct
{
        [Key]
    public  Guid CartProductId { get; set; } = Guid.NewGuid();   // pk 


    public required Guid CartId { get; set; } // Fk
    [ForeignKey("CartId")]
    public Cart? Cart { get; set; } // navigation property //  belonging to a cart




    public required Guid ProductId { get; set; } // Fk
    [ForeignKey("ProductId")]   
    public Product? Product { get; set; } // navigation property //  belonging to a product
    
    public required int Quantity { get; set; } = 1;//how mch products


    public string? Size { get; set; }
    public string? Color { get; set; }
    public string? Weight { get; set; }



    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

}
//actual practice from here to understand tables confirm